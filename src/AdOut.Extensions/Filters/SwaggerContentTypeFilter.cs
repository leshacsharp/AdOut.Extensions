using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace AdOut.Extensions.Filters
{
    public class SwaggerContentTypeFilter : IOperationFilter
    {
        private readonly string[] _mediaTypes;
        public SwaggerContentTypeFilter(string[] mediaTypes)
        {
            _mediaTypes = mediaTypes;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.RequestBody != null)
            {
                foreach (var content in operation.RequestBody.Content)
                {
                    if (!_mediaTypes.Contains(content.Key))
                    {
                        operation.RequestBody.Content.Remove(content.Key);
                    }
                }
            }

            foreach (var response in operation.Responses)
            {
                foreach (var content in response.Value.Content)
                {
                    if (!_mediaTypes.Contains(content.Key))
                    {
                        response.Value.Content.Remove(content.Key);
                    }
                }
            }
        }
    }
}
