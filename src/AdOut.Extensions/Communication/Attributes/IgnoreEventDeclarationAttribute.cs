using System;

namespace AdOut.Extensions.Communication.Attributes
{
    /// <summary>
    /// If set, the system won't create required message broker componenents for mantaining the event.
    /// </summary>
    public class IgnoreEventDeclarationAttribute : Attribute
    {
    }
}
