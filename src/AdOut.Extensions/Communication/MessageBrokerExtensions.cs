using AdOut.Extensions.Communication.Interfaces;
using AdOut.Extensions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace AdOut.Extensions.Communication
{
    public static class EventBrokerModule
    {
        public static IServiceCollection AddMessageBrokerServices(this IServiceCollection services)
        {
            services.AddSingleton<IChannelManager, RabbitChannelManager>();
            services.AddScoped<IMessageBroker, RabbitMessageBroker>();
            services.AddScoped<IMessageBrokerHelper, MessageBrokerHelper>();
            services.AddScoped<IInitialization, SimpleEventsInitialization>();
            services.AddScoped<IInitialization, ReplicationEventsInitialization>();
            return services;
        }
    }
}
