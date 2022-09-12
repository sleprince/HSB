using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.TextureImportTools.Editor
{
    /// <summary>
    /// Copy sprite import settings for single sprites and sprite sheet for multiple sprites.
    /// </summary>
    public class TextureImportSettingsCopy : EditorWindow
    {
        public Texture2D Source;
        public Object Target;

        [MenuItem("Window/Texture Import Settings/Copy")]
        public static void Init()
        {
            var window = GetWindow<TextureImportSettingsCopy>(false, "Import Settings / Copy");

            window.minSize = window.maxSize = new Vector2(300, 150);
            window.Show();
        }

        public void OnGUI()
        {
            EditorGUILayout.LabelField("Copy texture import settings and sprite sheet meta", new GUIStyle(EditorStyles.label) { normal = { textColor = Color.yellow } });
            Source = (Texture2D) EditorGUILayout.ObjectField(new GUIContent("Source Texture"), Source, typeof(Texture2D), false);
            Target = EditorGUILayout.ObjectField(new GUIContent("Target Texture or Folder"), Target, typeof(Object), false);

            if (GUILayout.Button("Copy"))
            {
                if (Source == null)
                {
                    Debug.LogWarning("Source is null");
                }
                else if (Target == null)
                {
                    Debug.LogWarning("Destination is null");
                }
                else if (Target is Texture2D destination)
                {
                    CopyPivotsAndSlices(Source, destination);
                }
                else
                {
                    var path = AssetDatabase.GetAssetPath(Target);
                    var textures = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories).Select(AssetDatabase.LoadAssetAtPath<Texture2D>).ToList();

                    foreach (var texture in textures)
                    {
                        CopyPivotsAndSlices(Source, texture);
                    }
                }
            }
        }

        private static void CopyPivotsAndSlices(Texture2D copyFrom, Texture2D copyTo)
        {
            if (!Mathf.Approximately((float) copyFrom.width / copyFrom.height, (float) copyTo.width / copyTo.height))
            {
                Debug.LogWarning($"Skipped: {copyTo.name} because it has different aspect. If it was asymmetric sprite - repeat the operation with another template.");
                return;
            }

            var copyFromPath = AssetDatabase.GetAssetPath(copyFrom);
            var source = (TextureImporter) AssetImporter.GetAtPath(copyFromPath);
            
            source.isReadable = true;

            var copyToPath = AssetDatabase.GetAssetPath(copyTo);
            var target = (TextureImporter) AssetImporter.GetAtPath(copyToPath);
            var ratio = copyFrom.width / copyTo.width;

            target.isReadable = false;
            target.spriteImportMode = source.spriteImportMode;

            var settings = new TextureImporterSettings();

            source.ReadTextureSettings(settings);
            target.SetTextureSettings(settings);

            if (source.spriteImportMode == SpriteImportMode.Multiple)
            {
                Debug.Log($"{source.spritesheet.Length} fragments found: {string.Join(",", source.spritesheet.Select(i => i.name))}");

                var spritesheet = new SpriteMetaData[source.spritesheet.Length];

                for (var i = 0; i < source.spritesheet.Length; i++)
                {
                    var meta = new SpriteMetaData
                    {
                        alignment = source.spritesheet[i].alignment,
                        pivot = source.spritesheet[i].pivot,
                        name = source.spritesheet[i].name,
                        rect = source.spritesheet[i].rect,
                        border = source.spritesheet[i].border
                    };

                    meta.rect.min /= ratio;
                    meta.rect.max /= ratio;

                    spritesheet[i] = meta;
                }

                target.spritesheet = spritesheet;
            }

            AssetDatabase.ImportAsset(copyToPath, ImportAssetOptions.ForceUpdate);
            Debug.LogFormat($"Imported: {copyToPath}");
        }
    }
}