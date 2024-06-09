namespace SportsStore.Models.Repository
{
<<<<<<< HEAD
    public class EFStoreRepository: IStoreRepository
    {
        private StoreDbContext context;
=======
    public class EFStoreRepository : IStoreRepository
    {
        private readonly StoreDbContext context;
>>>>>>> sports-store-application-4

        public EFStoreRepository(StoreDbContext ctx)
        {
            this.context = ctx;
        }

        public IQueryable<Product> Products => this.context.Products;

<<<<<<< HEAD
=======
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
>>>>>>> sports-store-application-4
    }
}
