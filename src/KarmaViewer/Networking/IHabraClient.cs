using System;
using KarmaViewer.Domain;

namespace KarmaViewer.Networking
{
    public interface IHabraClient
    {
        IObservable<KarmaModel> GetKarmaForUser(string userName);
    }
}