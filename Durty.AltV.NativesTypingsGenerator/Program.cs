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
                Modules = new List<TypeDefModule>()
                {
                    new TypeDefModule()
                    {
                        Name = "natives",
                        Functions = new List<TypeDefFunction>()
                    }
                }
            };

            TypeDefModule nativesModule = nativesTypeDefFile.Modules.First(m => m.Name == "natives");

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
            nativesModule.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Graphics));
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

            List<TypeDefFunction> functions = new List<TypeDefFunction>();
            foreach (Native native in nativeGroup.Values.Where(native => native.AltFunctionName != string.Empty))
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

            return functions;
        }
    }
}
