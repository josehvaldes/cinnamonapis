using Cinnamon.Application.Interfaces;
using Cinnamon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinnamon.Application.Handlers
{
    public class ProductHandler: IHandler
    {
        public async Task<List<Product>> GetNewArrivals()
        {
            var products = new List<Product>
            {

                new Product { Name = "Blueberry Muffin", Img = "/products/blueberry-muffin-1929337_1280.jpg", Price = 2.49f },
                new Product { Name = "Strawberry Tart", Img = "/products/strawberry-tart-1929338_1280.jpg", Price = 3.99f },
            };

            return products;
        }

        public async Task<List<Product>> GetTrending()
        {
            var products = new List<Product>
            {
                new Product { Name = "Cinnamon Roll", Img = "/products/cinnamon-roll-1995428_1280.jpg", Price = 2.99f },
                new Product { Name = "Chocolate Croissant", Img = "/products/chocolate-croissant-1929336_1280.jpg", Price = 3.49f },
                new Product { Name = "Lemon Tart", Img = "/products/lemon-tart-1929339_1280.jpg", Price = 3.79f },
            };

            return products;
        }

        public async Task<List<Product>> GetOnSales()
        {
            var products = new List<Product>
            {
                new Product { Name = "Apple Pie", Img = "/products/apple-pie-1929335_1280.jpg", Price = 4.99f },
                new Product { Name = "Vanilla Cupcake", Img = "/products/vanilla-cupcake-1929334_1280.jpg", Price = 1.99f },
                new Product { Name = "Chocolate Brownie", Img = "/products/chocolate-brownie-1929333_1280.jpg", Price = 2.49f },
            };
            return products;
        }
    }
}
