using AdOut.Extensions.Authorization;
using AdOut.Extensions.Communication;
using AdOut.Extensions.Communication.Interfaces;
using AdOut.Extensions.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AdOut.Extensions.Context
{
    public class CommitProvider<TContext> : ICommitProvider where TContext : DbContext
    { 
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMessageBroker _messageBroker;
        private readonly TContext _context;
        private readonly IMapper _mapper;

        public CommitProvider(
            IHttpContextAccessor httpContextAccessor,
            IMessageBroker messageBroker,
            TContext context,   
            IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;   
            _messageBroker = messageBroker;
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> SaveChangesAsync(bool generateEvents = true, CancellationToken cancellationToken = default)
        {
            FillEntities();

            var integrationEvents = generateEvents ? GenerateCRUDIntegrationEvents() : new List<IntegrationEvent>();
            var countChanges = await _context.SaveChangesAsync();

            //sending events AFTER successed execution of SaveChanges method to avoid no consistent data
            foreach (var integrationEvent in integrationEvents)
            {
                _messageBroker.Publish(integrationEvent);
            }

            return countChanges;
        }

        private void FillEntities()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypeNames.UserId)?.Value;
            var entries = _context.ChangeTracker.Entries();

            foreach (var entry in entries)
            { 
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    var entity = (PersistentEntity)entry.Entity;
                    if (string.IsNullOrEmpty(entity.Creator))
                    {
                        entity.Creator = userId;
                    }             
                }
            }
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
