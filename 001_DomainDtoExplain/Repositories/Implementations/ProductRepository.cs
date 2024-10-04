using _001_DomainDtoExplain.Models.Doamin.Entities;
using _001_DomainDtoExplain.Repositories.Abstractions;

namespace _001_DomainDtoExplain.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public Product? GetById(int id)
        {
            return _context.Products.FirstOrDefault(p => p.Id == id);
        }

        public void Add(Product product)
        {
            _context.Products.Add(product);
        }

        public void Update(Product product)
        {
            var findedproduct = _context.Products.FirstOrDefault(p => p.Id == product.Id);
            if (findedproduct != null)
            {
                findedproduct.Name = product.Name;
                findedproduct.Price = product.Price;
                findedproduct.Stock = product.Stock;
            }
        }

        public void Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
        }

        public List<Product> GetAll()
        {
            return _context.Products;
        }
    }
}
