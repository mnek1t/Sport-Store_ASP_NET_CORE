namespace SportsStore.Models.Repository
{
    public class EFStoreRepository: IStoreRepository
    {
        private StoreDbContext context;

        public EFStoreRepository(StoreDbContext ctx)
        {
            this.context = ctx;
        }

        public IQueryable<Product> Products => this.context.Products;

    }
}
