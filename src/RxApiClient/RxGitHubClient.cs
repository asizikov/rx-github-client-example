using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using RxApiClient.Caches;
using RxApiClient.Model;

namespace RxApiClient
{
    public sealed class RxGitHubClient : IRatingClient
    {
        private IRatingCache Cache { get; set; }
        private IHttpClient HttpClient { get; set; }
        private IScheduler Scheduler { get; set; }

        public RxGitHubClient(IRatingCache cache, IHttpClient httpClient, IScheduler scheduler)
        {
            Cache = cache;
            HttpClient = httpClient;
            Scheduler = scheduler;
        }

        public IObservable<RatingModel> GetRatingForUser(string userName)
        {
            return GetRatingInternal(userName)
                .WithCache(() => Cache.GetCachedItem(userName), model => Cache.Put(model))
                .DistinctUntilChanged(new RatingModelComparer());
        }

        private IObservable<RatingModel> GetRatingInternal(string userName)
        {
            return Observable.Create<RatingModel>(observer =>
                Scheduler.Schedule(async () =>
                {
                    var ratingResponse = await HttpClient.Get(userName);
                    if (!ratingResponse.IsSuccessful)
                    {
                        observer.OnError(ratingResponse.Exception);
                    }
                    else
                    {
                        observer.OnNext(ratingResponse.Data);
                        observer.OnCompleted();
                    }
                }));
        }
    }
}