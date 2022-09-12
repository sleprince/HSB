using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.TextureImportTools.Editor
{
    /// <summary>
    /// Set Texture Import Settings for a batch of textures. This script can be extended if you need to change additional properties.
    /// </summary>
    public class TextureImportSettingsBatch : EditorWindow
    {
        public UnityEngine.Object SpritesFolder;
        public int SpriteMode;
        public SpriteMeshType SpriteMeshType;
        public bool EnableReadWrite;
        public string TextureSize = "[Auto]";
        public FilterMode FilterMode = FilterMode.Bilinear;
        public TextureImporterCompression Compression;
        public bool CrunchedCompression;
        public int CompressionQuality = 50;

        [MenuItem("Window/Texture Import Settings/Batch")]
        public static void Init()
        {
            var window = GetWindow<TextureImportSettingsBatch>(false, "Import Settings / Batch");

            window.minSize = window.maxSize = new Vector2(300, 250);
            window.Show();
        }

        public void OnGUI()
        {
            EditorGUILayout.LabelField("Set import settings for a batch of textures", new GUIStyle(EditorStyles.label) { normal = { textColor = Color.yellow } });
            SpritesFolder = EditorGUILayout.ObjectField(new GUIContent("Textures (Folder)"), SpritesFolder, typeof(UnityEngine.Object), false);
            SpriteMode = EditorGUILayout.Popup(new GUIContent("Sprite Mode"), SpriteMode, new[] { "Don't override", SpriteImportMode.Single.ToString(), SpriteImportMode.Multiple.ToString(), SpriteImportMode.Polygon.ToString() });
            SpriteMeshType = (SpriteMeshType) EditorGUILayout.Popup(new GUIContent("Sprite Mesh Type"), (int) SpriteMeshType, new[] { SpriteMeshType.FullRect.ToString(), SpriteMeshType.Tight.ToString() });
            EnableReadWrite = EditorGUILayout.Toggle(new GUIContent("Read/Write Enabled", ""), EnableReadWrite);
            TextureSize = EditorGUILayout.TextField(new GUIContent("Max Size", ""), TextureSize);
            FilterMode = (FilterMode) EditorGUILayout.Popup(new GUIContent("Filter Mode", ""), (int) FilterMode, Enum.GetValues(typeof(FilterMode)).Cast<FilterMode>().Select(i => new GUIContent(i.ToString())).ToArray());
            Compression = (TextureImporterCompression) EditorGUILayout.Popup(new GUIContent("Compression", ""), (int) Compression, Enum.GetValues(typeof(TextureImporterCompression)).Cast<TextureImporterCompression>().Select(i => new GUIContent(i.ToString())).ToArray());
            CrunchedCompression = EditorGUILayout.Toggle(new GUIContent("Use Crunch Compression", ""), CrunchedCompression);
            CompressionQuality = EditorGUILayout.IntSlider(new GUIContent("Compressor Quality", ""), CompressionQuality, 0, 100);

            if (GUILayout.Button("Setup"))
            {
                if (SpritesFolder == null)
                {
                    Debug.LogWarning("SpritesFolder is null");
                }
                else
                {
                    var root = AssetDatabase.GetAssetPath(SpritesFolder);
                    var files = Directory.GetFiles(root, "*.png", SearchOption.AllDirectories).Union(Directory.GetFiles(root, "*.psd", SearchOption.AllDirectories)).ToList();

                    for (var i = 0; i < files.Count; i++)
                    {
                        var progress = (float)i / files.Count;

                        SetImportSettings(files[i], SpriteMode, SpriteMeshType, EnableReadWrite, TextureSize, FilterMode, Compression, CrunchedCompression, CompressionQuality);

                        if (EditorUtility.DisplayCancelableProgressBar("Processing sprites", $"[{(int) (100 * progress)}%] [{i}/{files.Count}] Processing {files[i]}", progress))
                        {
                            break;
                        }
                    }

                    EditorUtility.ClearProgressBar();
                }
            }
        }

        public static void SetImportSettings(string path, int spriteMode, SpriteMeshType meshType, bool enableReadWrite, string size, FilterMode filterMode, TextureImporterCompression compression, bool crunchedCompression, int compressionQuality)
        {
            path = path.Replace("\\", "/");

            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            var targetImporter = (TextureImporter) AssetImporter.GetAtPath(path);

            targetImporter.spriteImportMode = spriteMode == 0 ? targetImporter.spriteImportMode : (SpriteImportMode) (spriteMode - 1);
            targetImporter.textureType = TextureImporterType.Sprite;
            targetImporter.spritePackingTag = null;
            targetImporter.alphaIsTransparency = true;
            targetImporter.isReadable = enableReadWrite;
            targetImporter.mipmapEnabled = false;
            targetImporter.wrapMode = TextureWrapMode.Clamp;
            targetImporter.filterMode = FilterMode.Bilinear;
            targetImporter.maxTextureSize = size == "[Auto]" ? targetImporter.spritePackingTag == "HelmetMask" ? 128 : GetMaxTextureSize(texture) : int.Parse(size);
            targetImporter.filterMode = filterMode;
            targetImporter.textureCompression = compression;
            targetImporter.compressionQuality = compressionQuality;
            targetImporter.crunchedCompression = crunchedCompression;

            var textureSettings = new TextureImporterSettings();

            targetImporter.ReadTextureSettings(textureSettings);
            textureSettings.spriteMeshType = meshType;
            textureSettings.spriteExtrude = 1;
            targetImporter.SetTextureSettings(textureSettings);
            targetImporter.SaveAndReimport();

            Debug.LogFormat("Import Settings set for: {0}", path);
        }

        private static int GetMaxTextureSize(Texture2D texture)
        {
            var maxTextureSize = Math.Max(texture.width, texture.height);

            for (var i = 5; i <= 11; i++)
            {
                var size = (int) Math.Pow(2, i);

                if (size >= maxTextureSize) return size;
            }

            return 2048;
        }
    }
}