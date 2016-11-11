using NATTraversal;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.Networking.Types;
using System.Runtime.InteropServices;

[HelpURL("http://grabblesgame.com/nat-traversal/docs/class_n_a_t_traversal_1_1_network_manager.html")]
public class ExampleNetworkManager : NATTraversal.NetworkManager
{
    
    public override void Start()
    {
        base.Start();
        //StartHostAll("Hello World", 6);
    }

#if UNITY_5_4
    public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        int matchCount = matchList.Count;
        MatchInfoSnapshot match = null;
#else
    public override void OnMatchList(ListMatchResponse matchList)
    {
        bool success = matchList.success;
        int matchCount = matchList.matches.Count;
        MatchDesc match = null;
#endif
        if (!success)
        {
            Debug.Log("Failed to retrieve match list");
            return;
        }

        if (matchCount == 0)
        {
            Debug.Log("Match list is empty");
            return;
        }

        if (natHelper.guid != 0)
        {
            // If we have a guid we can use it to make sure we don't
            // try and join our own old match. This can happen when quickly switching
            // from hosting to joining because old matches are not cleaned up immediately
            // and there's no way to be notified when they are cleaned up
#if UNITY_5_4
            foreach (MatchInfoSnapshot m in matchList)
#else
            foreach (MatchDesc m in matchList.matches)
#endif

            {
                string[] parts = m.name.Split(':');
                ulong hostGUID;
                ulong.TryParse(parts[parts.Length - 1], out hostGUID);
                if (hostGUID == natHelper.guid)
                {
                    Debug.Log("Not joining old match");
                }
                else
                {
                    match = m;
                    break;
                }
            }
        }
        else
        {
#if UNITY_5_4
            match = matchList[0];
#else
            match = matchList.matches[0];
#endif
        }

        if (match == null)
        {
            Debug.Log("Match list is empty");
            return;
        }

        Debug.Log("Found a match, joining");

        matchID = match.networkId;
        StartClientAll(match);
    }

    // This will never actually be called (thanks Unity) but you have to define it and pass
    // it in to DropConnection anyway or you can't leave the match..
#if UNITY_5_4
    private void OnMatchDropped(bool success, string extendedInfo)
    {
        Debug.LogWarning("Match dropped");
    }
#else
    public void OnMatchDropped(BasicResponse resp)
    {
        Debug.LogWarning("Match dropped");
    }
#endif

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "Host"))
        {
            if (matchMaker == null) matchMaker = gameObject.AddComponent<NetworkMatch>();

            //matchMaker.CreateMatch("test", 10, true, "", OnMatchCreate);
            StartHostAll("Hello World", 6);
        }
        if (GUI.Button(new Rect(10, 110, 150, 100), "Join"))
        {
            if (matchMaker == null) matchMaker = gameObject.AddComponent<NetworkMatch>();

#if UNITY_5_4
            matchMaker.ListMatches(0, 10, "", true, 0, 0, OnMatchList);
#else
            matchMaker.ListMatches(0, 10, "", OnMatchList);
#endif
        }
        if (GUI.Button(new Rect(10, 210, 150, 100), "Disconnect"))
        {
            if (matchMaker && matchID != NetworkID.Invalid && matchmakingNodeID != NodeID.Invalid)
            {
                if (NetworkServer.active)
                {
#if UNITY_5_4
                    matchMaker.DestroyMatch(matchID, 0, OnMatchDropped);
#else
                    matchMaker.DestroyMatch(matchID, OnMatchDropped);
#endif
                }
                else
                {
#if UNITY_5_4
                    matchMaker.DropConnection(matchID, matchmakingNodeID, 0, OnMatchDropped);
#else
                    matchMaker.DropConnection(matchID, matchmakingNodeID, OnMatchDropped);
#endif
                }
            }

            if (NetworkServer.active)
            {
                NetworkServer.SetAllClientsNotReady();
                StopHost();
            }
            else
            {
                StopClient();
            }
        }

        if (NetworkServer.active)
        {
            if (GUI.Button(new Rect(10, 310, 150, 100), "Send To All"))
            {
                NetworkServer.SendToAll(MsgType.OtherTestMessage, new EmptyMessage());
            }
        }

        GUI.Label(new Rect(10, 410, 300, 100), "Is connected to Facilitator: " + natHelper.isConnectedToFacilitator);
    }

    public override void OnDoneConnectingToFacilitator(ulong guid)
    {
        if (guid == 0)
        {
            Debug.Log("Failed to connect to Facilitator");
        }
        else
        {
            Debug.Log("Facilitator connected");
        }
    }

    private void OnTestMessage(NetworkMessage netMsg)
    {
        Debug.Log("Received test message");
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        // Test sending a large message that requires fragmentation
        //byte[] data = new byte[40000];
        //for (int i = 0; i < data.Length; i++) data[i] = (byte)UnityEngine.Random.Range(0, 255);
        //LargeMessage msg = new LargeMessage(data);

        //conn.Send(MsgType.LargeMessage, msg);
    }
    
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("on server add player: " + playerControllerId);
        base.OnServerAddPlayer(conn, playerControllerId);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        NetworkServer.RegisterHandler(MsgType.OtherTestMessage, OnTestMessage);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        RegisterHandlerClient(MsgType.OtherTestMessage, OnTestMessage);
        //RegisterHandlerClient(MsgType.LargeMessage, OnLargeMessage);
    }

    /// <summary>
    /// Test receiving large fragmented messages because why not
    /// </summary>
    /// <param name="msg">The MSG.</param>
    //void OnLargeMessage(NetworkMessage msg)
    //{
    //    LargeMessage largeMsg = msg.ReadMessage<LargeMessage>();
    //    Debug.Log("Received large message: " + largeMsg.lotsOfData.Length);
    //}

}

class MsgType : NATTraversal.MsgType
{
    public static short LargeMessage = Highest + 1;
    public static short OtherTestMessage = Highest + 2;
}

class LargeMessage : MessageBase
{
    public byte[] lotsOfData;

    public LargeMessage() { }

    public LargeMessage(byte[] data)
    {
        lotsOfData = data;
    }
}
