using System;

namespace AdOut.Extensions.Authorization
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class ResourceIdAttribute : Attribute
    {
    }
}
