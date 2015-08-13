using System;
using Microsoft.Reactive.Testing;
using Moq;
using RxApiClient.Model;
using Xunit;

namespace RxApiClient.Tests
{
    public class RxGitHubClientTest : ReactiveTest
    {
        private Mock<IHttpClient> HttpClient { get; set; }
        private TestScheduler Scheduler { get; set; }
        private Mock<IRatingCache> Cache { get; set; }
        private RatingModel Model { get; set; }
        private RatingModel NextModel { get; set; }

        private const string UserName = "user";

        public RxGitHubClientTest()
        {
            Model = new RatingModel
            {
                Rating = 10, 
                LastModified = new DateTime(2015, 07, 10, 1, 1, 1, 0), 
                Id = UserName
            };
            NextModel = new RatingModel
            {
                Rating = 10,
                LastModified = new DateTime(2015, 07, 11, 1, 1, 1, 0),
                Id = UserName
            };
            Cache = new Mock<IRatingCache>();
            Scheduler = new TestScheduler();
            HttpClient = new Mock<IHttpClient>();
        }

        [Fact]
        public void ReturnsNewValueAfterCachedWhenCacheIsNotEmpty()
        {
            Cache.Setup(cache => cache.HasCached(UserName)).Returns(true);
            Cache.Setup(cache => cache.GetCachedItem(UserName)).Returns(Model);

            HttpClient.Setup(http => http.Get(UserName)).ReturnsAsync(new RatingResponse
            {
                Data = NextModel,
                IsSuccessful = true
            });
            var client = CreateClient();
            var expected = Scheduler.CreateHotObservable(OnNext(200, Model), OnNext(201, NextModel),
                OnCompleted<RatingModel>(201));


            var results = Scheduler.Start(() => client.GetRatingForUser(UserName), 0, 200, 500);
            ReactiveAssert.AreElementsEqual(expected.Messages, results.Messages);
            Cache.Verify(cache => cache.Put(NextModel));
        }

        [Fact]
        public void DoNotReturnTwiceWhenDataHasNotChanged()
        {
            Cache.Setup(cache => cache.HasCached(UserName)).Returns(true);
            Cache.Setup(cache => cache.GetCachedItem(UserName)).Returns(Model);

            HttpClient.Setup(http => http.Get(UserName)).ReturnsAsync(new RatingResponse
            {
                Data = Model,
                IsSuccessful = true
            });
            var client = CreateClient();
            var expected = Scheduler.CreateHotObservable(OnNext(200, Model), OnCompleted<RatingModel>(201));


            var results = Scheduler.Start(() => client.GetRatingForUser(UserName), 0, 200, 500);
            ReactiveAssert.AreElementsEqual(expected.Messages, results.Messages);
            Cache.Verify(cache => cache.Put(Model), Times.AtMostOnce);
        }

        [Fact]
        public void IfCacheIsEmptyDownloadsDataAndReturnsIt()
        {
            Cache.Setup(c => c.HasCached(It.IsAny<string>())).Returns(false);
            Cache.Setup(cache => cache.GetCachedItem(UserName)).Returns((RatingModel) null);
            HttpClient.Setup(http => http.Get(UserName)).ReturnsAsync(new RatingResponse
            {
                Data = Model,
                IsSuccessful = true
            });
            var client = CreateClient();
            var expected = Scheduler.CreateHotObservable(OnNext(2, Model), OnCompleted<RatingModel>(2));

            var results = Scheduler.Start(() => client.GetRatingForUser(UserName), 0, 1, 10);

            ReactiveAssert.AreElementsEqual(expected.Messages, results.Messages);
            Cache.Verify(cache => cache.Put(Model), Times.Once);
        }

        [Fact]
        public void ExecutesOnErrorWhenExceptionHappens()
        {
            Cache.Setup(c => c.HasCached(It.IsAny<string>())).Returns(false);
            Cache.Setup(cache => cache.GetCachedItem(UserName)).Returns((RatingModel) null);
            var exception = new Exception("something went wrong");
            HttpClient.Setup(http => http.Get(UserName)).ReturnsAsync(new RatingResponse
            {
                Data = null,
                Exception = exception,
                IsSuccessful = false
            });
            var client = CreateClient();
            var expected = Scheduler.CreateHotObservable(OnError<RatingModel>(201, exception));


            var results = Scheduler.Start(() => client.GetRatingForUser(UserName), 0, 200, 500);
            ReactiveAssert.AreElementsEqual(expected.Messages, results.Messages);
        }

        private RxGitHubClient CreateClient()
        {
            return new RxGitHubClient(Cache.Object, HttpClient.Object, Scheduler);
        }
    }
}