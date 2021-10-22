using System;
using System.IO;
using AltV.NativesDb.Reader.Extensions;
using Newtonsoft.Json;

namespace AltV.NativesDb.Reader
{
    public class NativeDbFileReader
    {
        private readonly string _filePath;

        public NativeDbFileReader(string filePath)
        {
            _filePath = filePath;
        }

        public Models.NativeDb.NativeDb Read()
        {
            if (!File.Exists(_filePath))
                return null;

            string nativesJson = File.ReadAllText(_filePath);
            string nativesHash = nativesJson.GetSha256Hash();
            try
            {
                Models.NativeDb.NativeDb nativeDb = JsonConvert.DeserializeObject<Models.NativeDb.NativeDb>(nativesJson);
                nativeDb.VersionHash = nativesHash;
                return nativeDb;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
