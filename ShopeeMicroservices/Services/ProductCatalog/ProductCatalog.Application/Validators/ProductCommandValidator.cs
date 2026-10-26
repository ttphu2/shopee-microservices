using System.Globalization;
using FluentValidation;
using ProductCatalog.Application.Handlers.Common;
using ProductCatalog.Application.Resources;
using ProductCatalog.Application.Utils;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Validators
{
    internal sealed class ProductCommandValidator : AbstractValidator<IProductCommand>
    {
        public ProductCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.BrandId).NotEmpty();
            RuleFor(x => x.CategoryId).NotEmpty();
            RuleFor(x => x.Images).NotNull();
            RuleForEach(x => x.Images).NotEmpty();

            RuleFor(x => x.VariantOptions).NotNull();
            RuleForEach(x => x.VariantOptions).ChildRules(option =>
            {
                option.RuleFor(o => o.Code).NotEmpty();
                option.RuleFor(o => o.Name).NotEmpty();
                option.RuleFor(o => o.Values).NotEmpty();
                option.RuleFor(o => o.Values)
                    .Must(values => values.Distinct(StringComparer.OrdinalIgnoreCase).Count() == values.Count)
                    .When(o => o.Values is not null)
                    .WithMessage(_ => ServerMessages.VariantOptionValuesNotUnique);
            });
            RuleFor(x => x.VariantOptions)
                .Must(options => options.Select(o => o.Code).Distinct(StringComparer.OrdinalIgnoreCase).Count() == options.Count)
                .When(x => x.VariantOptions is not null)
                .WithMessage(_ => ServerMessages.VariantOptionCodesNotUnique);

            RuleFor(x => x.Skus).NotEmpty().WithMessage(_ => ServerMessages.SkusRequired);
            RuleForEach(x => x.Skus).ChildRules(sku =>
            {
                sku.RuleFor(s => s.SkuCode).NotEmpty();
                sku.RuleFor(s => s.Price).GreaterThan(0);
                sku.RuleFor(s => s.Attributes).NotNull();
            });
            RuleFor(x => x.Skus)
                .Must(skus => skus.Select(s => s.SkuCode).Distinct(StringComparer.Ordinal).Count() == skus.Count)
                .When(x => x.Skus is not null)
                .WithMessage(_ => ServerMessages.SkuCodesNotUnique);
            RuleForEach(x => x.Skus)
                .Must((command, sku) => MatchesVariantOptions(sku.Attributes, command.VariantOptions))
                .When(x => x.VariantOptions is not null)
                .WithMessage((_, sku) => string.Format(CultureInfo.CurrentCulture, ServerMessages.SkuAttributesMismatch, sku.SkuCode));
            RuleFor(x => x.Skus)
                .Must(skus => skus.Select(s => VariantKeyBuilder.Build(s.Attributes)).Distinct().Count() == skus.Count)
                .When(x => x.Skus is not null && x.Skus.All(HasCompleteAttributes))
                .WithMessage(_ => ServerMessages.SkuCombinationsNotUnique);
        }

        private static bool MatchesVariantOptions(IReadOnlyCollection<SkuAttribute>? attributes, IReadOnlyCollection<VariantOption> options)
        {
            if (attributes is null)
            {
                return true; // Reported by the Attributes NotNull rule.
            }

            if (attributes.Count != options.Count)
            {
                return false;
            }

            foreach (var option in options)
            {
                var matching = attributes
                    .Where(attribute => string.Equals(attribute.AttributeCode, option.Code, StringComparison.Ordinal))
                    .ToList();

                if (matching.Count != 1 || option.Values is null || !option.Values.Contains(matching[0].Value, StringComparer.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool HasCompleteAttributes(ProductSkuCommand sku) =>
            sku.Attributes is not null && sku.Attributes.All(a => a.AttributeCode is not null && a.Value is not null);
    }
}
