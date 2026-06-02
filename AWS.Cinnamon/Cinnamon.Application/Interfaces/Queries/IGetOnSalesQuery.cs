using Cinnamon.Domain.Entities;

namespace Cinnamon.Application.Interfaces.Queries
{
    public interface IGetOnSalesQuery
    {
        Task<List<Product>> ExecuteAsync();
    }
}
