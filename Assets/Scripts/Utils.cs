using System.Net.Http;

namespace DefaultNamespace
{
    public static class Utils
    {
        public static readonly HttpClient HttpClient = new HttpClient(new HttpClientHandler(), false);
    }
}