using _001_DomainDtoExplain.Models.Doamin.Entities;

namespace _001_DomainDtoExplain.Repositories
{
    public class AppDbContext
    {
        private List<Product> _products;

        public List<Product> Products
        {
            get { return _products; }
            set { _products = value; }
        }

        public AppDbContext()
        {
            _products = Enumerable.Range(1, 10).Select(x => new Product
            {
                Id = x,
                Name = $"Product {x}",
                Price = x * 10,
                Stock = 100
            }).ToList();
        }
    }
}