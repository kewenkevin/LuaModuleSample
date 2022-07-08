using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace ND.Gameplay.Managers.ResourceManagerV2.Editor
{
    public class AtlasBuild
    {
        private static string m_guiFolder = "Sample/ResourceManager/ResourcesAssets/UI/GUI";
        private static string m_iconFolder = "Sample/ResourceManager/ResourcesAssets/UI/GUI/icon";
        //m_guiVariantFolderprivate static string m_guiVariantFolder = "ResourceVariant/En/Assets/";

        private static string m_extention = ".spriteatlas";
        [MenuItem("Tools/Build/Atlas Pack")]
        public static void GenerateBuild()
        {
            var files = Directory.GetDirectories(Path.Combine(Application.dataPath, m_guiFolder), "*", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; i++)
            {
                string dirPath = FullPath2Relative(files[i]);
                if (!dirPath.Contains(m_iconFolder))
                {
                    PackOneFolder(dirPath, dirPath + m_extention);
                }
            }

            var iconDicPath = Path.Combine(Application.dataPath, m_iconFolder);
            files = Directory.GetDirectories(iconDicPath, "*", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; i++)
            {
                string dirPath = FullPath2Relative(files[i]);
                string fileName = dirPath.Substring(FullPath2Relative(iconDicPath).Length + 1).Replace("/", "_");
                PackOneFolder(dirPath, FullPath2Relative(iconDicPath) + "_" + fileName + m_extention);
            }
            //files = Directory.GetDirectories(Path.Combine(Application.dataPath,m_guiVariantFolder,m_guiFolder), "*", SearchOption.TopDirectoryOnly);
            //for (int i = 0; i < files.Length; i++)
            //{
            //    string dirPath = FullPath2Relative(files[i]);
            //    PackOneFolder(dirPath);
            //}

            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Build/Atlas Delete")]
        public static void DeleteBuild()
        {
            var files = Directory.GetFiles(Path.Combine(Application.dataPath,m_guiFolder), "*.spriteatlas", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
            }
            
            //files = Directory.GetDirectories(Path.Combine(Application.dataPath,m_guiVariantFolder,m_guiFolder), "*.spriteatlas", SearchOption.TopDirectoryOnly);
            //for (int i = 0; i < files.Length; i++)
            //{
            //    File.Delete(files[i]);
            //}
            
            AssetDatabase.Refresh();
        }
        static void PackOneFolder(string folderPath,string atlasPath)
        {
            List<Object> assets = new List<Object>();
            string[] assetsGUIDs = AssetDatabase.FindAssets("t:texture", new string[] { folderPath });
            foreach (var guid in assetsGUIDs)
            {
                Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid)) as Sprite;
                if (sp != null)
                    assets.Add(sp);
            }
            if (assets.Count > 0)
            {
                SpriteAtlas atlas = GenerateAtlas(atlasPath);
                atlas.Add(assets.ToArray());
                if (AssetDatabase.DeleteAsset(atlasPath))
                    Debug.Log(string.Format("Deleta Old Atlas: {0}", atlasPath));
                AssetDatabase.CreateAsset(atlas, atlasPath);
     
                //设置atlas AssetBundleName 为atlasName
                //AssetImporter importer = AssetImporter.GetAtPath(atlasPath);
                //importer.assetBundleName = Path.GetFileNameWithoutExtension(atlasPath);
                //importer.SaveAndReimport();
                Debug.Log(string.Format("Packing Path: {0} into one Atlas: {1} ", folderPath, atlasPath));
            }
        }
        static string FullPath2Relative(string fullPath)
        {
            string relativePath = fullPath.Substring(fullPath.IndexOf("Assets"));
            relativePath = relativePath.Replace("\\", "/");
            return relativePath;
        }
        private static SpriteAtlas GenerateAtlas(string atlasPath)
        {
            // 设置参数 可根据项目具体情况进行设置
            SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
            {
                blockOffset = 1,
                enableRotation = false,
                enableTightPacking = false,
                padding = 2,
            };
     
            SpriteAtlasTextureSettings textureSetting = new SpriteAtlasTextureSettings()
            {
                readable = false,
                generateMipMaps = false,
                sRGB = true,
                filterMode = FilterMode.Bilinear,
            };
     
            SpriteAtlas atlas = new SpriteAtlas();
            atlas.SetPackingSettings(packSetting);
            atlas.SetTextureSettings(textureSetting);
            
            TextureImporterPlatformSettings textureCompressDefault = new TextureImporterPlatformSettings();
            textureCompressDefault.overridden = false;
            textureCompressDefault.name = "DefaultTexturePlatform";
            textureCompressDefault.textureCompression = TextureImporterCompression.Compressed;
            // textureCompressDefault.compressionQuality = (int) UnityEngine.TextureCompressionQuality.Best;
            atlas.SetPlatformSettings(textureCompressDefault);

            TextureImporterPlatformSettings textureCompressIOS = new TextureImporterPlatformSettings();
            textureCompressIOS.name = "iPhone";
            textureCompressIOS.overridden = true;
            textureCompressIOS.textureCompression = getTextureCompression(atlasPath);
            //textureCompressIOS.compressionQuality = (int) TextureCompressionQuality.Normal;
            textureCompressIOS.format = TextureImporterFormat.RGBA32;
            atlas.SetPlatformSettings(textureCompressIOS);

            TextureImporterPlatformSettings textureCompressAndroid = new TextureImporterPlatformSettings();
            textureCompressAndroid.name = "Android";
            textureCompressAndroid.overridden = true;
            textureCompressAndroid.textureCompression = getTextureCompression(atlasPath);
            textureCompressAndroid.format = TextureImporterFormat.RGBA32;
            //textureCompressAndroid.compressionQuality = (int) TextureCompressionQuality.Normal;
            atlas.SetPlatformSettings(textureCompressAndroid);
            
            atlas.SetIncludeInBuild(false);
            atlas.SetIsVariant(false);
            return atlas;
     
            
        }

        private static TextureImporterCompression getTextureCompression(string altasPath)
        {
            if (altasPath.Contains("commonui"))
            {
                return TextureImporterCompression.Compressed;
            }
            else
            {
                return TextureImporterCompression.Compressed;
            }
        }

    }
}