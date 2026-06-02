using Cinnamon.Domain.Entities;

namespace Cinnamon.Application.Interfaces.Queries
{
    public interface IGetProductsByCategoryAndInStock
    {
        Task<List<Product>> ExecuteAsync(string category, bool inStock);
    }
}
