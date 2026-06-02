using Cinnamon.Domain.Entities;

namespace Cinnamon.Application.Interfaces.Queries
{
    public interface IGetProductsByIdQuery
    {
        Task<List<Product>> ExecuteAsync(string id);
    }
}
