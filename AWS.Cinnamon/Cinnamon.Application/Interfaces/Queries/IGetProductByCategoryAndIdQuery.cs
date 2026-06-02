using Cinnamon.Domain.Entities;

namespace Cinnamon.Application.Interfaces.Queries
{
    public interface IGetProductByCategoryAndIdQuery
    {
        Task<Product?> ExecuteAsync(string category, string id);
    }
}
