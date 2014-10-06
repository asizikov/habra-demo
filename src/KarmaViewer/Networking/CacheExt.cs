using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using KarmaViewer.Domain;

namespace KarmaViewer.Networking
{
    public static class CacheExt
    {
        public static IObservable<KarmaModel> WithCache(this IObservable<KarmaModel> source, string userName, ICache cache)
        {
            return Observable.Create<KarmaModel>(observer =>
            {
                if (cache.HasCached(userName))
                {
                    var karma = cache.GetCachedItem(userName);
                    observer.OnNext(karma);
                }
                source.Subscribe(updatedKarma =>
                {
                    cache.Put(updatedKarma);
                    observer.OnNext(updatedKarma);

                    observer.OnCompleted();
                }, observer.OnError);
                return Disposable.Empty;
            });
        }
    }
}