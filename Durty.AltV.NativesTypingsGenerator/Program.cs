using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Durty.AltV.NativesTypingsGenerator.Converters;
using Durty.AltV.NativesTypingsGenerator.Models;
using Durty.AltV.NativesTypingsGenerator.Models.NativeDb;
using Durty.AltV.NativesTypingsGenerator.Models.Typing;
using Newtonsoft.Json;

namespace Durty.AltV.NativesTypingsGenerator
{
    class Program
    {
        private const string AltVNativeDbJsonSourceUrl = "https://natives.altv.mp/natives";

        static void Main(string[] args)
        {
            Console.WriteLine("Downloading latest natives from AltV...");
            NativeDb nativeDb;
            using (WebClient webClient = new WebClient())
            {
                string nativesJson = webClient.DownloadString(AltVNativeDbJsonSourceUrl);
                Console.WriteLine($"Done. Size = {nativesJson.Length}");
                nativeDb = JsonConvert.DeserializeObject<NativeDb>(nativesJson);
            }

            TypeDefFile nativesTypeDefFile = new TypeDefFile()
            {
                Interfaces = new List<TypeDefInterface>()
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
                },
                Types = new List<TypeDefType>()
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
                },
                Functions = new List<TypeDefFunction>()
            };

            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Graphics));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.System));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.App));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Audio));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Brain));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Cam));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Clock));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Cutscene));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Datafile));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Decorator));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Dlc));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Entity));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Event));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Files));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Fire));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Graphics));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Hud));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Interior));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Itemset));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Loadingscreen));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Localization));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Misc));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Mobile));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Money));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Netshopping));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Network));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Object));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Pad));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Pathfind));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Ped));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Physics));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Player));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Recording));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Replay));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Script));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Shapetest));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Socialclub));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Stats));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Streaming));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Task));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Vehicle));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Water));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Weapon));
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Zone));
            
            TypeDefFileWriter typeDefFileWriter = new TypeDefFileWriter(nativesTypeDefFile);
            typeDefFileWriter.Write("H:\\Games\\AltV-beta\\natives.d.ts");

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        public static List<TypeDefFunction> GetFunctionsFromNativeGroup(Dictionary<string, Native> nativeGroup)
        {
            Console.WriteLine($"Parsing {nativeGroup.Values.Count} natives from group...");

            NativeTypeToTypingConverter nativeTypeToTypingConverter = new NativeTypeToTypingConverter();
            NativeReturnTypeToTypingConverter nativeReturnTypeToTypingConverter = new NativeReturnTypeToTypingConverter();

            var functions = new List<TypeDefFunction>();
            foreach (Native native in nativeGroup.Values)
            {
                if (native.AltFunctionName != string.Empty)
                {
                    Console.WriteLine($"Found AltV Native Name: {native.AltFunctionName}");
                    TypeDefFunction function = new TypeDefFunction()
                    {
                        Name = native.AltFunctionName,
                        Parameters = native.Parameters.Select(p => new TypeDefFunctionParameter()
                        {
                            Name = p.Name,
                            Type = nativeTypeToTypingConverter.Convert(native, p.NativeParamType)
                        }).ToList(),
                        ReturnType = nativeReturnTypeToTypingConverter.Convert(native, native.ResultTypes)
                    };
                    functions.Add(function);
                }
            }

            return functions;
        }
    }
}
