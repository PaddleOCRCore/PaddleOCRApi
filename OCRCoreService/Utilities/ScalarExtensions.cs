using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace OCRCoreService
{
    /// <summary>
    /// Scalar API documentation extensions.
    /// </summary>
    internal static class ScalarExtensions
    {
        private const string DocumentName = "v1";

        public static void AddScalarOpenApi(this IServiceCollection services, IConfiguration configuration)
        {
            string headerName = configuration["ApiAuth:header_name"] ?? "PaddleOCR-Api-Key";
            if (string.IsNullOrWhiteSpace(headerName))
            {
                headerName = "PaddleOCR-Api-Key";
            }

            services.AddOpenApi(DocumentName, options =>
            {
                options.AddScalarTransformers();
                options.AddOperationTransformer((operation, context, _) =>
                {
                    string? operationName = GetOperationName(context.Description);
                    string? description = GetDescription(operation.Summary, operation.Description);
                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        operation.Description = description;
                    }

                    if (!string.IsNullOrWhiteSpace(operationName))
                    {
                        operation.Summary = operationName;
                        operation.OperationId ??= operationName;
                    }

                    return Task.CompletedTask;
                });
                options.AddDocumentTransformer((document, _, _) =>
                {
                    document.Info = new OpenApiInfo
                    {
                        Version = DocumentName,
                        Title = "WebAPI接口"
                    };

                    ApplyTagDescriptions(document);

                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                    document.Components.SecuritySchemes["ApiKey"] = new OpenApiSecurityScheme
                    {
                        Description = "请输入API Key",
                        Name = headerName,
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "ApiKeyScheme"
                    };

                    document.Security ??= new List<OpenApiSecurityRequirement>();
                    document.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("ApiKey", document, null)] = new List<string>()
                    });

                    return Task.CompletedTask;
                });
            });
        }

        private static void ApplyTagDescriptions(OpenApiDocument document)
        {
            if (document.Tags == null || document.Tags.Count == 0)
            {
                return;
            }

            IReadOnlyDictionary<string, string> descriptions = GetControllerTagDescriptions();
            foreach (OpenApiTag tag in document.Tags)
            {
                if (!string.IsNullOrWhiteSpace(tag.Name) &&
                    descriptions.TryGetValue(tag.Name, out string? description) &&
                    !string.IsNullOrWhiteSpace(description))
                {
                    tag.Description = description;
                }
            }
        }

        private static IReadOnlyDictionary<string, string> GetControllerTagDescriptions()
        {
            string xmlPath = Path.Combine(AppContext.BaseDirectory, $"{typeof(ScalarExtensions).Assembly.GetName().Name}.xml");
            if (!File.Exists(xmlPath))
            {
                return new Dictionary<string, string>();
            }

            XDocument xmlDocument = XDocument.Load(xmlPath);
            return xmlDocument
                .Descendants("member")
                .Select(member => new
                {
                    Name = member.Attribute("name")?.Value,
                    Summary = NormalizeXmlText(member.Element("summary")?.Value)
                })
                .Where(member =>
                    !string.IsNullOrWhiteSpace(member.Name) &&
                    member.Name.StartsWith("T:OCRCoreService.Controllers.", StringComparison.Ordinal) &&
                    member.Name.EndsWith("Controller", StringComparison.Ordinal) &&
                    !string.IsNullOrWhiteSpace(member.Summary))
                .ToDictionary(
                    member => member.Name!["T:OCRCoreService.Controllers.".Length..^"Controller".Length],
                    member => member.Summary!,
                    StringComparer.OrdinalIgnoreCase);
        }

        private static string? GetOperationName(Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription description)
        {
            if (description.ActionDescriptor.RouteValues.TryGetValue("action", out string? actionName) &&
                !string.IsNullOrWhiteSpace(actionName))
            {
                return actionName;
            }

            string? relativePath = description.RelativePath?.Split('?', 2)[0].Trim('/');
            return relativePath?.Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        }

        private static string? GetDescription(string? summary, string? description)
        {
            if (!string.IsNullOrWhiteSpace(description))
            {
                return description.Trim();
            }

            if (string.IsNullOrWhiteSpace(summary))
            {
                return null;
            }

            string[] lines = Regex.Split(summary.Trim(), @"\r?\n")
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToArray();

            if (lines.Length > 1 && lines[0].StartsWith("/", StringComparison.Ordinal))
            {
                return string.Join(Environment.NewLine, lines.Skip(1));
            }

            return summary.Trim();
        }

        private static string? NormalizeXmlText(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            string[] lines = Regex.Split(value.Trim(), @"\r?\n")
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToArray();

            return lines.Length == 0 ? null : string.Join(Environment.NewLine, lines);
        }

        public static void MapScalarOpenApi(this WebApplication app)
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.WithTitle("PaddleOCRApi V1");
            });
        }
    }
}
