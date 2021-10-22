using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AltV.NativesDb.Reader;
using AltV.NativesDb.Reader.Models.NativeDb;

namespace AltV.Natives.ChangelogGenerator.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                System.Console.WriteLine($"Please give 2 arguments [oldNativesFilePath] and [newNativesFilePath]");
                return;
            }

            var oldNativesFilePath = args[0];
            if (!File.Exists(oldNativesFilePath))
            {
                System.Console.WriteLine($"Could not find old natives file path {oldNativesFilePath}");
                return;
            }

            var newNativesFilePath = args[1];
            if (!File.Exists(newNativesFilePath))
            {
                System.Console.WriteLine($"Could not find new natives file path {newNativesFilePath}");
                return;
            }

            FixNativeDbFileNativeTypes(oldNativesFilePath);
            NativeDbFileReader oldNativeDbFileReader = new NativeDbFileReader(oldNativesFilePath);
            NativeDb oldNativeDb = oldNativeDbFileReader.Read();
            if (oldNativeDb == null)
            {
                System.Console.WriteLine("Failed to read old nativesdb file. File is invalid.");
                return;
            }

            FixNativeDbFileNativeTypes(newNativesFilePath);
            NativeDbFileReader newNativeDbFileReader = new NativeDbFileReader(newNativesFilePath);
            NativeDb newNativeDb = newNativeDbFileReader.Read();
            if (newNativeDb == null)
            {
                System.Console.WriteLine("Failed to read new nativesdb file. File is invalid.");
                return;
            }
            
            WriteNativeDeprecationChangelog(oldNativeDb, Path.Combine(Directory.GetCurrentDirectory(), "previouslyDeprecatedNativeNames.txt"));
            WriteNativeDeprecationChangelog(newNativeDb, Path.Combine(Directory.GetCurrentDirectory(), "newDeprecatedNativeNames.txt"));
            WriteNewNativesChangelog(newNativeDb, 2372, Path.Combine(Directory.GetCurrentDirectory(), "newNatives.txt"));
            System.Console.WriteLine("Finished generating changelog files.");
        }
        
        private static List<Native> GetNativesIntroducedWithBuild(NativeDb nativeDb, long build)
        {
            List<Native> nativesWithDeprecatedNames = nativeDb.AllNatives.Where(native => native.BuildNativeWasFound == build).ToList();
            return nativesWithDeprecatedNames;
        }

        private static List<Native> GetNativesWithDeprecatedNames(NativeDb nativeDb)
        {
            List<Native> nativesWithDeprecatedNames = nativeDb.AllNatives.Where(native => native.OldNames.Count != 0).ToList();
            return nativesWithDeprecatedNames;
        }
        
        private static void WriteNewNativesChangelog(NativeDb nativeDb, long build, string targetFilePath)
        {
            var deprecatedNativeNames = GetNativesIntroducedWithBuild(nativeDb, build);
            File.WriteAllLines(targetFilePath, deprecatedNativeNames.Select(x => $"{x.AltFunctionName}"));
        }

        private static void WriteNativeDeprecationChangelog(NativeDb nativeDb, string targetFilePath)
        {
            var deprecatedNativeNames = GetNativesWithDeprecatedNames(nativeDb).Select(n => new
            {
                NewName = n.AltFunctionName,
                OldName = n.OldNames.First()
            }).ToList();
            File.WriteAllLines(targetFilePath, deprecatedNativeNames.Select(x => $"{x.OldName} => {x.NewName}"));
        }

        private static void FixNativeDbFileNativeTypes(string filePath)
        {
            Dictionary<string, string> replaceTypes = new Dictionary<string, string>()
            {
                { "const char*", "string" },
                { "char*", "string" },
                { "Any*", "Any" },
                { "BOOL", "boolean" },
                { "boolean*", "boolean" },
                { "Vector3*", "Vector3" },
                { "Hash*", "Hash" },
                { "int*", "int" },
                { "float*", "float" },
                { "Ped*", "Ped" },
                { "Blip*", "Blip" },
                { "Vehicle*", "Vehicle" },
                { "Object*", "Object" },
                { "Entity*", "Entity" },
                { "ScrHandle*", "ScrHandle" },
            };
            var fileContent = File.ReadAllText(filePath);
            fileContent = replaceTypes.Aggregate(fileContent, (current, replaceType) => current.Replace(replaceType.Key, replaceType.Value));
            File.WriteAllText(filePath, fileContent);
        }
    }
}
