using System;

namespace GraphProcessor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WindowTitleAttribute : Attribute
    {
        public readonly string title;

        public WindowTitleAttribute(string title)
        {
            this.title = title;
        }
    }
}
