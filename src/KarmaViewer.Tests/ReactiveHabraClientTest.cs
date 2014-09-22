using System;
using KarmaViewer.Domain;
using KarmaViewer.Networking;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;

namespace KarmaViewer.Tests
{
    [TestFixture]
    public class ReactiveHabraClientTest : ReactiveTest
    {
        private Mock<IHttpClient> HttpClient { get; set; }
        private TestScheduler Scheduler { get; set; }
        private Mock<ICache> Cache { get; set; }
        private KarmaModel Model { get; set; }
        private KarmaModel NextModel { get; set; }

        private const string USER_NAME = "user";

        [SetUp]
        public void SetUp()
        {
            Model = new KarmaModel {Karma = 10, LastModified = new DateTime(2014, 09, 10, 1, 1, 1, 0), UserName = USER_NAME};
            NextModel = new KarmaModel {Karma = 10, LastModified = new DateTime(2014, 09, 11, 1, 1, 1, 0), UserName = USER_NAME};
            Cache = new Mock<ICache>();
            Scheduler = new TestScheduler();
            HttpClient = new Mock<IHttpClient>();
        }

        [Test]
        public void Test1()
        {
            Cache.Setup(cache => cache.HasCached(USER_NAME)).Returns(true);
            Cache.Setup(cache => cache.GetCachedItem(USER_NAME)).Returns(Model);

            HttpClient.Setup(http => http.Get(USER_NAME)).ReturnsAsync(new KarmaResponse
            {
                Data = NextModel,
                IsSuccessful = true
            });
            var client = new ReactiveHabraClient(Cache.Object, HttpClient.Object, Scheduler);
            var expected = Scheduler.CreateHotObservable(OnNext(201, Model), OnNext(201, NextModel), OnCompleted<KarmaModel>(201));


            var results = Scheduler.Start(() => client.GetKarmaForUser(USER_NAME), 0, 200, 500);
            ReactiveAssert.AreElementsEqual(expected.Messages, results.Messages);
            Cache.Verify(cache => cache.Put(NextModel));
        }

        [Test]
        public void Test2()
        {
            Cache.Setup(cache => cache.HasCached(USER_NAME)).Returns(true);
            Cache.Setup(cache => cache.GetCachedItem(USER_NAME)).Returns(Model);

            HttpClient.Setup(http => http.Get(USER_NAME)).ReturnsAsync(new KarmaResponse
            {
                Data = Model,
                IsSuccessful = true
            });
            var client = new ReactiveHabraClient(Cache.Object, HttpClient.Object, Scheduler);
            var expected = Scheduler.CreateHotObservable(OnNext(201, Model), OnCompleted<KarmaModel>(201));


            var results = Scheduler.Start(() => client.GetKarmaForUser(USER_NAME), 0, 200, 500);
            ReactiveAssert.AreElementsEqual(expected.Messages, results.Messages);
            Cache.Verify(cache => cache.Put(Model), Times.AtMostOnce);
        }

        [Test]
        public void IfCacheIsEmptyDownloadsDataAndReturnsIt()
        {
            Cache.Setup(c => c.HasCached(It.IsAny<string>())).Returns(false);
            HttpClient.Setup(http => http.Get(USER_NAME)).ReturnsAsync(new KarmaResponse
            {
                Data = Model,
                IsSuccessful = true
            });
            var client = new ReactiveHabraClient(Cache.Object, HttpClient.Object, Scheduler);
            var expected = Scheduler.CreateHotObservable(OnNext(2, Model), OnCompleted<KarmaModel>(2));

            var results = Scheduler.Start(() => client.GetKarmaForUser(USER_NAME), 0, 1, 10);

            ReactiveAssert.AreElementsEqual(expected.Messages, results.Messages);
            Cache.Verify(cache => cache.Put(Model), Times.Once);
        }

        [Test]
        public void Test4()
        {
            Cache.Setup(c => c.HasCached(It.IsAny<string>())).Returns(false);
            var exception = new Exception("something went wrong");
            HttpClient.Setup(http => http.Get(USER_NAME)).ReturnsAsync(new KarmaResponse
            {
                Data = null,
                Exception = exception,
                IsSuccessful = false
            });
            var client = new ReactiveHabraClient(Cache.Object, HttpClient.Object, Scheduler);
            var expected = Scheduler.CreateHotObservable(OnError<KarmaModel>(201, exception));


            var results = Scheduler.Start(() => client.GetKarmaForUser(USER_NAME), 0, 200, 500);
            ReactiveAssert.AreElementsEqual(expected.Messages, results.Messages);
        }
    }
}