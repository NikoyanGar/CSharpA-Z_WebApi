using _001_DomainDtoExplain.Models.Doamin.AggregatesAndAggregateRoots;
using _001_DomainDtoExplain.Repositories.Abstractions;

namespace _001_DomainDtoExplain
{
    public class OrderService
    {
        private readonly IProductRepository _productRepository;

        public OrderService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public void PlaceOrder(Order order)
        {
            foreach (var item in order.Items)
            {
                var product = _productRepository.GetById(item.Product.Id);
                product.UpdateStock(item.Quantity);
                _productRepository.Update(product);
            }
        }
    }

}
