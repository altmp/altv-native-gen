using System;
using Durty.AltV.NativesTypingsGenerator.Models.Typing;

namespace Durty.AltV.NativesTypingsGenerator.WebApi.Models
{
    public class CachedNativeTypingDef
    {
        public string NativeDbVersionHash { get; set; }

        public DateTime GenerationDateTime { get; set; }

        public string Branch { get; set; }

        public bool ContainsDocumentation { get; set; }

        public TypeDef TypingDefinition { get; set; }

        public string GeneratedTypingFileContent { get; set; }

        public int AccessCount { get; set; }
    }
}
