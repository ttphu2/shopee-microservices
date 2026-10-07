using ProductCatalog.Application.DTOs;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Mappers
{
    public static class ProductCatalogMapper
    {
        public static ProductSpuDto ToDto(this ProductSpu entity)
        {
            return new ProductSpuDto(
                entity.Id,
                entity.ProductCode,
                entity.Name,
                entity.Description,
                entity.BrandId,
                entity.CategoryId,
                entity.Images,
                entity.VariantOptions.Select(ToDto).ToList(),
                entity.CreatedDate,
                entity.CreatedBy,
                entity.LastModified,
                entity.LastModifiedBy);
        }

        public static IList<ProductSpuDto> ToDtoList(this IEnumerable<ProductSpu> products) => products.Select(p => p.ToDto()).ToList();

        public static ProductSkuDto ToDto(this ProductSku entity)
        {
            return new ProductSkuDto(
                entity.Id,
                entity.SpuId,
                entity.SkuCode,
                entity.Price,
                entity.ImageUrl,
                entity.Attributes
                    .Select(ToDto)
                    .ToList(),
                entity.CreatedDate,
                entity.CreatedBy,
                entity.LastModified,
                entity.LastModifiedBy);
        }

        public static ProductBrandDto ToDto(this ProductBrand entity)
        {
            return new ProductBrandDto(
                entity.Id,
                entity.Name);
        }

        public static ProductTypeDto ToDto(this ProductType entity)
        {
            return new ProductTypeDto(
                entity.Id,
                entity.Name);
        }

        private static VariantOptionDto ToDto(VariantOption entity)
        {
            return new VariantOptionDto(
                entity.Name,
                entity.Code,
                entity.Values);
        }

        private static SkuAttributeDto ToDto(SkuAttribute entity)
        {
            return new SkuAttributeDto(
                entity.AttributeCode,
                entity.Value);
        }
    }
}
