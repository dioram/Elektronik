using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Elektronik.Editor
{
    public class AutomateProtocCompile : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            string rootDir = Path.GetDirectoryName(Application.dataPath);
            string generatedFilesDir = Path.Combine(Application.dataPath, "generatedFiles");
            string scriptPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("AutomateProtocCompile")[0]);
            string scriptDir = Path.Combine(rootDir, Path.GetDirectoryName(scriptPath));
            string includeDir = Path.Combine(scriptDir, "include\\");
#if UNITY_EDITOR_WIN
            string protocPath = Path.Combine(scriptDir, "bin\\windows-x64\\protoc.exe");
            string grpcPluginPath = Path.Combine(scriptDir, "bin\\windows-x64\\grpc_csharp_plugin.exe");
#else
#error "windows only support"
#endif

            System.Collections.Generic.List<Tuple</*input file*/string, /*output dir*/string>> changedProtoFiles = importedAssets
                .AsParallel()
                .Where(asset => Path.GetExtension(asset) == ".proto")
                .Select(asset => 
                    Tuple.Create(
                        Path.Combine(rootDir, asset).Replace("/", "\\"), 
                        Path.Combine(generatedFilesDir, Path.GetDirectoryName(asset.Replace("Assets/", "")))
                        ))
                .Where(t => !t.Item1.StartsWith(includeDir))
                .ToList();

            UnityEngine.Debug.Log($"Protobuf generation started for {changedProtoFiles.Count} files.");
            var options = new StringBuilder();
            
            foreach (var protoFilePath in changedProtoFiles)
            {
                if (!Directory.Exists(protoFilePath.Item2))
                {
                    Directory.CreateDirectory(protoFilePath.Item2);
                }
                string protoFileDir = Path.GetDirectoryName(protoFilePath.Item1);
                string args = $" -I{includeDir} -I{protoFileDir} --csharp_out={protoFilePath.Item2} --grpc_out={protoFilePath.Item2} --plugin=protoc-gen-grpc={grpcPluginPath} {protoFilePath.Item1}";
                var procInfo = new ProcessStartInfo()
                {
                    FileName = protocPath, 
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                };
                var proc = new Process() { 
                    StartInfo = procInfo
                };
                proc.Start();
                proc.WaitForExit();
                UnityEngine.Debug.Log(args);
                UnityEngine.Debug.Log(proc.StandardOutput.ReadToEnd());
                UnityEngine.Debug.Log(proc.StandardError.ReadToEnd());
                UnityEngine.Debug.Log($"protoc exit code: {proc.ExitCode}");
            }
        }
    }
}
