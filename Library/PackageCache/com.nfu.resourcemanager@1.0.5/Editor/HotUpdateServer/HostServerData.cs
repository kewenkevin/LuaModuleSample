using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEditor;
using UnityEngine;

[Serializable]
public class HostData
{
    public string Name;
    public string URL;
}
//[CreateAssetMenu(menuName = "ResourceManager/HostServerData")]
public class HostServerData : ScriptableObject
{
    public static HostServerData Instant = null;
    public string ServerFilePath = "";
    public HotUpdateService hus;
    public string IpAddress;

    

    private static HostServerData _current;
    public static HostServerData Current
    {
        get
        {
            HostServerData ser = null;
            if (TryGet("HostServer", out ser))
            {
                _current = ser;
            }
            else
            {
                _current = ScriptableObject.CreateInstance<HostServerData>();
                _current.name = "HostServer";
                var path = Application.dataPath + "/GameSettings/NFUSettings/ResourceManager";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    AssetDatabase.Refresh();
                }
                AssetDatabase.CreateAsset(_current, "Assets/GameSettings/NFUSettings/ResourceManager/HostServer.asset");
                _current = AssetDatabase.LoadAssetAtPath("Assets/GameSettings/NFUSettings/ResourceManager/HostServer.asset", typeof(HostServerData)) as HostServerData;
                
            }

            return _current;
        }
    }
    
    public static bool TryGet(string name, out HostServerData ser)
    {
        ser = null;
        var allPath = AssetDatabase.FindAssets("t:ScriptableObject", new string[] {"Assets"});
        for (var i = 0; i < allPath.Length; i++)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(allPath[i]);
            var scriptableObject = AssetDatabase.LoadAssetAtPath(assetPath, typeof(HostServerData)) as HostServerData;
            if (scriptableObject != null)
            {
                if (scriptableObject.name == "HostServer")
                {
                    ser = scriptableObject;
                    return true;
                }
            }
        }

        return false;
    }
    
    private void InitValue()
    {
        if (string.IsNullOrEmpty(ServerFilePath))
        {
            ServerFilePath = Application.dataPath.Replace("Assets", "") + "output/";
        }
        if (string.IsNullOrEmpty(IpAddress))
        {
            var ipAddressList = FilterValidIPAddresses(NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback && n.OperationalStatus == OperationalStatus.Up)
                .SelectMany(n => n.GetIPProperties().UnicastAddresses)
                .Where(a => a.Address.AddressFamily == AddressFamily.InterNetwork)
                .Select(a => a.Address).ToList());
            IpAddress = ipAddressList[0].ToString();
        }
    }
    
    private List<IPAddress> FilterValidIPAddresses(List<IPAddress> ipAddresses)
    {
        List<IPAddress> validIpList = new List<IPAddress>();

        foreach (IPAddress address in ipAddresses)
        {
            var sender = new System.Net.NetworkInformation.Ping();
            var reply = sender.Send(address.ToString(), 5000);
            if (reply.Status == IPStatus.Success)
            {
                validIpList.Add(address);
            }
        }
        return validIpList;
    }

    public int ServerPort = 18888;
   

    public bool ServerStart = false;

    public List<HostData> HostList = new List<HostData>();
    
    private void OnEnable()
    {
        InitValue();
        Instant = this;
        hus = new HotUpdateService();
        Debug.Log("HostServerData OnEnable");
        if (ServerStart)
        {
            hus.ServicePort = ServerPort;
            hus.RootDir = ServerFilePath;
            hus.StartServer();
        }
    }
    
    private void OnDisable()
    {
        Debug.Log("HostServerData OnDisable");
        if (ServerStart)
        {
            if (hus != null)
            {
                hus.StopHostingService();
            }
        }
    }
}
