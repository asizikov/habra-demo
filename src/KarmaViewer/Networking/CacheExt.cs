﻿using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace KarmaViewer.Networking
{
    public static class CacheExt
    {
        public static IObservable<T> WithCache<T>(this IObservable<T> source, Func<T> get, Action<T> put) where T : class
        {
            return Observable.Create<T>(observer =>
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