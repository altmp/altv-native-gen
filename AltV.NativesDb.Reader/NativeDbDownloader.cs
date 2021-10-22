using System.Net;
using AltV.NativesDb.Reader.Extensions;
using Newtonsoft.Json;

namespace AltV.NativesDb.Reader
{
    public class NativeDbDownloader
    {
        private readonly string _nativesJsonUrl;

        public NativeDbDownloader(string nativesJsonUrl)
        {
            _nativesJsonUrl = nativesJsonUrl;
        }

        public Models.NativeDb.NativeDb DownloadLatest()
        {
            Models.NativeDb.NativeDb nativeDb;
            using (WebClient webClient = new WebClient())
            {
                string nativesJson = webClient.DownloadString(_nativesJsonUrl);
                string nativesHash = nativesJson.GetSha256Hash();
                nativeDb = JsonConvert.DeserializeObject<Models.NativeDb.NativeDb>(nativesJson);
                nativeDb.VersionHash = nativesHash;
            }
            return nativeDb;
        }
    }
}
