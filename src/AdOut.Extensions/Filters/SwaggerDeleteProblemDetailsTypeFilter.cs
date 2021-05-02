using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AdOut.Extensions.Filters
{
    public class SwaggerDeleteProblemDetailsTypeFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var response in operation.Responses)
            {
                foreach (var content in response.Value.Content)
                {
                    if (content.Value.Schema.Reference?.Id == nameof(ProblemDetails))
                    {
                        response.Value.Content.Remove(content.Key);
                    }
                }
            }
        }
    }
}
