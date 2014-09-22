using System.Threading.Tasks;

namespace KarmaViewer.Networking
{
    public interface IHttpClient
    {
        Task<KarmaResponse> Get(string userName);
    }
}