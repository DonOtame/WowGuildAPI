using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WowGuildAPI.Models.Enums;

namespace WowGuildAPI.SwaggerFilters
{
    public class RegionSchemaOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var parameter in operation.Parameters)
            {
                if (parameter.Name == "region")
                {
                    parameter.Schema.Enum.Clear();
                    parameter.Schema.Enum.Add(new OpenApiString(Region.us));
                    parameter.Schema.Enum.Add(new OpenApiString(Region.eu));
                    parameter.Schema.Enum.Add(new OpenApiString(Region.tw));
                    parameter.Schema.Enum.Add(new OpenApiString(Region.kr));
                    parameter.Schema.Enum.Add(new OpenApiString(Region.cn));
                }
            }
        }
    }

}
