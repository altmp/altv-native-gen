using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                Functions = new List<TypeDefFunction>()
            };
            
            nativesTypeDefFile.Functions.AddRange(GetFunctionsFromNativeGroup(nativeDb.Graphics));


            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        public static List<TypeDefFunction> GetFunctionsFromNativeGroup(Dictionary<string, Native> nativeGroup)
        {
            Console.WriteLine($"Parsing {nativeGroup.Values.Count} natives from group...");

            var functions = new List<TypeDefFunction>();
            foreach (Native native in nativeGroup.Values)
            {
                if (native.AltName != string.Empty)
                {
                    Console.WriteLine($"Found AltV Native Name: {native.AltName}");
                    TypeDefFunction function = new TypeDefFunction()
                    {
                        Name = native.AltName,
                        Parameters = native.Params.Select(p => new TypeDefFunctionParameter()
                        {
                            Name = p.Name,
                            Type = p.NativeParamType.ToString()
                        }).ToList(),
                        ReturnType = native.Results
                    };
                    functions.Add(function);
                }
            }

            return functions;
        }
    }
}
