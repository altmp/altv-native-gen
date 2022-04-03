using System;
using System.Collections.Generic;
using System.Linq;
using AltV.NativesDb.Reader.Models.NativeDb;
using Durty.AltV.NativesTypingsGenerator.Converters;
using Durty.AltV.NativesTypingsGenerator.Extensions;
using Durty.AltV.NativesTypingsGenerator.Models.Typing;

namespace Durty.AltV.NativesTypingsGenerator.TypingDef
{
    public class TypeDefFromNativeDbGenerator
    {
        private readonly bool _tryResolveDocs;
        private readonly TypeDef _typeDefinition;

        public TypeDefFromNativeDbGenerator(
            string nativesModuleName,
            List<TypeDefInterface> interfaces = null,
            List<TypeDefType> types = null,
            bool resolveDocs = true)
        {
            _tryResolveDocs = resolveDocs;
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

        public void AddFunctionsFromNativeDb(NativeDb nativeDb)
        {
            TypeDefModule nativesModule = _typeDefinition.Modules.First(m => m.Name == "natives");
            nativesModule.Functions.AddRange(GetFunctionsFromNatives(nativeDb.AllNatives));
        }

        private List<TypeDefFunction> GetFunctionsFromNatives(List<Native> natives)
        {
            NativeTypeToTypingConverter nativeTypeToTypingConverter = new NativeTypeToTypingConverter();
            NativeReturnTypeToTypingConverter nativeReturnTypeToTypingConverter = new NativeReturnTypeToTypingConverter();

            //TODO: Add possibility to override certain properties of retrieved native function (for example override param type etc.)
            List<TypeDefFunction> functions = new List<TypeDefFunction>();
            foreach (Native native in natives.Where(native =>
                native.AltFunctionName != string.Empty && native.Hashes != null && native.Hashes.Count != 0))
            {
                int lastVar = native.Parameters.ToList().FindLastIndex(elem => !elem.IsReference);

                // Prepare for docs
                List<string> nativeCommentLines = native.Comment.Split("\n").ToList();
                
                // Remove blank lines
                nativeCommentLines.RemoveAll(l => l.Trim().Length == 0);

                // Remove * at line beginnings and /* + */ comments
                for (var i = 0; i < nativeCommentLines.Count; i++)
                {
                    if (nativeCommentLines[i].StartsWith("* "))
                    {
                        nativeCommentLines[i] = nativeCommentLines[i].ReplaceFirst("* ", string.Empty);
                    }
                    nativeCommentLines[i] = nativeCommentLines[i].Replace("/*", "").Replace("*/", "");
                }

                List<string> commentLinesToRemove = new List<string>();
                TypeDefFunction function = new TypeDefFunction()
                {
                    Name = native.AltFunctionName,
                    LatestHash = native.Hashes[native.Hashes.Keys.Max(int.Parse).ToString()],
                    Parameters = native.Parameters.Select((p, i) => new TypeDefFunctionParameter()
                    {
                        Name = p.Name,
                        IsReference = p.IsReference,
                        NativeType = p.NativeParamType,
                        IsLastReference = i > lastVar,
                        Type = nativeTypeToTypingConverter.Convert(native, p.NativeParamType, native.Parameters.Length > 1 && p.IsReference && i < lastVar),
                        Description = _tryResolveDocs ? GetPossibleParameterDescriptionFromComment(i, p.Name, nativeCommentLines, ref commentLinesToRemove) : string.Empty
                    }).ToList(),
                    ReturnType = new TypeDefFunctionReturnType()
                    {
                        NativeType = native.ResultTypes,
                        Name = nativeReturnTypeToTypingConverter.Convert(native, native.ResultTypes),
                    }
                };
                
                // Try resolving return type description
                string foundReturnTypeDescription = string.Empty;
                if (_tryResolveDocs)
                {
                    List<string> returnCommentKeywords = new List<string>()
                    {
                        "return",
                        "returns"
                    };
                    var foundReturnTypeComment = nativeCommentLines.FirstOrDefault(l => returnCommentKeywords.Any(k => l.ToLower().Contains($" {k} ")));
                    if (foundReturnTypeComment != null)
                    {
                        foundReturnTypeDescription = foundReturnTypeComment;
                        commentLinesToRemove.Add(foundReturnTypeComment);
                    }
                }
                function.ReturnType.Description = foundReturnTypeDescription;
                
                // Remove comment lines used in parameter descriptions
                commentLinesToRemove.ForEach(l => nativeCommentLines.Remove(l));
                
                // Set native summary
                if (nativeCommentLines.Count > 10) //If native comment is really huge, cut & add NativeDB reference link to read
                {
                    nativeCommentLines = nativeCommentLines.Take(9).ToList();
                    nativeCommentLines.Add($"See NativeDB for reference: http://natives.altv.mp/#/{native.Hashes.First().Value}");
                }
                function.Description = string.Join("\n", nativeCommentLines);

                functions.Add(function);
            }

            return functions;
        }

        //TODO: in the future include Levenshtein Distance algorythm for better parameter name matching https://stackoverflow.com/a/2344347
        private string GetPossibleParameterDescriptionFromComment(int parameterIndex, string parameterName, List<string> commentLines, ref List<string> commentLinesToRemove)
        {
            string descriptionMatch = string.Empty;
            foreach (var similarParameterName in GetSimilarParameterNames(parameterIndex, parameterName))
            {
                foreach (string commentLine in commentLines)
                {
                    // Single parameter matching
                    if (TryGetSingleParameterMatch(commentLine, similarParameterName, out descriptionMatch))
                    {
                        commentLinesToRemove.Add(commentLine);
                        break;
                    }

                    // Parameter group matching
                    if (TryGetGroupParameterMatch(commentLine, similarParameterName, out descriptionMatch))
                    {
                        commentLinesToRemove.Add(commentLine);
                        break;
                    }

                    // Parameter index matching
                    if (TryGetSingleParameterMatch(commentLine, $"p{parameterIndex}", out descriptionMatch))
                    {
                        commentLinesToRemove.Add(commentLine);
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(descriptionMatch))
                {
                    break;
                }
            }
            return descriptionMatch;
        }

        private static bool TryGetGroupParameterMatch(string commentLine, string similarParameterName, out string descriptionMatch)
        {
            descriptionMatch = null;
            List<string> groupParameterMatchings = new List<string>()
            {
                ",",
                "/"
            };
            List<char> possibleCommentSeperators = new List<char>()
            {
                ':',
                '=',
                '-'
            };

            foreach (var groupParameterMatching in groupParameterMatchings)
            {
                if (!commentLine.Contains($"{similarParameterName}{groupParameterMatching}", StringComparison.OrdinalIgnoreCase))
                    continue;

                char? firstOccuredSeperator = null;
                int firstOccuredSeperatorIndex = int.MaxValue;
                foreach (var possibleCommentSeperator in possibleCommentSeperators)
                {
                    var commentSeperatorIndex = commentLine.IndexOf(possibleCommentSeperator);
                    if (commentSeperatorIndex != -1 && (firstOccuredSeperator == null || commentSeperatorIndex < firstOccuredSeperatorIndex))
                    {
                        firstOccuredSeperator = possibleCommentSeperator;
                        firstOccuredSeperatorIndex = commentSeperatorIndex;
                    }
                }

                if (firstOccuredSeperator == null) 
                    continue;

                var descriptionParts = commentLine.Split(firstOccuredSeperator.Value);
                if (descriptionParts.Length > 1)
                {
                    descriptionMatch = descriptionParts[1].Trim();
                    return true;
                }
            }
            return false;
        }

        private static bool TryGetSingleParameterMatch(string commentLine, string similarParameterName, out string descriptionMatch)
        {
            descriptionMatch = null;
            List<Tuple<string, string>> singleParameterMatchings = new List<Tuple<string, string>>() // Prefix, Suffix
            {
                new Tuple<string, string>("", " "),
                new Tuple<string, string>("", ":"),
                new Tuple<string, string>("-", ":"),
                new Tuple<string, string>("", " = "),
                new Tuple<string, string>("", " - "),
                new Tuple<string, string>("", " ,"),
                new Tuple<string, string>("", "= "),
            };
            foreach (var singleParameterMatching in singleParameterMatchings)
            {
                if (!commentLine.StartsWith($"{singleParameterMatching.Item1}{similarParameterName}{singleParameterMatching.Item2}", StringComparison.OrdinalIgnoreCase))
                    continue;

                descriptionMatch = commentLine.ReplaceFirst($"{singleParameterMatching.Item1}{similarParameterName}{singleParameterMatching.Item2}", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();
                return true;
            }
            return false;
        }

        private List<string> GetSimilarParameterNames(int parameterIndex, string parameterName)
        {
            var similarParameterNameGroups = new List<List<string>>()
            {
                new List<string>()
                {
                    "r",
                    "red",
                    "colorr",
                    "color"
                },
                new List<string>()
                {
                    "g",
                    "green",
                    "colorg",
                    "color"
                },
                new List<string>()
                {
                    "b",
                    "blue",
                    "colorb",
                    "color"
                },
                new List<string>()
                {
                    "a",
                    "alpha"
                },
                new List<string>()
                {
                    $"x{parameterIndex}",
                    "x",
                },
                new List<string>()
                {
                    $"y{parameterIndex}",
                    "y",
                },
                new List<string>()
                {
                    $"z{parameterIndex}",
                    "z",
                },
                new List<string>()
                {
                    "pos",
                    $"pos{parameterIndex}",
                    "posx",
                    $"posx{parameterIndex}",
                    "posy",
                    $"posy{parameterIndex}",
                    "posz",
                    $"posz{parameterIndex}",
                },
                new List<string>()
                {
                    "dir",
                    $"dir{parameterIndex}",
                    "dirx",
                    $"dirx{parameterIndex}",
                    "diry",
                    $"diry{parameterIndex}",
                    "dirz",
                    $"dirz{parameterIndex}",
                }
            };
            foreach (var similarParameterNameGroup in similarParameterNameGroups
                .Where(similarParameterNameGroup => similarParameterNameGroup.Contains(parameterName.ToLower())))
            {
                return similarParameterNameGroup;
            }
            return new List<string>()
            {
                parameterName
            };
        }
    }
}
