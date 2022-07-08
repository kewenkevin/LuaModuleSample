using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEditor;
using UnityEngine;

public class ServerWindow : EditorWindow
{
    private bool RefleshIP = true;
    private IPAddress ipAddress;
    public HostServerData HSD;

    public void createHus()
    {
        
    }

    [MenuItem("Tools/ResourceManager/UpdateServer")]
    public static void ShowWindow()
    {
        ServerWindow window = GetWindow<ServerWindow>("UpdateServer", true);
        window.HSD = HostServerData.Current;
    }
    


    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        if (HSD.ServerStart == false)
        {
            HSD.ServerFilePath =
                EditorGUILayout.TextField("服务器文件根目录", HSD.ServerFilePath);
            HSD.ServerPort =
                EditorGUILayout.IntField("服务器端口", HSD.ServerPort);
            if (GUILayout.Button("打开服务器"))
            {
                HSD.ServerStart = true;
                HSD.hus.ServicePort = HSD.ServerPort;
                HSD.hus.RootDir = HSD.ServerFilePath;
                HSD.hus.StartServer();
            }
        }
        else
        {
            EditorGUILayout.LabelField("服务器文件根目录", HSD.ServerFilePath);
            EditorGUILayout.LabelField("服务器端口", HSD.ServerPort.ToString());
            if (GUILayout.Button("关闭服务器"))
            {
                HSD.ServerStart = false;
                HSD.hus.StopHostingService();
            }
            DrawOutline(new Rect(10, 100, 600, 200), 1);
            GUILayout.BeginArea(new Rect(10, 100, 600, 200));
            {
                GUILayout.Label("IP:" + HSD.IpAddress.ToString());
                GUILayout.Label("Port:" + HSD.ServerPort.ToString());
            }
            GUILayout.EndArea();
        }
        
        

        EditorGUILayout.EndVertical();
    }
    
    internal static void DrawOutline(Rect rect, float size)
    {
        Color color = new Color(0.6f, 0.6f, 0.6f, 1.333f);
        if (EditorGUIUtility.isProSkin)
        {
            color.r = 0.12f;
            color.g = 0.12f;
            color.b = 0.12f;
        }

        if (Event.current.type != EventType.Repaint)
            return;

        Color orgColor = UnityEngine.GUI.color;
        UnityEngine.GUI.color = UnityEngine.GUI.color * color;
        UnityEngine.GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, size), EditorGUIUtility.whiteTexture);
        UnityEngine.GUI.DrawTexture(new Rect(rect.x, rect.yMax - size, rect.width, size), EditorGUIUtility.whiteTexture);
        UnityEngine.GUI.DrawTexture(new Rect(rect.x, rect.y + 1, size, rect.height - 2 * size), EditorGUIUtility.whiteTexture);
        UnityEngine.GUI.DrawTexture(new Rect(rect.xMax - size, rect.y + 1, size, rect.height - 2 * size), EditorGUIUtility.whiteTexture);

        UnityEngine.GUI.color = orgColor;
    }
}
