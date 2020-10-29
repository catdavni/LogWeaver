using System;

namespace Extensions
{
    [System.AttributeUsage(AttributeTargets.Class | System.AttributeTargets.Struct, AllowMultiple = false)]
    public class LoggingAttribute : Attribute
    {
    }
}
