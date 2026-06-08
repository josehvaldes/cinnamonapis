using Cinnamon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinnamon.Application.Interfaces
{
    public interface IHandler
    {
        public Task<List<Product>> GetNewArrivals();
        public Task<List<Product>> GetTrending();
        public Task<List<Product>> GetOnSales();
        public Task<Product> GetProductById(string id);
    }
}
