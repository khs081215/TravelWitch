using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;

/**
 * 포톤 서버에 새로운 룸을 생성해 사냥터로 이동합니다.
 */
public class HuntingPortal : MonoBehaviourPunCallbacks
{
    private bool talkOn = false;
    private bool calledBy = false;
    private GameObject player;
    private GameObject[] cineCamera;
    [SerializeField] private Collider2D collider2D;

    private void Start()
    {
        cineCamera = GameObject.FindGameObjectsWithTag("VirtualCamera");
    }

    private void Update()
    {
        if (talkOn)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                //player.transform.position = new Vector3(movepos.x, movepos.y, player.transform.position.z);
                player.GetComponent<Player>().KillCharacter();

                var confiner = cineCamera[0].GetComponent<CinemachineConfiner>();
                confiner.m_BoundingShape2D = collider2D;
                cineCamera[0].SetActive(false);
                cineCamera[0].SetActive(true);

                talkOn = false;
                transform.GetChild(0).gameObject.SetActive(false);
                calledBy = true;
                createroom();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            talkOn = true;
            player = collision.gameObject;
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            talkOn = false;
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void createroom()
    {
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 2;
        ro.IsOpen = true;
        ro.IsVisible = true;
        string randomRoomName = "Room_" + Random.Range(1000, 9999);
        PhotonNetwork.CreateRoom(randomRoomName, ro);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"roomname ={PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnJoinedRoom()
    {
        if (calledBy)
        {
            Debug.Log($"PhotonNetwork.InRoom ={PhotonNetwork.InRoom}");
            Debug.Log($"plyaercouint ={PhotonNetwork.CurrentRoom.PlayerCount}");

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Instantiate("Player", new Vector2(95.0f, -1.35f), transform.rotation, 0);
            }

            calledBy = false;
        }
    }
}