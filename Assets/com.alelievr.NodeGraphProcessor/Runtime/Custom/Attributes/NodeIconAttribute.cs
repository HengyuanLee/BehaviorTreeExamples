using System;

namespace GraphProcessor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeIconAttribute : Attribute
    {
        public readonly string icon;
        public NodeIconAttribute(string icon) {
            this.icon = icon;
        }
    }
}
