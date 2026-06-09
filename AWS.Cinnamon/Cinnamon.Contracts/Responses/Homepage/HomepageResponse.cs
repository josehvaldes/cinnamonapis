namespace Cinnamon.Contracts.Responses.Homepage
{
    public class HomepageResponse
    {
        public IReadOnlyList<ProductResponse> Trendings { get; set; } = [];
        public IReadOnlyList<ProductResponse> NewArrivals { get; set; } = [];
        public IReadOnlyList<ProductResponse> OnSales { get; set; } = [];
    }
}
