using Cinnamon.Application.Common;
using Cinnamon.Domain.Entities;

namespace Cinnamon.Application.Interfaces.Queries
{
    public interface IGetProductsByCategoryQuery
    {
        Task<List<Product>> ExecuteAsync(string category);

        Task<PagedResult<Product>> ExecuteAsync(string category, int pageNumber, int pageSize);
    }
}
