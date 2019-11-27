using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Durty.AltV.NativesTypingsGenerator.Models.Typing;
using Durty.AltV.NativesTypingsGenerator.NativeDb;
using Durty.AltV.NativesTypingsGenerator.TypingDef;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Durty.AltV.NativesTypingsGenerator.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NativesTypingController 
        : ControllerBase
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

        private readonly ILogger<NativesTypingController> _logger;
        private readonly NativeDbDownloader _nativeDbDownloader;
        private readonly NativeDbCacheService _nativeDbCacheService;

        public NativesTypingController(
            ILogger<NativesTypingController> logger, 
            NativeDbDownloader nativeDbDownloader, 
            NativeDbCacheService nativeDbCacheService)
        {
            _logger = logger;
            _nativeDbDownloader = nativeDbDownloader;
            _nativeDbCacheService = nativeDbCacheService;
        }

        [HttpGet("cache/all")]
        public IActionResult GetCache()
        {
            return Ok(_nativeDbCacheService.GetAll().Select(n => new
            {
                Timestamp = n.Key.ToShortDateString() + " " + n.Key.ToShortTimeString(),
                NativeDbVersionHash = n.Value.VersionHash
            }));
        }

        [HttpGet("cache/triggerrefresh")]
        public IActionResult RefreshCache()
        {
            _nativeDbCacheService.RefreshCache();
            return Ok();
        }

        [HttpGet("latest")]
        public IActionResult GetLatest([FromQuery] string branch = "beta")
        {
            if (branch.ToLower() != "beta")
            {
                return NotFound("Unsupported branch. (Only 'beta' branch is currently supported)");
            }
            Models.NativeDb.NativeDb nativeDb = _nativeDbDownloader.DownloadLatest();
            Stream stream = GetNativeTypeDefContent(nativeDb);
            
            if (stream == null)
                return NotFound();

            return new FileStreamResult(stream, "application/json")
            {
                FileDownloadName = "natives.d.ts"
            };
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string branch = "beta")
        {
            if (branch.ToLower() != "beta")
            {
                return NotFound("Unsupported branch. (Only 'beta' branch is currently supported)");
            }
            Models.NativeDb.NativeDb nativeDb = _nativeDbCacheService.GetLatest();
            Stream stream = GetNativeTypeDefContent(nativeDb);

            if (stream == null)
                return NotFound();

            return new FileStreamResult(stream, "application/json")
            {
                FileDownloadName = "natives.d.ts"
            };
        }

        private Stream GetNativeTypeDefContent(Models.NativeDb.NativeDb nativeDb)
        {
            TypeDefFromNativeDbGenerator typeDefGenerator = new TypeDefFromNativeDbGenerator(Interfaces, Types, "natives");

            typeDefGenerator.AddFunctionsFromNativeDb(nativeDb);
            string typingFileContent = typeDefGenerator.GetTypingDefinition();
            Stream stream = GenerateStreamFromString(typingFileContent);
            return stream;
        }

        private Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
