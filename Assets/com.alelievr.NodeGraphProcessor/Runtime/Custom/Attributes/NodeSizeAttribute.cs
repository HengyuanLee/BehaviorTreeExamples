using System;
namespace GraphProcessor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeSizeAttribute : Attribute
    {
        public readonly float width;
        public readonly float height;

        public NodeSizeAttribute(float width = 0, float height = 0)
        {
            this.width = width;
            this.height = height;
        }
    }
}
