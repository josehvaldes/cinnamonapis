using Cinnamon.Contracts.Responses;

namespace AWS.Cinnamon.Api.Services
{
    public interface ILinkService
    {
        IReadOnlyList<Link> GetProductLinks(string id);
    }
}
