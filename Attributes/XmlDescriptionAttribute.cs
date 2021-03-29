using System;

namespace DSPBotTemplate.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class XmlDescriptionAttribute : Attribute
    {
        public string description;

        public XmlDescriptionAttribute(string des) => description = des;
    }
}