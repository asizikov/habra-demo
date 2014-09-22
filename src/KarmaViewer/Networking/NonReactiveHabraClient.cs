using System.Threading.Tasks;
using KarmaViewer.Domain;

namespace KarmaViewer.Networking
{
    public sealed class NonReactiveHabraClient
    {
        private IHttpClient HttpClient { get; set; }

        public NonReactiveHabraClient(IHttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public async Task<KarmaModel> GetKarmaForUser(string userName)
        {
            var karmaResponse = await HttpClient.Get(userName);
            if (!karmaResponse.IsSuccessful)
            {
                throw karmaResponse.Exception;
            }
            return karmaResponse.Data;
        }
    }
}