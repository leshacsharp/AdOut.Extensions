using AdOut.Extensions.Communication;
using AdOut.Extensions.Communication.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace AdOut.Extensions.Context
{
    public class CommitProvider<TContext> : ICommitProvider where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly IMessageBroker _messageBroker;
        private readonly IMapper _mapper;

        public CommitProvider(
            TContext context,
            IMessageBroker messageBroker,
            IMapper mapper)
        {
            _context = context;
            _messageBroker = messageBroker;
            _mapper = mapper;
        }

        public async Task<int> SaveChangesAsync(bool generateEvents = true, CancellationToken cancellationToken = default)
        {
            var integrationEvents = generateEvents ? GenerateCRUDIntegrationEvents() : new List<IntegrationEvent>();
            var countChanges = await _context.SaveChangesAsync();

            //sending events AFTER successed execution SaveChanges to avoid no consistent data
            foreach (var integrationEvent in integrationEvents)
            {
                _messageBroker.Publish(integrationEvent);
            }

            return countChanges;
        }

        private List<IntegrationEvent> GenerateCRUDIntegrationEvents()
        {
            var integrationEvents = new List<IntegrationEvent>();
            var entries = _context.ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                var entityType = entry.Entity.GetType();
                var entityStateName = ((EventReason)(int)entry.State).ToString();
                var eventName = $"{entityType.Name}{entityStateName}Event";

                var eventType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.BaseType == typeof(IntegrationEvent) &&
                                         t.Name.Equals(eventName, StringComparison.OrdinalIgnoreCase));
                     
                if (eventType != null)
                {
                    var integrationEvent = (IntegrationEvent)_mapper.Map(entry.Entity, entityType, eventType);
                    integrationEvents.Add(integrationEvent);
                }
            }

            return integrationEvents;
        }
    }
}
