using System;

namespace Konamiman.NestorMSX
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NestorMSXPluginAttribute : Attribute
    {
        public NestorMSXPluginAttribute(string name)
        {
            this.Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
        }

        public NestorMSXPluginAttribute() : this("")
        {
        }

        public string Name { get; }
    }
}
