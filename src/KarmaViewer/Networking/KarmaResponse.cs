using System;
using KarmaViewer.Domain;

namespace KarmaViewer.Networking
{
    public class KarmaResponse
    {
        public bool IsSuccessful { get; set; }
        public KarmaModel Data { get; set; }
        public Exception Exception { get; set; }
    }
}