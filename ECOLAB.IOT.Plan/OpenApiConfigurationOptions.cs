using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

namespace ECOLAB.IOT.Plan
{
    internal class OpenApiConfigurationOptions : IOpenApiConfigurationOptions
    {
        public OpenApiInfo Info { get; set; } =
          new OpenApiInfo
          {
              Title = "My API Documentation",
              Version = "1.0",
              Description = "a long description of my APIs",
              Contact = new OpenApiContact()
              {
                  Name = "My name",
                  Email = "myemail@company.com",
                  Url = new Uri("https://github.com/Azure/azure-functions-openapi-extension/issues"),
              },
              License = new OpenApiLicense()
              {
                  Name = "MIT",
                  Url = new Uri("http://opensource.org/licenses/MIT"),
              }
          };

        public List<OpenApiServer> Servers { get; set; } = new List<OpenApiServer>()
            {
                //new OpenApiServer() { Url = "https://localhost:7071/api/" },
                new OpenApiServer() { Url = "https://cn-ins-edmiot-functionapp-cleardbdata-d.chinacloudsites.cn/api/" },
            };

        public OpenApiVersionType OpenApiVersion { get; set; } = OpenApiVersionType.V3;

        public bool IncludeRequestingHostName { get; set; } = false;
        public bool ForceHttp { get; set; } = true;
        public bool ForceHttps { get; set; } = false;
        //public List<IDocumentFilter> DocumentFilters { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public List<IDocumentFilter> DocumentFilters { get; set; } = new();
    }
}
