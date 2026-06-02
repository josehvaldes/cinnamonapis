using Cinnamon.Domain.Entities;

namespace Cinnamon.Application.Interfaces.Queries
{
    public interface IGetTrendingQuery
    {
        Task<List<Product>> ExecuteAsync();
    }
}
