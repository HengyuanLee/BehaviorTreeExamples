
using System;
using UnityEngine;

namespace GraphProcessor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeColorAttribute : Attribute
    {
        public readonly Color color;

        public NodeColorAttribute(float r = 0, float g = 0, float b = 0, float a = 1)
        {
            this.color = new Color(r, g, b, a);
        }
    }
}