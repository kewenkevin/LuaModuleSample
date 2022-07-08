using System.Collections.Generic;
using System.Linq;
using ND.Managers.ResourceMgr.Editor;
using ResourceMgr.Runtime.ResourceUpdate;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CheckAppVersion))]
public class CheckAppVersionInspector : UnityEditor.Editor
{
    private List<HostData> hostList;
    
    private SerializedProperty m_CheckVersionUrl = null;
    
    
    private string[] m_CheckVersionConfigs = null;
    private string[] m_CheckVersionUrls = null;
    private int m_CheckVersionUrlIndex = 0;
    private void OnEnable()
    {
        hostList = HostServerData.Current.HostList;
        m_CheckVersionUrl = serializedObject.FindProperty("CheckVersionUrl");
        RefreshTypeNames();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.BeginVertical("box");
        int versionHelperSelectedIndex = EditorGUILayout.Popup("Host", m_CheckVersionUrlIndex, m_CheckVersionConfigs);
        if (versionHelperSelectedIndex != m_CheckVersionUrlIndex)
        {
            m_CheckVersionUrlIndex = versionHelperSelectedIndex;
            m_CheckVersionUrl.stringValue = versionHelperSelectedIndex < 0 ? null : m_CheckVersionUrls[versionHelperSelectedIndex];
        }
        EditorGUILayout.LabelField("CheckVersionUrl", m_CheckVersionUrl.stringValue);
        EditorGUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
    }

    private void RefreshTypeNames()
    {
        var list = new List<string>();
        list.Add("DefaultTestHost");
        list.AddRange(hostList.Select(t=> t.Name).ToList());
        var list2 = new List<string>();
        list2.Add($"http://{HostServerData.Current.IpAddress}:{HostServerData.Current.ServerPort}/versionInfos/{EditorUserBuildSettings.activeBuildTarget}/versionInfo.json");
        list2.AddRange(hostList.Select(t=> t.URL).ToList());
        m_CheckVersionConfigs = list.ToArray();
        m_CheckVersionUrls = list2.ToArray();
        m_CheckVersionUrlIndex = 0;
        if (!string.IsNullOrEmpty(m_CheckVersionUrl.stringValue))
        {
            m_CheckVersionUrlIndex = list2.IndexOf(m_CheckVersionUrl.stringValue);
            if (m_CheckVersionUrlIndex <= 0)
            {
                m_CheckVersionUrlIndex = 0;
                m_CheckVersionUrl.stringValue =  m_CheckVersionConfigs.Length == 0 ? "":m_CheckVersionUrls[m_CheckVersionUrlIndex];;
            }
        }
        else
        {
            m_CheckVersionUrlIndex = 0;
            m_CheckVersionUrl.stringValue =  m_CheckVersionConfigs.Length == 0 ? "":m_CheckVersionUrls[m_CheckVersionUrlIndex];;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
