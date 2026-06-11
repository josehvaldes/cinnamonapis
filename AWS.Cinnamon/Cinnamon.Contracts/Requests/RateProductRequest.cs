using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinnamon.Contracts.Requests
{
    public class RateProductRequest
    {
        [Range(0, 5)]
        public int Value { get; set; }

        [AllowedValues("like", "rating")]
        public string RatingType { get; set; } = "like";

    }
}
