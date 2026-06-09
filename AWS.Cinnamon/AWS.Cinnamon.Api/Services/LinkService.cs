using Cinnamon.Contracts.Responses;

namespace AWS.Cinnamon.Api.Services
{
    public class LinkService(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor) : ILinkService
    {
        public IReadOnlyList<Link> GetProductLinks(string id)
        {
            var httpContext = httpContextAccessor.HttpContext!;

            return
            [
                new Link(
                    linkGenerator.GetUriByAction(httpContext, "GetProduct", "Products", new { id }) ?? string.Empty,
                    "self",
                    "GET"),
                //new Link(
                //    linkGenerator.GetUriByAction(httpContext, "DeleteProduct", "Products", new { id }) ?? string.Empty,
                //    "delete-product",
                //    "DELETE"),
                //new Link(
                //    linkGenerator.GetUriByAction(httpContext, "CreateProduct", "Products", null) ?? string.Empty,
                //    "create-product",
                //    "POST")
            ];
        }
    }
}
