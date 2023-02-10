using System;

namespace GraphProcessor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeNameAttribute : Attribute
    {
        public readonly string name;
        public NodeNameAttribute(string name)
        {
            this.name = name;
        }
    }
}
