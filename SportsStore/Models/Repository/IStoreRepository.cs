namespace SportsStore.Models.Repository
{
    public interface IStoreRepository
    {
        IQueryable<Product> Products { get; }
<<<<<<< HEAD
=======

        void SaveProduct(Product p);

        void CreateProduct(Product p);

        void DeleteProduct(Product p);
>>>>>>> sports-store-application-4
    }
}
