using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

namespace Songify.Simple
{
    public class SlugifyParametersTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            if (value is null) return null;

            return Regex.Replace(value.ToString() ?? string.Empty, "([a-z])([A-Z])", "$1-$2").ToLower();
        }
    }
}