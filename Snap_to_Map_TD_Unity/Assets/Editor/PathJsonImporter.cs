#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SnapToMapTD.EditorTools
{
    public static class PathJsonImporter
    {
        [MenuItem("Tools/Snap To Map/Import Map Assets from Repo Root")]
        public static void ImportPathJson()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string repoRoot = Path.GetFullPath(Path.Combine(projectRoot, ".."));
            string assetsRoot = Application.dataPath;

            string sourcePath = Path.Combine(repoRoot, "path_data.json");
            string sourceImagePath = Path.Combine(repoRoot, "img.jpg");
            string streamingAssetsDir = Path.Combine(assetsRoot, "StreamingAssets");
            string targetPath = Path.Combine(streamingAssetsDir, "path_data.json");
            string targetImagePath = Path.Combine(streamingAssetsDir, "img.jpg");

            if (!File.Exists(sourcePath))
            {
                Debug.LogError($"Could not find source JSON at: {sourcePath}");
                return;
            }

            if (!File.Exists(sourceImagePath))
            {
                Debug.LogError($"Could not find source image at: {sourceImagePath}");
                return;
            }

            Directory.CreateDirectory(streamingAssetsDir);
            File.Copy(sourcePath, targetPath, true);
            File.Copy(sourceImagePath, targetImagePath, true);
            AssetDatabase.Refresh();

            Debug.Log($"Imported path JSON to: {targetPath}");
            Debug.Log($"Imported background image to: {targetImagePath}");
        }

        [MenuItem("Tools/Snap To Map/Open StreamingAssets Folder")]
        public static void OpenStreamingAssetsFolder()
        {
            string streamingAssetsDir = Path.Combine(Application.dataPath, "StreamingAssets");
            Directory.CreateDirectory(streamingAssetsDir);
            EditorUtility.RevealInFinder(streamingAssetsDir);
        }
    }
}
#endif
