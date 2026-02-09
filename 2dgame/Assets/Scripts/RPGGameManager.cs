using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
/**
 * 플레이어 스폰 및 속성을 관리합니다.
 */
public class RPGGameManager : MonoBehaviour
{
    public static RPGGameManager sharedInstance = null;
    
    public bool gotCrown { get; set; }
    public SpawnPoint playerSpawnPoint;
    public RPGCameraManager cameraManager;
    public GameObject deadcanvas;
    public Collider2D collider2D;
    private CinemachineVirtualCamera virtualCamera;
    public float hitpoints;
    public int quantity;


    void Awake()
    {
        if (sharedInstance != null && sharedInstance != this)
        {
            // We only ever want one instance to exist, so destroy the other existing object
            Destroy(gameObject);
        }
        else
        {
            // If this is the only instance, then assign the sharedInstance variable to the current object.
            sharedInstance = this;
        }
    }

    void Start()
    {
        // Consolidate all the logic to setup a scene inside a single method. 
        // This makes it easier to call again in the future, in places other than the Start() method.

        virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        SetupScene();

    }
    void Update()
    {
        if (Input.GetKey("escape"))
            Application.Quit();
    }


    public void SetupScene()
    {
        
          SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        deadcanvas.SetActive(false);
        var confiner = virtualCamera.GetComponent<CinemachineConfiner>();
        confiner.m_BoundingShape2D = collider2D;
        virtualCamera.gameObject.SetActive(false);
        virtualCamera.gameObject.SetActive(true);
        GameObject player;
        if (playerSpawnPoint != null)
        {
            player = playerSpawnPoint.SpawnObject();
            cameraManager.virtualCamera.Follow = player.transform;
        }
    }
}
