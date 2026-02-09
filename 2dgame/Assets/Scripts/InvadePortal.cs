using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
/**
 * 포톤 서버에 기존의 생성된 룸 중 1명 있는 룸을 랜덤하게 들어갑니다.
 */
public class InvadePortal : MonoBehaviourPunCallbacks
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

    private void Update()
    {
        if (talkOn)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                //player.transform.position = new Vector3(movepos.x, movepos.y, player.transform.position.z);
                var confiner = cineCamera[0].GetComponent<CinemachineConfiner>();
                confiner.m_BoundingShape2D = collider2D;
                cineCamera[0].SetActive(false);
                cineCamera[0].SetActive(true);

                talkOn = false;
                transform.GetChild(0).gameObject.SetActive(false);
                calledBy = true;
                PhotonNetwork.JoinRandomRoom();
                player.GetComponent<Player>().KillCharacter();
            }
        }
    }

    public override void OnJoinedRoom()
    {
        if (calledBy)
        {
            Debug.Log($"PhotonNetwork.InRoom ={PhotonNetwork.InRoom}");
            Debug.Log($"plyaercouint ={PhotonNetwork.CurrentRoom.PlayerCount}");

            Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
            int idx = Random.Range(1, points.Length);
            PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0);

            calledBy = false;
        }
    }
}