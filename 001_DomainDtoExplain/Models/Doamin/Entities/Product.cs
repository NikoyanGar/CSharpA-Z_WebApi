namespace _001_DomainDtoExplain.Models.Doamin.Entities
{
    //Domain entities represent the core objects in your application. They typically correspond to tables in your database and contain business logic.
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public void UpdateStock(int quantity)
        {
            Stock -= quantity;
        }
    }

}
