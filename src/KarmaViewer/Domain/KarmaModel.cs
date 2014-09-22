using System;

namespace KarmaViewer.Domain
{
    public class KarmaModel
    {
        public string UserName { get; set; }
        public int Karma { get; set; }
        public DateTime LastModified { get; set; }
    }
}
