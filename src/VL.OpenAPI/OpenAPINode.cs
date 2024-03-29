﻿using System;
using System.Linq;
using VL.Core;
using RestSharp;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Collections;

namespace VL.OpenAPI
{
    internal class OpenAPINode : VLObject, IVLNode
    {
        readonly OpenAPINodeDescription description;
        readonly Pin resultPin;
        readonly Pin runPin;

        private RestClient client;
        private RestRequest request;

        private string authParameterName;

        // This is where we'll run the queries to the Directus instance

        public OpenAPINode(OpenAPINodeDescription description, NodeContext nodeContext) : base(nodeContext)
        {
            this.description = description;
            Inputs = description.Inputs.Select(p => new Pin() { Name = p.Name, OriginalName = ((PinDescription)p).OriginalName, Type = p.Type, Value = p.DefaultValue }).ToArray();
            Outputs = description.Outputs.Select(p => new Pin() { Name = p.Name, OriginalName = ((PinDescription)p).OriginalName, Type = p.Type, Value = p.DefaultValue }).ToArray();

            resultPin = Outputs.FirstOrDefault(o => o.Name == "Result");
            runPin = Inputs.LastOrDefault();

            // Create RestClient & RestRequest
            client = new RestClient(description.FEndpoint + description.FPath);
            Method method = new Method();
            bool meth = Enum.TryParse<Method>(description.FOperation.Key.ToString(), out method);
            request = new RestRequest("", method);

            // Look for authentication stuff
            try
            {
                authParameterName = description.FSecuritySchemes.FirstOrDefault(x => x.Value.In == ParameterLocation.Query).Value.Name;
                request.AddOrUpdateParameter(authParameterName, description.FAPIKey);
                Console.WriteLine("Added auth parameter " + authParameterName);
            }
            catch (Exception ex)
            {
                authParameterName = "";
                Console.WriteLine("No query-based authentication found");
            }
        }

        public IVLNodeDescription NodeDescription => description;

        public Pin[] Inputs { get; }
        public Pin[] Outputs { get; }

        public void Update()
        {
            if (runPin is null || !(bool)runPin.Value)
                return;

            // Clear all params except auth!
            // Is it better to do that or just create a new request?
            foreach(var param in request.Parameters.Where(x => x.Name != authParameterName))
            {
                request.Parameters.Remove(param);
            }

            // Look for pins that actually have a value and add them as params
            foreach(var input in Inputs.Cast<Pin>().SkipLast(1))
            {
                // That looks a bit convoluted
                if (input.Type == typeof(IEnumerable<string>) && ((int)typeof(ICollection).GetProperty("Count").GetValue(input.Value, null)) > 0)
                {
                    request.AddOrUpdateParameter(input.OriginalName, string.Join(",", (IEnumerable<string>)input.Value));
                }
                else if(input.Type == typeof(string) && !(string.IsNullOrEmpty(input.Value as string)))
                {
                    request.AddOrUpdateParameter(input.OriginalName, (string)input.Value);
                }
                else if(input.Type == typeof(bool))
                {
                    // TBD
                }
            }

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