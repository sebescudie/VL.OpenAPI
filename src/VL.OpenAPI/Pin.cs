using VL.Core;
using System;

namespace VL.OpenAPI
{
    public class Pin : IVLPin
    {
        public object Value { get; set; }
        public Type Type { get; set; }
        public string Name { get; set; }
        public string OriginalName { get; set; }
    }
}