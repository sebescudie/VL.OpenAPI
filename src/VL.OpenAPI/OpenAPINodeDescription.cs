using System.Reactive.Linq;
using VL.Core;
using VL.Core.Diagnostics;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VL.OpenAPI
{
    class OpenAPINodeDescription : IVLNodeDescription, IInfo
    {
        // Fields
        bool FInitialized;
        bool FError;

        string FSummary;
        string FCategory;

        // Inputs and outputs
        List<PinDescription> inputs = new List<PinDescription>();
        List<PinDescription> outputs = new List<PinDescription>();

        public OpenAPINodeDescription(IVLNodeDescriptionFactory factory, string category, KeyValuePair<OperationType,OpenApiOperation> operation)
        {
            Factory = factory;
            Name = operation.Value.OperationId;
            FCategory = category;
            FSummary = operation.Value.Description;
            FOperation = operation;
        }

        void Init()
        {
            if (FInitialized)
                return;

            try
            {
                Type type = typeof(object);
                object dflt = "";
                string name = "";
                string desc = "";

                // Retrieve parameters from the OpenAPI dump and create input pins
                foreach(var parameter in FOperation.Value.Parameters)
                {
                    GetTypeDefaultAndDescription(parameter, ref type, ref dflt, ref desc);
                    inputs.Add(new PinDescription(parameter.Name, type, dflt, desc));
                }

                // Adds the trigger pin
                inputs.Add(new PinDescription("Execute", typeof(bool), false, "Sends a query as long as enabled"));

                // For now let's just get the raw JSON response from Directus. Create a single string output pin
                outputs.Add(new PinDescription("Result", typeof(string),"", "The raw string response"));

                FInitialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        void GetTypeDefaultAndDescription(OpenApiParameter parameter, ref Type type, ref object dflt, ref string desc)
        {
            desc = parameter.Description;

            if(parameter.Schema.Type == "string")
            {
                type = typeof(string);
                dflt = "";
            }
            else if(parameter.Schema.Type == "boolean")
            {
                type = typeof(bool);
                dflt = false;
            }
            else if(parameter.Schema.Type == "array")
            {
                if(parameter.Schema.Items.Type == "string")
                {
                    type = typeof(IEnumerable<string>);
                    dflt = Enumerable.Repeat<string>("", 0).ToArray();
                }
            }
        }
        private KeyValuePair<OperationType, OpenApiOperation> FOperation;
        public IVLNodeDescriptionFactory Factory { get; }
        public string Name { get; }
        public string Category => "OpenAPI";
        public bool Fragmented => false;
        public IReadOnlyList<IVLPinDescription> Inputs
        {
            get
            {
                Init();
                return inputs;
            }    
        }
        public IReadOnlyList<IVLPinDescription> Outputs
        {
            get
            {
                Init();
                return outputs;
            }
        }

        public IEnumerable<Core.Diagnostics.Message> Messages
        {
            get
            {
                if(FError) 
                    yield return new Message(MessageType.Warning, "Grrrr");
                else
                    yield break;
            }    
        }

        public string Summary => FSummary;
        public string Remarks => "";
        public IObservable<object> Invalidated => Observable.Empty<object>();
        public IVLNode CreateInstance(NodeContext context)
        {
            return new OpenAPINode(this, context);
        }
        public bool OpenEditor()
        {
            return true;
        }
    }
}