using System;
using RxApiClient.Model;

namespace RxApiClient
{
    public interface IRatingClient
    {
        IObservable<RatingModel> GetRatingForUser(string userName);
    }
}