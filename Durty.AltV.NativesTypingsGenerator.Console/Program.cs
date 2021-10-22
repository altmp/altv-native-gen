using System.Collections.Generic;
using System.IO;
using AltV.NativesDb.Reader;
using AltV.NativesDb.Reader.Models.NativeDb;
using Durty.AltV.NativesTypingsGenerator.Models.Typing;
using Durty.AltV.NativesTypingsGenerator.TypingDef;

namespace Durty.AltV.NativesTypingsGenerator.Console
{
    class Program
    {
        private const string AltVNativeDbJsonSourceUrl = "https://natives.altv.mp/natives";

        static void Main(string[] args)
        {
            // Download latest natives from nativedb
            //System.Console.WriteLine("Downloading latest natives from AltV...");
            //NativeDbDownloader nativeDbDownloader = new NativeDbDownloader(AltVNativeDbJsonSourceUrl);
            //Models.NativeDb.NativeDb nativeDb = nativeDbDownloader.DownloadLatest();
            string nativeDbFilePath = Path.Combine(Directory.GetCurrentDirectory(), "resources", "natives", "natives.json");
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "index.d.ts");
            string fileIndent = null;
            bool generateDocs = true;
            List<KeyValuePair<string, string>> arguments = new List<KeyValuePair<string, string>>();
            for (var i = 0; i < args.Length; i++)
            {
                string key = args[i], keyNext = i + 1 < args.Length ? args[i + 1] : null;
                if (!key.StartsWith("--")) continue;
                if (keyNext != null && !keyNext.StartsWith("--"))
                {
                    arguments.Add(new KeyValuePair<string, string>(key, keyNext));
                }
                else
                {
                    arguments.Add(new KeyValuePair<string, string>(key, bool.TrueString));
                }
            }

            foreach (var (key, val) in arguments)
            {
                switch (key)
                {
                    case "--disableDocs":
                        generateDocs = false;
                        break;
                    case "--nativesPath" when val != null:
                        nativeDbFilePath = Path.GetFullPath(val);
                        break;
                    case "--outPath" when val != null:
                        filePath = Path.GetFullPath(val);
                        break;
                    case "--outIndent" when val != null:
                        fileIndent = val;
                        break;
                }
            }

            // Read nativedb from file
            System.Console.WriteLine("Reading natives from file...");
            NativeDbFileReader nativeDbFileReader = new NativeDbFileReader(nativeDbFilePath);
            NativeDb nativeDb = nativeDbFileReader.Read();
            if (nativeDb == null)
            {
                System.Console.WriteLine("Failed to read natives from file. File doesnt exist or is invalid.");
                return;
            }

            TypeDefFromNativeDbGenerator typeDefGenerator = new TypeDefFromNativeDbGenerator("natives");
            typeDefGenerator.AddFunctionsFromNativeDb(nativeDb);
            TypeDef typingDefinition = typeDefGenerator.GetTypingDefinition();

            TypeDefFileGenerator typeDefFileGenerator = new TypeDefFileGenerator(typingDefinition, generateDocs, fileIndent);
            typeDefFileGenerator.Generate(out string typingFileContent);

            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }
            File.WriteAllText(filePath, typingFileContent);

            System.Console.WriteLine($"Done writing natives typings to file: {filePath}");

            // TypeDefCSharpFileGenerator typeDefCSharpFileGenerator = new TypeDefCSharpFileGenerator(typingDefinition);
            // string csharpTypingFileContent = typeDefCSharpFileGenerator.Generate();
        }
    }
}
