using System;
using System.Collections.Generic;
using Durty.AltV.NativesTypingsGenerator.NativeDb;
using Durty.AltV.NativesTypingsGenerator.WebApi.Repositories;

namespace Durty.AltV.NativesTypingsGenerator.WebApi.Services
{
    public class NativeDbCacheService
    {
        private readonly NativeDbDownloader _nativeDbDownloader;
        private readonly Dictionary<DateTime, NativesTypingsGenerator.Models.NativeDb.NativeDb> _cachedNativeDbs;
        private readonly TimeSpan _refreshTimeSpan;
        private readonly CachedNativeTypingDefRepository _cachedNativeTypingDefRepository;

        private NativesTypingsGenerator.Models.NativeDb.NativeDb _latestNativeDb = new NativesTypingsGenerator.Models.NativeDb.NativeDb()
        {
            VersionHash = "0"
        };
        private DateTime _lastCacheDateTime;

        public NativeDbCacheService(
            NativeDbDownloader nativeDbDownloader, 
            TimeSpan cacheRefreshTimeSpan,
            CachedNativeTypingDefRepository cachedNativeTypingDefRepository)
        {
            _cachedNativeDbs = new Dictionary<DateTime, NativesTypingsGenerator.Models.NativeDb.NativeDb>();
            _nativeDbDownloader = nativeDbDownloader;
            _refreshTimeSpan = cacheRefreshTimeSpan;
            _cachedNativeTypingDefRepository = cachedNativeTypingDefRepository;
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

        public void RefreshCache()
        {
            NativesTypingsGenerator.Models.NativeDb.NativeDb nativeDb = _nativeDbDownloader.DownloadLatest();
            if (nativeDb.VersionHash == _latestNativeDb.VersionHash) //Downloaded NativeDB has not changed
                return;

            _latestNativeDb = nativeDb;
            _lastCacheDateTime = DateTime.Now;
            _cachedNativeDbs.Add(DateTime.Now, nativeDb);
        }
    }
}
