using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using KarmaViewer.Domain;

namespace KarmaViewer.Networking
{
    public sealed class ReactiveHabraClient : IHabraClient
    {
        private ICache Cache { get; set; }
        private IHttpClient HttpClient { get; set; }
        private IScheduler Scheduler { get; set; }

        public ReactiveHabraClient(ICache cache, IHttpClient httpClient, IScheduler scheduler)
        {
            Cache = cache;
            HttpClient = httpClient;
            Scheduler = scheduler;
        }

        public IObservable<KarmaModel> GetKarmaForUser(string userName)
        {
            return Observable.Create<KarmaModel>(observer =>
                Scheduler.Schedule(async () =>
                {
                    KarmaModel karma = null;
                    if (Cache.HasCached(userName))
                    {
                        karma = Cache.GetCachedItem(userName);
                        observer.OnNext(karma);
                    }

                    var karmaResponse = await HttpClient.Get(userName);
                    if (!karmaResponse.IsSuccessful)
                    {
                        observer.OnError(karmaResponse.Exception);
                        return;
                    }

                    var updatedKarma = karmaResponse.Data;
                    Cache.Put(updatedKarma);
                    if (karma == null || updatedKarma.LastModified > karma.LastModified)
                    {
                        observer.OnNext(updatedKarma);
                    }

                    observer.OnCompleted();
                }));
        }
    }
}