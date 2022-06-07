using Microsoft.OpenApi.Readers;

var httpClient = new HttpClient
{
    BaseAddress = new Uri("http://localhost:8055/server/specs/oas?access_token=AYGr6sCTk6lH4N2F-N1TcAi2R6Sw-FJB")
};

var stream = await httpClient.GetStreamAsync("");

OpenApiDiagnostic diagnostic = new OpenApiDiagnostic();

// Read V3 as YAML
var openApiDocument = new OpenApiStreamReader().Read(stream, out diagnostic);

foreach(var thing in openApiDocument.Paths)
{
    Console.WriteLine(thing.Key);
}

Console.ReadLine();