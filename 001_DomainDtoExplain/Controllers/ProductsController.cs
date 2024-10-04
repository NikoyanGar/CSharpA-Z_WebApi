using _001_DomainDtoExplain.Models.Doamin.Entities;
using _001_DomainDtoExplain.Repositories.Abstractions;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace _001_DomainDtoExplain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //The controller is responsible for handling incoming HTTP requests and returning responses to the client.
    //for demonstrate not decoupled models problems 
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return _productRepository.GetAll();
        }


        [HttpGet("{id}")]
        public Product Get(int id)
        {
            return _productRepository.GetById(id);
        }


        [HttpPost]
        public void Post([FromBody] Product value)
        {
            _productRepository.Add(value);
        }


        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Product value)
        {
            _productRepository.Update(value);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _productRepository.Delete(id);
        }
    }
}
