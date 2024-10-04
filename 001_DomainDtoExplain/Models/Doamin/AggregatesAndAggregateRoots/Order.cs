using _001_DomainDtoExplain.Models.Doamin.Entities;

namespace _001_DomainDtoExplain.Models.Doamin.AggregatesAndAggregateRoots
{
    //Aggregates are clusters of domain objects that are treated as a single unit.
    //The aggregate root is the main entity that ensures the consistency of changes within the aggregate.
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        public void AddItem(Product product, int quantity)
        {
            var orderItem = new OrderItem(product, quantity);
            Items.Add(orderItem);
        }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public OrderItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }

}
