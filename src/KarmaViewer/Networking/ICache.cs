using KarmaViewer.Domain;

namespace KarmaViewer.Networking
{
    public interface ICache
    {
        bool HasCached(string userName);
        KarmaModel GetCachedItem(string userName);
        void Put(KarmaModel updatedKarma);
    }
}