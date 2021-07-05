using AdOut.Extensions.Authorization;
using AdOut.Extensions.Communication;
using AdOut.Extensions.Communication.Interfaces;
using AdOut.Extensions.Communication.Models;
using AdOut.Extensions.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AdOut.Extensions.Context
{
    public class CommitProvider<TContext> : ICommitProvider where TContext : DbContext
    { 
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMessageBroker _messageBroker;
        private readonly TContext _context;

        public CommitProvider(
            IHttpContextAccessor httpContextAccessor,
            IMessageBroker messageBroker,
            TContext context)
        {
            _httpContextAccessor = httpContextAccessor;   
            _messageBroker = messageBroker;
            _context = context;
        }

        public async Task<int> SaveChangesAsync(bool generateEvents = true, CancellationToken cancellationToken = default)
        {
            FillEntities();

            var integrationEvents = generateEvents ? GenerateReplicationEvents() : new List<IntegrationEvent>();
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
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypeNames.UserId)?.Value;
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

        private List<IntegrationEvent> GenerateReplicationEvents()
        {
            var integrationEvents = new List<IntegrationEvent>();
            var entries = _context.ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                var entityType = entry.Entity.GetType();
                var replicationEventType = typeof(ReplicationEvent<>).MakeGenericType(entityType);
                var replicationEvent = (IntegrationEvent)Activator.CreateInstance(replicationEventType);
                integrationEvents.Add(replicationEvent);
            }

            return integrationEvents;
        }
    }
}
