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
            string streamingAssetsDir = Path.Combine(assetsRoot, "StreamingAssets");
            string targetPath = Path.Combine(streamingAssetsDir, "path_data.json");

            if (!File.Exists(sourcePath))
            {
                Debug.LogError($"Could not find source JSON at: {sourcePath}");
                return;
            }

            // image_file을 JSON에서 읽어 동적으로 파일명 결정
            string imageFileName = "img.jpg";
            string jsonText = File.ReadAllText(sourcePath);
            int imageFileIdx = jsonText.IndexOf("\"image_file\"");
            if (imageFileIdx >= 0)
            {
                int colonIdx = jsonText.IndexOf(':', imageFileIdx);
                int quoteStart = jsonText.IndexOf('"', colonIdx + 1);
                int quoteEnd = jsonText.IndexOf('"', quoteStart + 1);
                if (quoteStart >= 0 && quoteEnd > quoteStart)
                    imageFileName = jsonText.Substring(quoteStart + 1, quoteEnd - quoteStart - 1);
            }

            string sourceImagePath = Path.Combine(repoRoot, imageFileName);
            string targetImagePath = Path.Combine(streamingAssetsDir, imageFileName);

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
