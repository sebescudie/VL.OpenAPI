using System;
using VL.Core;

namespace VL.OpenAPI
{
    class PinDescription : IVLPinDescription, IInfo
    {

        public string Name { get; }
        public string OriginalName { get; }
        public Type Type { get; set; }
        public object DefaultValue { get; }

        public string Summary { get; }

        public string Remarks => "";

        public PinDescription(string name, Type type, object defaultValue, string description)
        {
            Name = Utils.ToPascalCase(name);
            OriginalName = name;
            Type = type;
            DefaultValue = defaultValue;
            Summary = description;
        }
    }
}
