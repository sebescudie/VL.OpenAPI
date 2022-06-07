using System;
using System.Linq;
using VL.Core;

namespace VL.OpenAPI
{
    internal class OpenAPINode : VLObject, IVLNode
    {
        readonly OpenAPINodeDescription description;
        readonly Pin resultPin;
        readonly Pin runPin;

        // This is where we'll run the queries to the Directus instance

        public OpenAPINode(OpenAPINodeDescription description, NodeContext nodeContext) : base(nodeContext)
        {
            this.description = description;
            Inputs = description.Inputs.Select(p => new Pin() { Name = p.Name, Type = p.Type, Value = p.DefaultValue }).ToArray();
            Outputs = description.Outputs.Select(p => new Pin() { Name = p.Name, Type = p.Type, Value = p.DefaultValue }).ToArray();

            resultPin = Outputs.FirstOrDefault(o => o.Name == "Result");
            runPin = Inputs.LastOrDefault();
        }

        public IVLNodeDescription NodeDescription => description;

        public Pin[] Inputs { get; }
        public Pin[] Outputs { get; }

        public void Update()
        {
            // Execute the query here
            if (runPin is null || !(bool)runPin.Value)
                return;

            Console.WriteLine("======");
            Console.WriteLine("HTTP method is " + description.FOperation.Key);
            Console.WriteLine("Path is " + description.FPath);
        }

        public void Dispose()
        {
            Console.WriteLine("Byyyye");
        }

        IVLPin[] IVLNode.Inputs => Inputs;
        IVLPin[] IVLNode.Outputs => Outputs;
    }
}