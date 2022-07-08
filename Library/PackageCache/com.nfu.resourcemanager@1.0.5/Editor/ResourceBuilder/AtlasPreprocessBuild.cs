using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{
    public class AtlasPreprocessBuild : IResourcePreprocessBuild, ISerializationCallbackReceiver
    {
        [Tooltip("图集根目录，正则表达式格式")]
        public string spriteRootPath;

        [Tooltip("图集设置模板，生成的图集设置都从模板读取")]
        public SpriteAtlas template;
        [HideInInspector]
        [SerializeField]
        private string templateGuid;//= "4e0c5ce8812d122489a548a8462adc16";

        [Tooltip("如果为true 递归子目录生成图集，多级目录以'_'进行连接，如：a/b/c/1.png > a_b_c")]
        public bool recursive = true;


        private static string AtlasExtension = ".spriteatlas";
        private static Dictionary<string, List<string>> atlasFiles;

        public virtual void PreprocessBuildInitialize()
        {
            atlasFiles = new Dictionary<string, List<string>>();

        }


        public virtual void PreprocessBuild()
        {
            if (string.IsNullOrEmpty(spriteRootPath))
                throw new Exception($"[{nameof(spriteRootPath)}] null");

            Regex regex = new Regex(spriteRootPath + "$", RegexOptions.IgnoreCase);
            HashSet<string> allFolders = new HashSet<string>();
            foreach (var folder in Directory.GetDirectories("Assets", "*", SearchOption.AllDirectories))
            {
                string dir = folder.Replace('\\', '/');
                if (regex.IsMatch(dir))
                {
                    if (EditorResourceSettings.IsExcludeAssetPath(dir))
                        continue;
                    allFolders.Add(dir);
                }
            }

            foreach (var folder in allFolders)
            {
                Debug.Log("Gennerate atlas root directory: " + folder);
                var dirs = Directory.GetDirectories(folder, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                for (int i = 0; i < dirs.Length; i++)
                {
                    string dirPath = dirs[i];
                    if (MetaFile.IsEmptyDirectory(dirPath))
                        continue;

                    if (recursive)
                    {
                        //递归的必须是叶子目录
                        if (Directory.GetDirectories(dirPath, "*", SearchOption.TopDirectoryOnly).Length > 0)
                            continue;
                    }
                    dirPath = dirPath.Replace('\\', '/');
                    if (EditorResourceSettings.IsExcludeAssetPath(dirPath))
                        continue;
                    string atlasPath = Path.Combine(folder, dirPath.Substring(folder.Length + 1).Replace("/", EditorResourceSettings.AtlasDirectorySeparator) + AtlasExtension);
                    atlasPath = atlasPath.Replace('\\', '/');
                    PackOneFolder(dirPath, atlasPath);

                    List<string> files;
                    if (!atlasFiles.TryGetValue(folder, out files))
                    {
                        files = new List<string>();
                        atlasFiles[folder] = files;
                    }
                    files.Add(atlasPath);
                }

            }

            EditorUtility.ClearProgressBar();
        }


        void PackOneFolder(string folderPath, string atlasPath)
        {
            Object folder = AssetDatabase.LoadAssetAtPath<Object>(folderPath);

            if (folder)
            {
                SpriteAtlas atlas;

                //替换 DeleteAsset，图集发生差异才重新生成，提高性能
                atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);
                if (!atlas)
                {
                    atlas = new SpriteAtlas();
                    AssetDatabase.CreateAsset(atlas, atlasPath);
                }
                //清理
                var items = atlas.GetPackables();
                if (items != null)
                {
                    atlas.Remove(items);
                }
                GenerateAtlas(atlas);

                //改为文件夹，避免 LoadAssetAtPath<Sprite> 把所有图片对象加载到内存，提高性能
                atlas.Add(new Object[] { folder });

                EditorUtility.SetDirty(atlas);

                //设置atlas AssetBundleName 为atlasName
                //AssetImporter importer = AssetImporter.GetAtPath(atlasPath);
                //importer.assetBundleName = Path.GetFileNameWithoutExtension(atlasPath);
                //importer.SaveAndReimport();
                Debug.Log(string.Format("Packing Path: {0} into one Atlas: {1} ", folderPath, atlasPath));
            }
            else
            {
                //文件夹不存在则删除图集
                if (AssetDatabase.DeleteAsset(atlasPath))
                {
                    Debug.Log(string.Format("Deleta empty folder Atlas: {0}", atlasPath));
                }
            }
        }
        private SpriteAtlas GenerateAtlas(SpriteAtlas atlas)
        {
            SpriteAtlas tpl = template;

            // 设置参数 可根据项目具体情况进行设置
            SpriteAtlasPackingSettings packSetting;
            SpriteAtlasTextureSettings textureSetting;
            Dictionary<string, TextureImporterPlatformSettings> platformSettings = new Dictionary<string, TextureImporterPlatformSettings>();
            if (!atlas)
                atlas = new SpriteAtlas();

            if (tpl)
            {
                packSetting = tpl.GetPackingSettings();
                textureSetting = tpl.GetTextureSettings();
                TextureImporterPlatformSettings platformSetting;
                platformSetting = tpl.GetPlatformSettings("DefaultTexturePlatform");
                if (platformSetting != null)
                    platformSettings.Add(platformSetting.name, platformSetting);
                platformSetting = tpl.GetPlatformSettings("Android");
                if (platformSetting != null)
                    platformSettings.Add(platformSetting.name, platformSetting);
                platformSetting = tpl.GetPlatformSettings("iPhone");
                if (platformSetting != null)
                    platformSettings.Add(platformSetting.name, platformSetting);
                atlas.SetIsVariant(tpl.isVariant);
            }
            else
            {
                packSetting = new SpriteAtlasPackingSettings()
                {
                    blockOffset = 1,
                    enableRotation = false,
                    enableTightPacking = false,
                    padding = 2,
                };
                textureSetting = new SpriteAtlasTextureSettings()
                {
                    readable = false,
                    generateMipMaps = false,
                    sRGB = true,
                    filterMode = FilterMode.Bilinear,
                };


                TextureImporterPlatformSettings textureCompressDefault = new TextureImporterPlatformSettings();
                textureCompressDefault.overridden = false;
                textureCompressDefault.name = "DefaultTexturePlatform";
                textureCompressDefault.textureCompression = TextureImporterCompression.Compressed;
                // textureCompressDefault.compressionQuality = (int) UnityEngine.TextureCompressionQuality.Best;

                platformSettings[textureCompressDefault.name] = textureCompressDefault;

                TextureImporterPlatformSettings textureCompressIOS = new TextureImporterPlatformSettings();
                textureCompressIOS.name = "iPhone";
                textureCompressIOS.overridden = true;
                textureCompressIOS.textureCompression = TextureImporterCompression.Compressed;
                textureCompressIOS.format = TextureImporterFormat.ASTC_6x6;
                // textureCompressIOS.compressionQuality = (int) UnityEngine.TextureCompressionQuality.Best;

                platformSettings[textureCompressIOS.name] = textureCompressIOS;

                TextureImporterPlatformSettings textureCompressAndroid = new TextureImporterPlatformSettings();
                textureCompressAndroid.name = "Android";
                textureCompressAndroid.overridden = true;
                textureCompressAndroid.textureCompression = TextureImporterCompression.Compressed;
                textureCompressAndroid.format = TextureImporterFormat.ETC2_RGBA8;
                // textureCompressAndroid.compressionQuality = (int) UnityEngine.TextureCompressionQuality.Best;

                platformSettings[textureCompressAndroid.name] = textureCompressAndroid;
                atlas.SetIsVariant(false);
            }

            atlas.SetPackingSettings(packSetting);
            atlas.SetTextureSettings(textureSetting);

            foreach (var platformSetting in platformSettings.Values)
            {
                atlas.SetPlatformSettings(platformSetting);
            }

            atlas.SetIncludeInBuild(false);


            return atlas;


        }

        public bool GetAddressableName(string assetPath, out string bundleName, out string variant, out string assetName)
        {
            bundleName = null;
            variant = null;
            assetName = null;
            return false;
        }

        public void OnBeforeSerialize()
        {
            templateGuid = null;
            if (template)
            {
                long localId;
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(template, out templateGuid, out localId);
            }
        }

        public void OnAfterDeserialize()
        {
            template = null;
            if (!string.IsNullOrEmpty(templateGuid))
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(templateGuid);
                if (string.IsNullOrEmpty(assetPath))
                    Debug.LogError("Template null, guid: " + templateGuid);
                else
                    template = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(assetPath);
            }

        }

        public virtual void PreprocessBuildCleanup()
        {

            //清理文件夹不存在的图集，如：之前生成过图集，后来把文件夹删掉
            foreach (var item in atlasFiles)
            {
                string folder = item.Key;
                List<string> files = item.Value;
                foreach (var file in Directory.GetFiles(folder, "*.spriteatlas", SearchOption.TopDirectoryOnly).Select(o => o.Replace('\\', '/')))
                {
                    if (!files.Contains(file))
                    {
                        File.Delete(file);
                        Debug.Log($"Delete Atlas: {file}.spriteatlas");
                    }
                }
            }
        }

    }
}
