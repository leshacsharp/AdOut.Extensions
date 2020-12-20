using AdOut.Extensions.Communication.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AdOut.Extensions.Communication
{
    public static class EventBrokerModule
    {
        public static void AddMessageBrokerServices(this IServiceCollection services)
        {
            services.AddSingleton<IChannelManager, RabbitChannelManager>();
            services.AddScoped<IMessageBroker, RabbitMessageBroker>();
            services.AddScoped<IMessageBrokerHelper, MessageBrokerHelper>();
            services.AddScoped<IConsumerBinder, ConsumerBinder>();
        }
    }
}
