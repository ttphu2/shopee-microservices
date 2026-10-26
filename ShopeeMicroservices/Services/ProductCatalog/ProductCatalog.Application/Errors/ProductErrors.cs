using System.Globalization;
using Infrastructure.Abstractions.Messaging;
using ProductCatalog.Application.Resources;

namespace ProductCatalog.Application.Errors
{
    // Messages come from ServerMessages.resx and are read on each access, so they follow the current UI culture.
    public static class ProductErrors
    {
        public const string SkuCodeConflictCode = "Product.SkuCodeConflict";

        public static Error BrandNotFound => new("Product.BrandNotFound", ServerMessages.BrandNotFound);

        public static Error CategoryNotFound => new("Product.CategoryNotFound", ServerMessages.CategoryNotFound);

        public static Error SkuCodeConflict(IEnumerable<string> skuCodes) =>
            new(SkuCodeConflictCode, string.Format(CultureInfo.CurrentCulture, ServerMessages.SkuCodeConflict, string.Join(", ", skuCodes)));
    }
}
