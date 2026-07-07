using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinnamon.Contracts.Requests
{
    public class AddWishlistRequest
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [MinLength(1, ErrorMessage = "At least one product ID must be provided.")]
        public string[] ProductIds { get; set; } = Array.Empty<string>();
    }
}
