namespace ProductCatalog.Domain.Entities
{
    public class Inventory : BaseEntity
    {
        public Guid SkuId { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
    }
}