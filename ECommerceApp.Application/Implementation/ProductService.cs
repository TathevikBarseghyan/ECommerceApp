using ECommerceApp.Application.Interfaces;
using ECommerceApp.Domain.Entities;

namespace ECommerceApp.Application.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _productRepository;

        public ProductService(IGenericRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync() =>
            await _productRepository.ListAllAsync();

        public async Task<Product> GetProductByIdAsync(int id) =>
            await _productRepository.GetByIdAsync(id);

        public async Task<Product> CreateProductAsync(Product product) =>
            await _productRepository.AddAsync(product);

        public async Task UpdateProductAsync(Product product) =>
            await _productRepository.UpdateAsync(product);

        public async Task DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product != null)
            {
                await _productRepository.DeleteAsync(product);
            }
        }
    }

}
