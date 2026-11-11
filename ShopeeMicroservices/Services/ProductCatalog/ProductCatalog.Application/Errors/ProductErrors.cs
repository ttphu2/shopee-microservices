using System.Globalization;
using Infrastructure.Abstractions.Messaging;
using ProductCatalog.Application.Resources;

namespace ProductCatalog.Application.Errors
{
    // Codes are constants so callers (e.g. the API's HTTP status mapping) can match on them.
    // Fixed errors are created once; errors whose message depends on input are methods.
    public static class ProductErrors
    {
        public const string BrandNotFoundCode = "Product.BrandNotFound";
        public const string CategoryNotFoundCode = "Product.CategoryNotFound";
        public const string SkuCodeConflictCode = "Product.SkuCodeConflict";

        public static readonly Error BrandNotFound = new(BrandNotFoundCode, ServerMessages.BrandNotFound);

        public static readonly Error CategoryNotFound = new(CategoryNotFoundCode, ServerMessages.CategoryNotFound);

        public static Error SkuCodeConflict(IEnumerable<string> skuCodes) =>
            new(SkuCodeConflictCode, string.Format(CultureInfo.CurrentCulture, ServerMessages.SkuCodeConflict, string.Join(", ", skuCodes)));
    }
}
