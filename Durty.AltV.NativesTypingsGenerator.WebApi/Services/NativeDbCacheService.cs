using System;
using System.Collections.Generic;
using Durty.AltV.NativesTypingsGenerator.NativeDb;

namespace Durty.AltV.NativesTypingsGenerator.WebApi.Services
{
    public class NativeDbCacheService
    {
        private readonly NativeDbDownloader _nativeDbDownloader;
        private readonly Dictionary<DateTime, Models.NativeDb.NativeDb> _cachedNativeDbs;
        private readonly TimeSpan _refreshTimeSpan;

        private Models.NativeDb.NativeDb _latestNativeDb = new Models.NativeDb.NativeDb()
        {
            VersionHash = "0"
        };
        private DateTime _lastCacheDateTime;

        public NativeDbCacheService(NativeDbDownloader nativeDbDownloader, TimeSpan cacheRefreshTimeSpan)
        {
            _cachedNativeDbs = new Dictionary<DateTime, Models.NativeDb.NativeDb>();
            _nativeDbDownloader = nativeDbDownloader;
            _refreshTimeSpan = cacheRefreshTimeSpan;
        }

        public Dictionary<DateTime, Models.NativeDb.NativeDb> GetAll()
        {
            return _cachedNativeDbs;
        }

        public Models.NativeDb.NativeDb GetLatest()
        {
            if (_lastCacheDateTime + _refreshTimeSpan < DateTime.Now)
            {
                RefreshCache();
            }
            return _latestNativeDb;
        }

        public void RefreshCache()
        {
            Models.NativeDb.NativeDb nativeDb = _nativeDbDownloader.DownloadLatest();
            if (nativeDb.VersionHash == _latestNativeDb.VersionHash)
            {
                return;
            }
            _latestNativeDb = nativeDb;
            _lastCacheDateTime = DateTime.Now;
            _cachedNativeDbs.Add(DateTime.Now, nativeDb);
        }
    }
}
