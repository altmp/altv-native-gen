using System;
using System.Collections.Generic;
using Durty.AltV.NativesTypingsGenerator.Models.Typing;
using Durty.AltV.NativesTypingsGenerator.TypingDef;
using Durty.AltV.NativesTypingsGenerator.WebApi.Models;
using Durty.AltV.NativesTypingsGenerator.WebApi.Repositories;

namespace Durty.AltV.NativesTypingsGenerator.WebApi.Services
{
    public class NativeTypingDefService
    {
        private static readonly List<TypeDefInterface> Interfaces = new List<TypeDefInterface>()
        {
            new TypeDefInterface()
            {
                Name = "Vector3",
                Properties = new List<TypeDefInterfaceProperty>()
                {
                    new TypeDefInterfaceProperty()
                    {
                        Name = "x",
                        Type = "number"
                    },
                    new TypeDefInterfaceProperty()
                    {
                        Name = "y",
                        Type = "number"
                    },
                    new TypeDefInterfaceProperty()
                    {
                        Name = "z",
                        Type = "number"
                    }
                }
            }
        };
        private static readonly List<TypeDefType> Types = new List<TypeDefType>()
        {
            new TypeDefType()
            {
                Name = "MemoryBuffer",
                TargetTypeName = "object"
            },
            new TypeDefType()
            {
                Name = "vectorPtr",
                TargetTypeName = "Vector3"
            }
        };
        private readonly NativeDbCacheService _nativeDbCacheService;
        private readonly CachedNativeTypingDefRepository _cachedNativeTypingDefRepository;

        public NativeTypingDefService(
            NativeDbCacheService nativeDbCacheService,
            CachedNativeTypingDefRepository cachedNativeTypingDefRepository)
        {
            _nativeDbCacheService = nativeDbCacheService;
            _cachedNativeTypingDefRepository = cachedNativeTypingDefRepository;
        }

        public string GetLatestCachedTypingFile(string branch, bool includeDocumentation)
        {
            var latestNativeDb = _nativeDbCacheService.GetLatest();
            if (_cachedNativeTypingDefRepository.TryGet(latestNativeDb.VersionHash, includeDocumentation, branch,
                out var cachedNativeTypingDef))
            {
                return cachedNativeTypingDef.GeneratedTypingFileContent;
            }

            TypeDefFromNativeDbGenerator typeDefGenerator = new TypeDefFromNativeDbGenerator(Interfaces, Types, "natives");
            typeDefGenerator.AddFunctionsFromNativeDb(latestNativeDb);

            TypeDefFileGenerator typeDefFileGenerator = new TypeDefFileGenerator(typeDefGenerator.GetTypingDefinition(), includeDocumentation);
            string typingFileContent = typeDefFileGenerator.Generate(true, new List<string>()
            {
                $" Natives retrieved from alt:V / NativeDB at http://natives.altv.mp/#/ - VersionHash: {latestNativeDb.VersionHash}"
            });

            _cachedNativeTypingDefRepository.Add(new CachedNativeTypingDef()
            {
                ContainsDocumentation = includeDocumentation,
                NativeDbVersionHash = latestNativeDb.VersionHash,
                TypingDefinition = typeDefGenerator.GetTypingDefinition(),
                Branch = branch,
                GenerationDateTime = DateTime.Now,
                GeneratedTypingFileContent = typingFileContent
            });
            return typingFileContent;
        }
    }
}
