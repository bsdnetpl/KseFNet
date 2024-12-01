using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace KseF.Models
    {
    public class DefaultSchemaFilter : ISchemaFilter
        {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
            {
            if (schema.Properties != null)
                {
                foreach (var property in schema.Properties)
                    {
                    if (property.Value.Default == null)
                        {
                        property.Value.Default = new Microsoft.OpenApi.Any.OpenApiString("");
                        }
                    }
                }
            }
        }
    }
