using System;
using System.Collections.Generic;
using System.Text;

namespace Cinnamon.Application.Common
{
    public class RatingMessage
    {
        public RatingMessageType MessageType { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public string Rating { get; set; } = string.Empty;
    }
}
