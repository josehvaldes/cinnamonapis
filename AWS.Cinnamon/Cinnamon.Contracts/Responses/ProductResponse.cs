using System;
using System.Collections.Generic;
using System.Text;

namespace Cinnamon.Contracts.Responses
{
    public class ProductResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Img { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;

        public IReadOnlyList<Link> Links { get; set; } = [];
    }
}
