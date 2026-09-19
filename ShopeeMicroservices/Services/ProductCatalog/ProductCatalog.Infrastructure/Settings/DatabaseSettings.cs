namespace ProductCatalog.Infrastructure.Settings
{
    public class DatabaseSettings
    {
        public required string ConnectionString { get; init; }
        public required string DatabaseName { get; init; }
        public required string BrandCollectionName { get; init; }
        public required string TypeCollectionName { get; init; }
        public required string ProductSpuCollectionName { get; init; }
        public required string ProductSkusCollectionName { get; init; }
    }
}
