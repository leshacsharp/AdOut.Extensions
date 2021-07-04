using System;

namespace AdOut.Extensions.Communication.Attributes
{
    /// <summary>
    /// Message broker components settings for the event.
    /// By default, the system uses default settings. 
    /// The attribute is optional.
    /// </summary>
    public class EventDeclarationAttribute : Attribute
    {
        public EventDeclarationAttribute(ExchangeTypeEnum type = ExchangeTypeEnum.Fanout)
        {
            ExchangeType = type;
        }

        public ExchangeTypeEnum ExchangeType { get; set; }
    }
}
