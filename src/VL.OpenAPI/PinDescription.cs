using System;
using VL.Core;

namespace VL.OpenAPI
{
    class PinDescription : IVLPinDescription, IInfo
    {
        static string BeautifyPin(string s)
        {
            // Sanitize string
            return s;
        }

        public string Name { get; }
        public string OriginalName { get; }
        public Type Type { get; set; }
        public object DefaultValue { get; }

        public string Summary { get; }

        public string Remarks => "";

        public PinDescription(string name, Type type, object defaultValue, string description)
        {
            Name = BeautifyPin(name);
            OriginalName = name;
            Type = type;
            DefaultValue = defaultValue;
            Summary = description;
        }
    }
}
