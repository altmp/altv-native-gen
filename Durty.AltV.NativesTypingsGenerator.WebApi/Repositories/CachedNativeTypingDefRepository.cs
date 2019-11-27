using System.Collections.Generic;
using System.Linq;
using Durty.AltV.NativesTypingsGenerator.WebApi.Models;

namespace Durty.AltV.NativesTypingsGenerator.WebApi.Repositories
{
    public class CachedNativeTypingDefRepository
    {
        private readonly List<CachedNativeTypingDef> _cachedNativeTypingDefs;

        public CachedNativeTypingDefRepository()
        {
            _cachedNativeTypingDefs = new List<CachedNativeTypingDef>();
        }

        public List<CachedNativeTypingDef> GetAll()
        {
            return _cachedNativeTypingDefs;
        }

        public bool TryGet(string nativeDbVersionHash, bool containsDocumentation, string branch, out CachedNativeTypingDef found)
        {
            found = _cachedNativeTypingDefs.FirstOrDefault(n =>
                n.Branch == branch && n.ContainsDocumentation == containsDocumentation &&
                n.NativeDbVersionHash == nativeDbVersionHash);

            if (found != null)
            {
                found.AccessCount++;
            }
            
            return found != null;
        }

        public void Add(CachedNativeTypingDef cachedNativeTypingDef)
        {
            _cachedNativeTypingDefs.Add(cachedNativeTypingDef);
        }

        public void Clear()
        {
            _cachedNativeTypingDefs.Clear();
        }
    }
}
