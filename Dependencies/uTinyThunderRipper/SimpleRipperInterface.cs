#if DEBUG
#define DEBUG_PROGRAM
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using uTinyRipper;
using uTinyRipper.Converters;
using Version = uTinyRipper.Version;
using Logger = uTinyRipper.Logger;
using LogType = uTinyRipper.LogType;
using UnityEngine;
using uTinyRipperGUI.Exporters;
using ILogger = uTinyRipper.ILogger;

namespace ThunderKit.uTinyRipper
{
    public class SimpleRipperInterface : ScriptableObject
    {
        internal static void Main(string[] args)
        {
            Logger.Instance = ConsoleLogger.Instance;

            if (args.Length == 0)
            {
                Console.WriteLine("No arguments");
                Console.ReadKey();
                return;
            }

            foreach (string arg in args)
            {
                if (arg.StartsWith("--Types=")) continue;
                if (MultiFileStream.Exists(arg))
                {
                    continue;
                }
                if (DirectoryUtils.Exists(arg))
                {
                    continue;
                }
                Console.WriteLine(MultiFileStream.IsMultiFile(arg) ?
                    $"File '{arg}' doesn't have all parts for combining" :
                    $"Neither file nor directory with path '{arg}' exists");
                Console.ReadKey();
                return;
            }

            var classes = args
                .FirstOrDefault(arg => arg.StartsWith("--Types="))
                .Substring("--Types=".Length)
                .Split(',')
                .Select(cls =>
                {
                    if (Enum.TryParse<ClassIDType>(cls, out var result))
                        return result;
                    return (ClassIDType)(-1);
                })
                .Where(v => ((int)(v)) >= 0)
                .ToList();

            classes.Add(ClassIDType.GraphicsSettings);
            classes.Add(ClassIDType.NavMeshSettings);
            classes.Add(ClassIDType.PlayerSettings);
            classes.Add(ClassIDType.QualitySettings);
            classes.Add(ClassIDType.RenderSettings);
            classes.Add(ClassIDType.SceneSettings);
            classes.Add(ClassIDType.EditorSettings);
            classes.Add(ClassIDType.EditorBuildSettings);
            classes.Add(ClassIDType.EditorUserBuildSettings);
            classes.Add(ClassIDType.LightmapSettings);
            classes.Add(ClassIDType.Physics2DSettings);
            classes.Clear();

            PrepareExportDirectory(Path.Combine("Ripped", "Assets"));
            PrepareExportDirectory(Path.Combine("Ripped", "ProjectSettings"));

            SimpleRipperInterface program = new SimpleRipperInterface();
            program.Load(args[0], classes, Platform.NoTarget, TransferInstructionFlags.NoTransferInstructionFlags);
            Console.ReadKey();
        }

        public void Load(string gameDir, IEnumerable<ClassIDType> classes, Platform platform, TransferInstructionFlags transferInstructionFlags, ILogger logger = null)
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                Logger.Instance = logger;
                var filename = Path.GetFileName(gameDir);
                var playerInfo = FileVersionInfo.GetVersionInfo(gameDir);
                var unityVersion = playerInfo.ProductVersion;
                unityVersion = unityVersion.Substring(0, unityVersion.LastIndexOf('.'));

                var gameStructure = GameStructure.Load(new[] { Path.GetDirectoryName(gameDir) });
                var fileCollection = gameStructure.FileCollection;

                Logger.Log(LogType.Info, LogCategory.General, "Loading Class Types export configuration");
                SetUpExporter(fileCollection.Exporter);

                Dictionary<string, Guid> AssemblyHash = new Dictionary<string, Guid>();
                Dictionary<string, long> ScriptId = new Dictionary<string, long>();

                var unityPlayerVersion = new System.Version(unityVersion);
                var version = new Version(unityPlayerVersion.Major, unityPlayerVersion.Minor, unityPlayerVersion.Revision, VersionType.Final, 3);

                var options = new ExportOptions(
                    version,
                    platform,
                    transferInstructionFlags
                );
                var serializedFiles = fileCollection.GameFiles.Values;

                string path = Directory.GetCurrentDirectory();
                fileCollection.Exporter.Export(path, fileCollection, serializedFiles, options);

                Logger.Log(LogType.Info, LogCategory.General, "Finished");
            }
            catch (Exception ex)
            {
                Logger.Log(LogType.Error, LogCategory.General, ex.ToString());
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        private void SetUpExporter(ProjectExporter exporter)
        {
            TextureAssetExporter textureExporter = new TextureAssetExporter();
            
            exporter.OverrideExporter(ClassIDType.Texture2D, textureExporter);
            exporter.OverrideExporter(ClassIDType.Cubemap, textureExporter);
            exporter.OverrideExporter(ClassIDType.Sprite, textureExporter);
            
            exporter.OverrideYamlExporter(ClassIDType.Shader);
            exporter.OverrideExporter(ClassIDType.MonoScript, new NoScriptExporter());
            exporter.OverrideExporter(ClassIDType.AudioClip, new AudioAssetExporter());
            exporter.OverrideExporter(ClassIDType.TextAsset, new TextAssetExporter());
            exporter.OverrideExporter(ClassIDType.Font, new FontAssetExporter());
            exporter.OverrideExporter(ClassIDType.MovieTexture, new MovieTextureAssetExporter());
        }

        private static void PrepareExportDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                //DirectoryUtils.Delete(path, true);
            }
        }
    }
}
