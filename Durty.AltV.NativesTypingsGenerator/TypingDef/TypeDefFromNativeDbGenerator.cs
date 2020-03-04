using System;
using System.Collections.Generic;
using System.Linq;
using Durty.AltV.NativesTypingsGenerator.Converters;
using Durty.AltV.NativesTypingsGenerator.Extensions;
using Durty.AltV.NativesTypingsGenerator.Models.NativeDb;
using Durty.AltV.NativesTypingsGenerator.Models.Typing;

namespace Durty.AltV.NativesTypingsGenerator.TypingDef
{
    public class TypeDefFromNativeDbGenerator
    {
        private readonly bool _tryResolveDocs;
        private readonly TypeDef _typeDefinition;

        public TypeDefFromNativeDbGenerator(
            List<TypeDefInterface> interfaces, 
            List<TypeDefType> types, 
            string nativesModuleName,
            bool tryResolveDocs = true)
        {
            _tryResolveDocs = tryResolveDocs;
            _typeDefinition = new TypeDef()
            {
                Modules = new List<TypeDefModule>()
                {
                    new TypeDefModule()
                    {
                        Name = nativesModuleName,
                        Interfaces = interfaces,
                        Types = types,
                        Functions = new List<TypeDefFunction>()
                    }
                }
            };
        }

        public TypeDef GetTypingDefinition()
        {
            return _typeDefinition;
        }

        public void AddFunctionsFromNativeDb(Models.NativeDb.NativeDb nativeDb)
        {
            TypeDefModule nativesModule = _typeDefinition.Modules.First(m => m.Name == "natives");

            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Graphics));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.System));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.App));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Audio));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Brain));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Cam));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Clock));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Cutscene));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Datafile));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Decorator));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Dlc));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Entity));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Event));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Files));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Fire));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Hud));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Interior));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Itemset));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Loadingscreen));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Localization));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Misc));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Mobile));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Money));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Netshopping));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Network));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Object));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Pad));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Pathfind));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Ped));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Physics));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Player));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Recording));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Replay));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Script));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Shapetest));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Socialclub));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Stats));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Streaming));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Task));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Vehicle));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Water));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Weapon));
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Zone));
        }

        private List<TypeDefFunction> GetFunctionsFromNativeGroup(Dictionary<string, Native> nativeGroup)
        {
            NativeTypeToTypingConverter nativeTypeToTypingConverter = new NativeTypeToTypingConverter();
            NativeReturnTypeToTypingConverter nativeReturnTypeToTypingConverter = new NativeReturnTypeToTypingConverter();

            //TODO: Add possibility to override certain properties of retrieved native function (for example override param type etc.)
            List<TypeDefFunction> functions = new List<TypeDefFunction>();
            foreach (Native native in nativeGroup.Values.Where(native => native.AltFunctionName != string.Empty && native.Hashes != null && native.Hashes.Count != 0))
            {
                //Prepare for docs
                List<string> nativeCommentLines = native.Comment.Split("\n").ToList();

                //Try resolving return type description
                string foundReturnTypeDescription = string.Empty;
                if (_tryResolveDocs)
                {
                    if (nativeCommentLines.Any(l => l.ToLower().Contains("returns")))
                    {
                        foundReturnTypeDescription = nativeCommentLines.FirstOrDefault(l => l.ToLower().Contains("returns"));
                        nativeCommentLines.Remove(foundReturnTypeDescription);
                    }
                }

                //Remove blank lines
                nativeCommentLines.RemoveAll(l => l.Trim().Length == 0);

                //Remove * at line beginnings
                for (var i = 0; i < nativeCommentLines.Count; i++)
                {
                    if (nativeCommentLines[i].StartsWith("* "))
                    {
                        nativeCommentLines[i] = nativeCommentLines[i].ReplaceFirst("* ", string.Empty);
                    }
                }

                if (nativeCommentLines.Count > 10) //If native comment is really huge, cut & add NativeDB reference link to read
                {
                    nativeCommentLines = nativeCommentLines.Take(9).ToList();
                    nativeCommentLines.Add($"See NativeDB for reference: http://natives.altv.mp/#/{native.Hashes.First().Value}");
                }
                TypeDefFunction function = new TypeDefFunction()
                {
                    Name = native.AltFunctionName,
                    Description = string.Join("\n", nativeCommentLines),
                    Parameters = native.Parameters.Select(p => new TypeDefFunctionParameter()
                    {
                        Name = p.Name,
                        NativeType = p.NativeParamType,
                        Type = nativeTypeToTypingConverter.Convert(native, p.NativeParamType, p.IsReference),
                        Description = _tryResolveDocs ? GetPossibleParameterDescriptionFromComment(p.Name, nativeCommentLines) : string.Empty
                    }).ToList(),
                    ReturnType = new TypeDefFunctionReturnType()
                    {
                        NativeType = native.ResultTypes,
                        Name = nativeReturnTypeToTypingConverter.Convert(native, native.ResultTypes),
                        Description = foundReturnTypeDescription
                    }
                };
                functions.Add(function);
            }

            return functions;
        }

        private string GetPossibleParameterDescriptionFromComment(string parameterName, List<string> commentLines)
        {
            //TODO: in the future use Levenshtein Distance algorythm for better matching https://stackoverflow.com/a/2344347
            string bestDescriptionMatch = string.Empty;
            foreach (string commentLine in commentLines)
            {
                //Worst match, replace this if we find something else
                if (commentLine.StartsWith($"{parameterName} ", StringComparison.OrdinalIgnoreCase))
                {
                    bestDescriptionMatch = commentLine.ReplaceFirst($"{parameterName} ", string.Empty);
                }
                if (commentLine.Contains($"{parameterName}: ", StringComparison.OrdinalIgnoreCase))
                {
                    bestDescriptionMatch = commentLine.ReplaceFirst($"{parameterName}: ", string.Empty);
                }
                else if (commentLine.Contains($"{parameterName} = ", StringComparison.OrdinalIgnoreCase))
                {
                    bestDescriptionMatch = commentLine.ReplaceFirst($"{parameterName} = ", string.Empty);
                }
                else if (commentLine.Contains($"{parameterName} - ", StringComparison.OrdinalIgnoreCase))
                {
                    bestDescriptionMatch = commentLine.ReplaceFirst($"{parameterName} - ", string.Empty);
                }
            }

            return bestDescriptionMatch;
        }
    }
}
