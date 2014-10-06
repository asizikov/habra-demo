using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using KarmaViewer.Domain;

namespace KarmaViewer.Networking
{
    public static class CacheExt
    {
        public static IObservable<KarmaModel> WithCache(this IObservable<KarmaModel> source, Func<KarmaModel> get, Action<KarmaModel> put)
        {
            return Observable.Create<KarmaModel>(observer =>
            {
                var cached = get();
                if (cached != null)
                {
                    observer.OnNext(cached);
                }
                source.Subscribe(updatedKarma =>
                {
                    put(updatedKarma);
                    observer.OnNext(updatedKarma);

                    observer.OnCompleted();
                }, observer.OnError);
                return Disposable.Empty;
            });
        }
    }
}