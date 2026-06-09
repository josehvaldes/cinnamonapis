using System;
using System.Collections.Generic;
using System.Text;

namespace Cinnamon.Contracts.Responses
{
    public record Link(string Href, string Rel, string Method);
}
