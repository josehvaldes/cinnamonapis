using Cinnamon.Application.Interfaces;
using Cinnamon.Application.Interfaces.Queries;
using Cinnamon.Domain.Entities;

namespace Cinnamon.Application.Handlers
{
    public class ProductHandler : IHandler
    {
        private readonly IGetNewArrivalsQuery _getNewArrivals;
        private readonly IGetTrendingQuery _getTrending;
        private readonly IGetOnSalesQuery _getOnSales;
        private readonly IGetProductsByIdQuery _getProductById;
        public ProductHandler(
            IGetNewArrivalsQuery getNewArrivals,
            IGetTrendingQuery getTrending,
            IGetOnSalesQuery getOnSales,
            IGetProductsByIdQuery getProductById)
        {
            _getNewArrivals = getNewArrivals;
            _getTrending = getTrending;
            _getOnSales = getOnSales;
            _getProductById = getProductById;
        }

        public Task<List<Product>> GetNewArrivals() => _getNewArrivals.ExecuteAsync();

        public Task<List<Product>> GetTrending() => _getTrending.ExecuteAsync();

        public Task<List<Product>> GetOnSales() => _getOnSales.ExecuteAsync();

        public Task<Product> GetProductById(string id) => _getProductById.ExecuteAsync(id);

    }
}
