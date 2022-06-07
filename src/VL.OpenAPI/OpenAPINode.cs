using System;
using System.Linq;
using VL.Core;
using RestSharp;

namespace VL.OpenAPI
{
    internal class OpenAPINode : VLObject, IVLNode
    {
        readonly OpenAPINodeDescription description;
        readonly Pin resultPin;
        readonly Pin runPin;

        private RestClient client;
        private RestRequest request;

        // This is where we'll run the queries to the Directus instance

        public OpenAPINode(OpenAPINodeDescription description, NodeContext nodeContext) : base(nodeContext)
        {
            this.description = description;
            Inputs = description.Inputs.Select(p => new Pin() { Name = p.Name, Type = p.Type, Value = p.DefaultValue }).ToArray();
            Outputs = description.Outputs.Select(p => new Pin() { Name = p.Name, Type = p.Type, Value = p.DefaultValue }).ToArray();

            resultPin = Outputs.FirstOrDefault(o => o.Name == "Result");
            runPin = Inputs.LastOrDefault();

            // Create RestClient & RestRequest
            client = new RestClient(description.FEndpoint + description.FPath);
            Method method = new Method();
            bool meth = Enum.TryParse<Method>(description.FOperation.Key.ToString(), out method);
            request = new RestRequest("", method);
        }

        public IVLNodeDescription NodeDescription => description;

        public Pin[] Inputs { get; }
        public Pin[] Outputs { get; }

        public void Update()
        {
            // Execute the query here
            if (runPin is null || !(bool)runPin.Value)
                return;

            // Console debug
            Console.WriteLine("======");
            Console.WriteLine("HTTP method is " + description.FOperation.Key);
            Console.WriteLine("Path is " + description.FPath);

            var response = client.Execute(request);

            resultPin.Value = response.Content;
        }

        public void Dispose()
        {
            Console.WriteLine("Byyyye");
        }

        IVLPin[] IVLNode.Inputs => Inputs;
        IVLPin[] IVLNode.Outputs => Outputs;
    }
}