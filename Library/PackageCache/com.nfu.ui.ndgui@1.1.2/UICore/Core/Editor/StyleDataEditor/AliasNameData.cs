using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ND.UI.Core.StyleDataEditor
{


    internal class AliasNameData
    {
        private static readonly Dictionary<object, AliasNameData> m_Dict = new Dictionary<object, AliasNameData>();

        public static AliasNameData Get(ScriptableObject obj)
        {
            if (m_Dict.TryGetValue(obj, out var data)) return data;

            data = new AliasNameData(obj);
            m_Dict.Add(obj, data);

            return data;
        }

        public static AliasNameData Get(Object obj)
        {

            if (m_Dict.TryGetValue(obj, out var data)) return data;

            var path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path))
            {
                throw new UnityException("Required An Object Under Assets Folder.");
                return null;
            }

            data = new AliasNameData(obj);
            m_Dict.Add(obj, data);

            return data;
        }

        private readonly Object m_Source;

        private AliasNameData(Object obj)
        {
            m_Source = obj;
            LoadUserData();
        }

        private string m_AliasName;

        public string AliasName
        {
            get => string.IsNullOrEmpty(m_AliasName) ? m_Source.name : m_AliasName;
            set
            {
                m_AliasName = value;
                SaveUserData();
            }
        }

        private AssetImporter AssetImporter => AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(m_Source));

        private void LoadUserData()
        {
            if (AssetImporter != null)
            {
                object tempStruct = new AliasNameStruct();
                EditorJsonUtility.FromJsonOverwrite(AssetImporter.userData, tempStruct);
                m_AliasName = ((AliasNameStruct) tempStruct).value;

            }

            if (string.IsNullOrEmpty(m_AliasName))
            {
                m_AliasName = m_Source.name;
            }


        }

        private void SaveUserData()
        {
            var tempStruct = new AliasNameStruct();
            tempStruct.value = m_AliasName;

            var importer = AssetImporter;
            importer.userData = EditorJsonUtility.ToJson(tempStruct);
            importer.SaveAndReimport();
        }

        private struct AliasNameStruct
        {
            public string value;
        }
    }
}