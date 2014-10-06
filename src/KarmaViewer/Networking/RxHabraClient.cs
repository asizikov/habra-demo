using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using KarmaViewer.Domain;

namespace KarmaViewer.Networking
{
    public sealed class RxHabraClient : IHabraClient
    {
        private ICache Cache { get; set; }
        private IHttpClient HttpClient { get; set; }
        private IScheduler Scheduler { get; set; }

        public RxHabraClient(ICache cache, IHttpClient httpClient, IScheduler scheduler)
        {
            Cache = cache;
            HttpClient = httpClient;
            Scheduler = scheduler;
        }

        public IObservable<KarmaModel> GetKarmaForUser(string userName)
        {
            return GetKarma(userName)
                .WithCache(() => Cache.GetCachedItem(userName), model => Cache.Put(model))
                .DistinctUntilChanged(new KarmaComparer());
        }

        private IObservable<KarmaModel> GetKarma(string userName)
        {
            return Observable.Create<KarmaModel>(observer =>
                Scheduler.Schedule(async () =>
                {
                    var karmaResponse = await HttpClient.Get(userName);
                    if (!karmaResponse.IsSuccessful)
                    {
                        observer.OnError(karmaResponse.Exception);
                    }
                    else
                    {
                        observer.OnNext(karmaResponse.Data);
                        observer.OnCompleted();
                    }
                }));
        }
    }
}