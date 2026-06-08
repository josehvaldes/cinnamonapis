using Cinnamon.Domain.Entities;
using Cinnamon.Infrastructure.AWS.Model;
using Mapster;

namespace Cinnamon.Infrastructure.AWS
{
    public static class AwsMapping
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<ProductItem, Product>.NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Img, src => src.Img)
                .Map(dest => dest.Price, src => (float)src.Price);
        }
    }
}
