using VL.Core;
using VL.Core.CompilerServices;

// Tell VL where to find the initializer
[assembly: AssemblyInitializer(typeof(VL.OpenAPI.Initialization))]

namespace VL.OpenAPI
{
    public class Initialization : AssemblyInitializer<Initialization>
    {
        protected override void RegisterServices(IVLFactory factory)
        {
            factory.RegisterNodeFactory(openAPIFactory);
        }

        static IVLNodeDescriptionFactory openAPIFactory = new OpenAPINodeFactory();
    }    
}