using System;
using System.Collections.Generic;
using System.Text;

namespace Cinnamon.Domain.Entities
{
    public class Product
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Img { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public float Price { get; set; }
        public bool InStock { get; set; }
    }
}
