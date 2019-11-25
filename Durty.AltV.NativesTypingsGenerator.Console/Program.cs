using System.Collections.Generic;
using Durty.AltV.NativesTypingsGenerator.Models.Typing;
using Durty.AltV.NativesTypingsGenerator.NativeDb;
using Durty.AltV.NativesTypingsGenerator.TypingDef;

namespace Durty.AltV.NativesTypingsGenerator.Console
{
    class Program
    {
        private const string AltVNativeDbJsonSourceUrl = "https://natives.altv.mp/natives";

        /* TODO
         - Cache AltV NativeDB file, hash content ?
         - Create WebApp & Endpoint to get latest / up2date typings
        */
        static void Main(string[] args)
        {
            System.Console.WriteLine("Downloading latest natives from AltV...");
            NativeDbDownloader nativeDbDownloader = new NativeDbDownloader(AltVNativeDbJsonSourceUrl);
            Models.NativeDb.NativeDb nativeDb = nativeDbDownloader.DownloadLatest();

            TypeDefFileFromNativeDbGenerator typeDefGenerator = new TypeDefFileFromNativeDbGenerator(new List<TypeDefInterface>()
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
            }, new List<TypeDefType>()
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
            }, "natives");

            typeDefGenerator.AddFunctionsFromNativeDb(nativeDb);
            string typingFileContent = typeDefGenerator.GetTypingFile();

            System.Console.WriteLine("Press any key to exit");
            System.Console.ReadKey();
        }
    }
}
