using System;
using System.Net;
using System.Security.Cryptography;
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
                string nativesHash = GetSha256Hash(nativesJson);
                nativeDb = JsonConvert.DeserializeObject<Models.NativeDb.NativeDb>(nativesJson);
                nativeDb.VersionHash = nativesHash;
            }
            return nativeDb;
        }

        private string GetSha256Hash(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            using var sha = new SHA256Managed();
            byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] hash = sha.ComputeHash(textData);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }
    }
}
