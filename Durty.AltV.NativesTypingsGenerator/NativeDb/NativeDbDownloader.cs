using System.Net;
using Durty.AltV.NativesTypingsGenerator.Extensions;
using Newtonsoft.Json;

namespace Durty.AltV.NativesTypingsGenerator.NativeDb
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
