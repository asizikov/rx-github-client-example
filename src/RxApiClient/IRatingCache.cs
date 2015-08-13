using RxApiClient.Caches;
using RxApiClient.Model;

namespace RxApiClient
{
    public interface IRatingCache : ICache<string, RatingModel>
    {
    }
}
