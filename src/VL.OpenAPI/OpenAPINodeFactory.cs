using Microsoft.OpenApi.Readers;
using System.Collections.Immutable;
using VL.Core;
using RestSharp;
using System.Reactive.Linq;
using System;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace VL.OpenAPI
{
    public class OpenAPINodeFactory : IVLNodeDescriptionFactory
    {
        const string openAPIsubdir = "openAPI";
        const string identifier = "VL.OpenAPI-Factory";
        const string filename = "openapi.txt";

        public readonly string Directory;
        public readonly string DirectoryToWatch;

        // A regex to remove whitespacce from strings
        private static readonly Regex sWhitespace = new Regex(@"\s+");

        // The node factory cache will invalidate in case a cached factory or one of its nodes invalidates
        private readonly NodeFactoryCache factoryCache = new NodeFactoryCache();

        public OpenAPINodeFactory(string directory = default, string directoryToWatch = default)
        {
            Directory = directory;
            DirectoryToWatch = directoryToWatch;

            var builder = ImmutableArray.CreateBuilder<IVLNodeDescription>();

            if(directory != null)
            {
                // Make up file name
                var filePath = Path.Combine(directory, filename);

                // Iterate over lines of the file
                foreach(var line in File.ReadLines(filePath))
                {
                    // Split at coma, we expect the following format :
                    // [0] = Project name
                    // [1] = OpenAPI document URL
                    // [2] = API key
                    var elements = line.Split(',');

                    var projectName = sWhitespace.Replace(elements[0], "");
                    var endpoint = sWhitespace.Replace(elements[1], "");
                    var apiKey = sWhitespace.Replace(elements[2], "");

                    Console.WriteLine(String.Format("Found project {0}", projectName));
                    
                    // Extract domain and port
                    // We assume the openapi schema and resources are on the same domain/port
                    // Should we?
                    Uri uri = new Uri(endpoint);
                    string hostname = uri.ToString().Replace(uri.PathAndQuery, "");


                    // Query the OpenAPI schema
                    var client = new RestClient(endpoint);
                    var response = client.GetAsync(new RestRequest(), new CancellationToken()).GetAwaiter().GetResult().Content;
                    OpenApiDiagnostic diagnostic = new OpenApiDiagnostic();

                    // Parse schema
                    var openApiDocument = new OpenApiStringReader().Read(response, out diagnostic);

                    // Iterate over the paths
                    foreach (var path in openApiDocument.Paths)
                    {
                        // path.Key is the actual path of the request
                        foreach (var operation in path.Value.Operations)
                        {
                            // Category is hardcoded for now
                            builder.Add(new OpenAPINodeDescription(this, projectName, hostname, path.Key, operation));
                        }
                    }
                }
            }

            NodeDescriptions = builder.ToImmutable();
        }

        public ImmutableArray<IVLNodeDescription> NodeDescriptions { get; }

        public string Identifier
        {
            get
            {
                if (Directory != null)
                    return GetIdentifierForPath(Directory);
                else
                    return identifier;
            }
        }

        public IObservable<object> Invalidated
        {
            get
            {
                if(Directory != null)
                {
                    return NodeBuilding.WatchDir(Directory).Where(e => e.ChangeType == WatcherChangeTypes.All);
                }
                else if(DirectoryToWatch != null)
                {
                    return NodeBuilding.WatchDir(DirectoryToWatch).Where(e => e.ChangeType == WatcherChangeTypes.All);
                }
                else
                {
                    return Observable.Empty<object>();
                }
            }
        }

        public void Export(ExportContext exportContext)
        {

        }

        string GetIdentifierForPath(string path) => $"{identifier} ({path})";

        public IVLNodeDescriptionFactory ForPath(string path)
        {
            var identifier = GetIdentifierForPath(path);
            return factoryCache.GetOrAdd(identifier, () =>
            {
                var openAPIdir = Path.Combine(path, openAPIsubdir);
                if (System.IO.Directory.Exists(openAPIdir))
                    return new OpenAPINodeFactory(openAPIdir);
                return new OpenAPINodeFactory(directoryToWatch: path);
            });
        }
    }
}