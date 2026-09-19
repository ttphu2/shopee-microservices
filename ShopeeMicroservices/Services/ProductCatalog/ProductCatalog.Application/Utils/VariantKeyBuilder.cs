using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Utils
{
    public static class VariantKeyBuilder
    {
        public static string Build(
            IEnumerable<SkuAttribute> attributes)
        {
            return string.Join(
                "|",
                attributes
                    .OrderBy(x => x.AttributeCode)
                    .Select(x =>
                        $"{Normalize(x.AttributeCode)}:{Normalize(x.Value)}"));
        }

        private static string Normalize(string value)
        {
            return value
                .Trim()
                .ToLowerInvariant()
                .Replace(" ", "");
        }
    }
}
