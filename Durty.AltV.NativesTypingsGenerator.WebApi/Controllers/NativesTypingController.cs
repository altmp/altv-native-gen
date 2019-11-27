using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Durty.AltV.NativesTypingsGenerator.Models.Typing;
using Durty.AltV.NativesTypingsGenerator.NativeDb;
using Durty.AltV.NativesTypingsGenerator.TypingDef;
using Durty.AltV.NativesTypingsGenerator.WebApi.Repositories;
using Durty.AltV.NativesTypingsGenerator.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Durty.AltV.NativesTypingsGenerator.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NativesTypingController 
        : ControllerBase
    {
        private readonly ILogger<NativesTypingController> _logger;
        private readonly NativeTypingDefService _nativeTypingDefService;
        private readonly NativeDbCacheService _nativeDbCacheService;
        private readonly CachedNativeTypingDefRepository _cachedNativeTypingDefRepository;

        public NativesTypingController(
            ILogger<NativesTypingController> logger,
            NativeTypingDefService nativeTypingDefService,
            NativeDbCacheService nativeDbCacheService,
            CachedNativeTypingDefRepository cachedNativeTypingDefRepository)
        {
            _logger = logger;
            _nativeTypingDefService = nativeTypingDefService;
            _nativeDbCacheService = nativeDbCacheService;
            _cachedNativeTypingDefRepository = cachedNativeTypingDefRepository;
        }

        [HttpGet("cache/nativedb/all")]
        public IActionResult GetNativeDbCache()
        {
            return Ok(_nativeDbCacheService.GetAll().Select(n => new
            {
                Timestamp = n.Key.ToShortDateString() + " " + n.Key.ToShortTimeString(),
                NativeDbVersionHash = n.Value.VersionHash
            }));
        }

        [HttpGet("cache/nativedb/triggerrefresh")]
        public IActionResult RefreshNativeDbCache()
        {
            _nativeDbCacheService.RefreshCache();
            return Ok();
        }

        [HttpGet("cache/nativetypingdef/all")]
        public IActionResult GetNativeTypingDefCache()
        {
            return Ok(_cachedNativeTypingDefRepository.GetAll().Select(n => new
            {
                n.Branch,
                n.ContainsDocumentation,
                n.NativeDbVersionHash,
                Timestamp = n.GenerationDateTime.ToShortDateString() + " " + n.GenerationDateTime.ToShortTimeString(),
                n.AccessCount
            }));
        }

        [HttpGet("cache/nativetypingdef/triggerrefresh")]
        public IActionResult RefreshNativeTypingDefCache()
        {
            _cachedNativeTypingDefRepository.Clear();
            return Ok();
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string branch = "beta", [FromQuery] bool generateDocumentation = true)
        {
            if (branch.ToLower() != "beta")
            {
                return NotFound("Unsupported branch. (Only 'beta' branch is currently supported)");
            }

            string typingFileContent = _nativeTypingDefService.GetLatestCachedTypingFile(branch, generateDocumentation);
            Stream stream = GenerateStreamFromString(typingFileContent);

            if (stream == null)
                return NotFound();

            return new FileStreamResult(stream, "application/json")
            {
                FileDownloadName = "natives.d.ts"
            };
        }

        private Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
