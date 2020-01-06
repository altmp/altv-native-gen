using System.Collections.Generic;
using System.IO;
using Durty.AltV.NativesTypingsGenerator.Models.Typing;
using Durty.AltV.NativesTypingsGenerator.NativeDb;
using Durty.AltV.NativesTypingsGenerator.TypingDef;

namespace Durty.AltV.NativesTypingsGenerator.Console
{
    class Program
    {
        private const string AltVNativeDbJsonSourceUrl = "https://natives.altv.mp/natives";
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
                TypeDefinition = "object"
            },
            new TypeDefType()
            {
                Name = "vectorPtr",
                TypeDefinition = "Vector3"
            }
        };

        static void Main(string[] args)
        {
            // Download latest natives from nativedb
            //System.Console.WriteLine("Downloading latest natives from AltV...");
            //NativeDbDownloader nativeDbDownloader = new NativeDbDownloader(AltVNativeDbJsonSourceUrl);
            //Models.NativeDb.NativeDb nativeDb = nativeDbDownloader.DownloadLatest();

            // Read nativedb from file
            System.Console.WriteLine("Reading natives from file...");
            string nativeDbFilePath = Path.Combine(Directory.GetCurrentDirectory(), "resources", "natives", "natives.json");
            NativeDbFileReader nativeDbFileReader = new NativeDbFileReader(nativeDbFilePath);
            Models.NativeDb.NativeDb nativeDb = nativeDbFileReader.Read();
            if (nativeDb == null)
            {
                System.Console.WriteLine("Failed to read natives from file. File doesnt exist or is invalid.");
                return;
            }

            TypeDefFromNativeDbGenerator typeDefGenerator = new TypeDefFromNativeDbGenerator(Interfaces, Types, "natives");
            typeDefGenerator.AddFunctionsFromNativeDb(nativeDb);
            TypeDef typingDefinition = typeDefGenerator.GetTypingDefinition();

            TypeDefFileGenerator typeDefFileGenerator = new TypeDefFileGenerator(typingDefinition);
            string typingFileContent = typeDefFileGenerator.Generate(true, new List<string>()
            {
                $" Natives retrieved from alt:V / NativeDB at http://natives.altv.mp/#/ - VersionHash: {nativeDb.VersionHash}"
            });

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "native-types", "natives.d.ts");
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }
            File.WriteAllText(filePath, typingFileContent);

            System.Console.WriteLine($"Done writing natives to file: {filePath}");
        }
    }
}
