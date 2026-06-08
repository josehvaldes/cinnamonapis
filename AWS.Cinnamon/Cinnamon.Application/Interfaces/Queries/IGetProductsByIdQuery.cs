using Cinnamon.Domain.Entities;

namespace Cinnamon.Application.Interfaces.Queries
{
    public interface IGetProductsByIdQuery
    {
        Task<Product> ExecuteAsync(string id);
    }
}
