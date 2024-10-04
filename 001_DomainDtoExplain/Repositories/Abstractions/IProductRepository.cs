using _001_DomainDtoExplain.Models.Doamin.Entities;

namespace _001_DomainDtoExplain.Repositories.Abstractions
{
    public interface IProductRepository
    {
        List<Product> GetAll();
        Product GetById(int id);
        void Add(Product product);
        void Update(Product product);
        void Delete(int id);
    }
}
