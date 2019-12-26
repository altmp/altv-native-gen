using System;
using System.Collections.Generic;
using Durty.AltV.NativesTypingsGenerator.NativeDb;

namespace Durty.AltV.NativesTypingsGenerator.WebApi.Services
{
    public class NativeDbCacheService
    {
        private readonly NativeDbDownloader _nativeDbDownloader;
        private readonly Dictionary<DateTime, NativesTypingsGenerator.Models.NativeDb.NativeDb> _cachedNativeDbs;
        private readonly TimeSpan _refreshTimeSpan;

        private NativesTypingsGenerator.Models.NativeDb.NativeDb _latestNativeDb = new NativesTypingsGenerator.Models.NativeDb.NativeDb()
        {
            VersionHash = "0"
        };
        private DateTime _lastCacheDateTime = DateTime.MinValue;

        public NativeDbCacheService(
            NativeDbDownloader nativeDbDownloader, 
            TimeSpan cacheRefreshTimeSpan)
        {
            _cachedNativeDbs = new Dictionary<DateTime, NativesTypingsGenerator.Models.NativeDb.NativeDb>();
            _nativeDbDownloader = nativeDbDownloader;
            _refreshTimeSpan = cacheRefreshTimeSpan;
        }

        public Dictionary<DateTime, NativesTypingsGenerator.Models.NativeDb.NativeDb> GetAll()
        {
            return _cachedNativeDbs;
        }

        public NativesTypingsGenerator.Models.NativeDb.NativeDb GetLatest()
        {
            if (_lastCacheDateTime + _refreshTimeSpan < DateTime.Now)
            {
                RefreshCache();
            }
            return _latestNativeDb;
        }

        public bool RefreshCache()
        {
            NativesTypingsGenerator.Models.NativeDb.NativeDb nativeDb = _nativeDbDownloader.DownloadLatest();
            if (nativeDb.VersionHash == _latestNativeDb.VersionHash) //Downloaded NativeDB has not changed
                return false;

            _latestNativeDb = nativeDb;
            _lastCacheDateTime = DateTime.Now;
            _cachedNativeDbs.Add(DateTime.Now, nativeDb);
            return true;
        }
    }
}
