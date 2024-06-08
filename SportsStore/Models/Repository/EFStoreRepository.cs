namespace SportsStore.Models.Repository
{
    public class EFStoreRepository : IStoreRepository
    {
        private readonly StoreDbContext context;

        public EFStoreRepository(StoreDbContext ctx)
        {
            this.context = ctx;
        }

        public IQueryable<Product> Products => this.context.Products;

        public void CreateProduct(Product p)
        {
            this.context.Add(p);
            this.context.SaveChanges();
        }

        public void DeleteProduct(Product p)
        {
            this.context.Remove(p);
            this.context.SaveChanges();
        }

        public void SaveProduct(Product p)
        {
            if (p.ProductId == 0)
            {
                this.context.Products.Add(p);
            }
            else
            {
                Product? dbEntry = this.context.Products?.FirstOrDefault(p => p.ProductId == p.ProductId);

                if (dbEntry != null)
                {
                    dbEntry.Name = p.Name;
                    dbEntry.Description = p.Description;
                    dbEntry.Price = p.Price;
                    dbEntry.Category = p.Category;
                }
            }

            this.context.SaveChanges();
        }
    }
}
