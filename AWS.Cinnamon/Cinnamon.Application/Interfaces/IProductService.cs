using Cinnamon.Application.Common;
using Cinnamon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinnamon.Application.Interfaces
{
    public interface IProductService
    {
        public Task<List<Product>> GetNewArrivals();
        public Task<List<Product>> GetTrending();
        public Task<List<Product>> GetOnSales();
        public Task<Product> GetProductById(string id);
        public Task<PagedResult<Product>> GetProductsByCategory(string category, int pageNumber, int pageSize);
    }
}
