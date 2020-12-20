namespace AdOut.Extensions.Communication
{
    public class RabbitConfig
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public string HostName { get; set; }
        public int MaxRetriesToConnect { get; set; }
        public int IntervalToConnectMs { get; set; }
        public int MaxChannelsInPool { get; set; }
    }
}
