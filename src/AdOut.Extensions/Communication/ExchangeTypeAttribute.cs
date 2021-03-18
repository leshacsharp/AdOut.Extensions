using System;

namespace AdOut.Extensions.Communication
{
    public class ExchangeTypeAttribute : Attribute
    {
        public ExchangeTypeEnum Type { get; set; }
        public ExchangeTypeAttribute(ExchangeTypeEnum type)
        {
            Type = type;
        }
    }
}
