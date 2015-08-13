using System.Threading.Tasks;

namespace RxApiClient
{
    public interface IHttpClient
    {
        Task<RatingResponse> Get(string userName);
    }
}