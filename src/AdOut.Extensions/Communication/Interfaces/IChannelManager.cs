using RabbitMQ.Client;

namespace AdOut.Extensions.Communication.Interfaces
{
    public interface IChannelManager
    {
        IModel GetConsumerChannel();
        IModel GetPublisherChannel();
        void ReturnPublisherChannel(IModel channel);
    }
}
