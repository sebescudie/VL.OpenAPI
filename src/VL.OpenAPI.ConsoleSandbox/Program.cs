using Microsoft.OpenApi.Readers;

var httpClient = new HttpClient
{
    BaseAddress = new Uri("http://localhost:8055/server/specs/oas?access_token=AYGr6sCTk6lH4N2F-N1TcAi2R6Sw-FJB")
};

var stream = await httpClient.GetStreamAsync("");

OpenApiDiagnostic diagnostic = new OpenApiDiagnostic();

// Read V3 as YAML
var doc = new OpenApiStreamReader().Read(stream, out diagnostic);

foreach(var scheme in doc.Components.SecuritySchemes)
{
    Console.WriteLine(scheme.Key);
    Console.WriteLine("Name : " + scheme.Value.Name);
    Console.WriteLine("In : " + scheme.Value.In);
    Console.WriteLine("Type : " + scheme.Value.Type);
}

Console.ReadLine();