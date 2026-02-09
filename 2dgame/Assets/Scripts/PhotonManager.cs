using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
/**
 * 포톤 서버에 연결하여 네트워크 통신을 지원합니다.
 */
public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string version = "1.0";
    private string userId = "Zacjk";

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = version;
        PhotonNetwork.NickName = userId;
        Debug.Log(PhotonNetwork.SendRate);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        Debug.Log($"inlobby ={PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log($"inlobby ={PhotonNetwork.InLobby}");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
    }
}