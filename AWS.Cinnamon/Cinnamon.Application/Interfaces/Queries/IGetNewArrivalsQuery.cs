using Cinnamon.Domain.Entities;

namespace Cinnamon.Application.Interfaces.Queries
{
    public interface IGetNewArrivalsQuery
    {
        Task<List<Product>> ExecuteAsync();
    }
}
