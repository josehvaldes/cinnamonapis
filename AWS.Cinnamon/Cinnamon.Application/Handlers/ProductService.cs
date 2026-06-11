using Cinnamon.Application.Common;
using Cinnamon.Application.Interfaces;
using Cinnamon.Application.Interfaces.Queries;
using Cinnamon.Domain.Entities;

namespace Cinnamon.Application.Handlers
{
    public class ProductService : IProductService
    {
        private readonly IGetProductsByIdQuery _getProductById;
        private readonly IGetProductsByCategoryQuery _getProductsByCategory;
        public ProductService(
            IGetProductsByCategoryQuery getProductsByCategory,
            IGetProductsByIdQuery getProductById)
        {
            _getProductsByCategory = getProductsByCategory;
            _getProductById = getProductById;
        }

        public Task<List<Product>> GetNewArrivals() => _getProductsByCategory.ExecuteAsync("new-arrivals");

        public Task<List<Product>> GetTrending() => _getProductsByCategory.ExecuteAsync("trendings");

        public Task<List<Product>> GetOnSales() => _getProductsByCategory.ExecuteAsync("on-sales");

        public Task<Product> GetProductById(string id) => _getProductById.ExecuteAsync(id);

        public Task<PagedResult<Product>> GetProductsByCategory(string category, int pageNumber, int pageSize) => _getProductsByCategory.ExecuteAsync(category, pageNumber, pageSize);
    }
}
