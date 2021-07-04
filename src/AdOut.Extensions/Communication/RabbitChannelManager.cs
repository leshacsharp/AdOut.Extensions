using AdOut.Extensions.Communication.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AdOut.Extensions.Communication
{
    public class RabbitChannelManager : IChannelManager, IDisposable
    {
        private readonly RabbitConfig _config;
        private readonly Queue<IModel> _sharedChannels;
        private readonly object _syncDequeue = new object();
        private readonly object _syncEnqueue = new object();
        private IConnection _connection;

        public RabbitChannelManager(IOptions<RabbitConfig> config)
        {
            _config = config.Value;
            _connection = CreateConnection();  
            _sharedChannels = new Queue<IModel>();
        }

        private IConnection Connection
        {
            get 
            {
                if (_connection == null)
                {
                    _connection = CreateConnection();
                }
                return _connection;
            }    
        }

        public IModel GetConsumerChannel()
        {
            return Connection.CreateModel();
        }

        public IModel GetPublisherChannel()
        {
            lock (_syncDequeue)
            {
                if (_sharedChannels.Count > 0)
                {
                    return _sharedChannels.Dequeue();
                }
                else
                {
                    return Connection.CreateModel();
                }
            }
        }

        public void ReturnPublisherChannel(IModel channel)
        {
            lock (_syncEnqueue)
            {
                if (_sharedChannels.Count < _config.MaxChannelsInPool)
                {
                    if (channel.IsOpen)
                    {
                        _sharedChannels.Enqueue(channel);
                    }
                }
                else
                {
                    channel.Dispose();
                }
            }
        }

        private IConnection CreateConnection()
        {
            var connectionFactory = new ConnectionFactory()
            {
                DispatchConsumersAsync = true,
                AutomaticRecoveryEnabled = true,
                UserName = _config.UserName,
                Password = _config.Password,
                VirtualHost = _config.VirtualHost,
                HostName = _config.HostName
            };

            IConnection connection = null;
            var countAttemptsToConnect = 0;
            var isConnected = false;

            while (!isConnected)
            {
                try
                {
                    connection = connectionFactory.CreateConnection();
                    isConnected = true;
                }
                catch (BrokerUnreachableException)
                {
                    countAttemptsToConnect++;
                    if (countAttemptsToConnect == _config.MaxRetriesToConnect)
                    {
                        //todo: throw new exceptions with data about attempts to recove a connection
                        throw;
                    }
                    else
                    {
                        Thread.Sleep(_config.IntervalToConnectMs);
                    }
                }
            }

            return connection;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
