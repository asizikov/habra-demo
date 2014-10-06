using System.Collections.Generic;
using KarmaViewer.Domain;

namespace KarmaViewer.Networking
{
    public class KarmaComparer : IEqualityComparer<KarmaModel>
    {
        public bool Equals(KarmaModel x, KarmaModel y)
        {
            return x.Karma == y.Karma && x.LastModified == y.LastModified && x.UserName == y.UserName;
        }

        public int GetHashCode(KarmaModel obj)
        {
            return obj.GetHashCode();
        }
    }
}