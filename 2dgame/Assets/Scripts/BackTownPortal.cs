using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Serialization;
/**
 * 사냥터에 있는 전투중이지 않은 플레이어를 마을로 이동시키는 포탈입니다.
 *
 * 이 경우 포톤서버의 룸에서도 나오게 됩니다.
 */
public class BackTownPortal : MonoBehaviourPunCallbacks
{
    #region private member
    private bool isPlayerStayingDoor = false;
    private bool isLocked=false;
    private bool hasLeavedComplete = false;
    private GameObject player;
    private GameObject[] cineCamera;
    private Vector2 returnLocation = new Vector2(57.2f, 5.6f);
    private GameObject moveUI;
    private GameObject lockedUI;
    [SerializeField] private GameObject prefabPlayer;
    [SerializeField] private Collider2D cineCameraCollider2D;
    #endregion private memeber
    
    private void Start()
    {
        cineCamera = GameObject.FindGameObjectsWithTag("VirtualCamera");
        moveUI=transform.GetChild(0).gameObject;
        lockedUI = transform.GetChild(1).gameObject;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerStayingDoor = true;
            player = collision.gameObject;

        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerStayingDoor = false;
        }
    }

    private void Update()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            //플레이어가 2명이면 한명을 처치하기 전까지는 잠깁니다.
            if (PhotonNetwork.CurrentRoom.PlayerCount > 1) isLocked = true;
            else isLocked = false;
        }
        else isLocked = false;

        if(isPlayerStayingDoor&&!isLocked)
        {
            moveUI.SetActive(true);
            lockedUI.SetActive(false);
            
            //룸을 떠난 뒤 플레이어를 파괴하고 마을에서 다시 생성합니다.
            if (Input.GetKeyDown(KeyCode.F))
            {
                PhotonNetwork.LeaveRoom();
                player.GetComponent<Player>().KillCharacter();

                var confiner = cineCamera[0].GetComponent<CinemachineConfiner>();
                confiner.m_BoundingShape2D = cineCameraCollider2D;
                //바운딩을 바꾸어서 카메라를 한번 껐다 켜줍니다.
                cineCamera[0].SetActive(false);
                cineCamera[0].SetActive(true);

                isPlayerStayingDoor = false;
                moveUI.SetActive(false);
                lockedUI.SetActive(false);
                hasLeavedComplete = true;
            }
        }
        else if(isPlayerStayingDoor)
        {
            moveUI.SetActive(false);
            lockedUI.SetActive(true);
        }
        else
        {
            moveUI.SetActive(false);
            lockedUI.SetActive(false);
        }
        //룸을 떠나 성공적으로 마을로 돌아왔을 경우 플레이어를 재생성하고 카메라를 다시 설정합니다.
        if(hasLeavedComplete)
        {
            if (PhotonNetwork.CurrentRoom == null)
            {
                GameObject prefabbuffer = Instantiate(prefabPlayer,returnLocation , Quaternion.identity);
                cineCamera[0].GetComponent<CinemachineVirtualCamera>().Follow = prefabbuffer.transform;
                hasLeavedComplete = false;
            }
            
        }
    }

   
       

    





}