using UnityEngine;
using Cinemachine;
/**
 * 시네머신 카메라를 관리합니다.
 */
public class RPGCameraManager : MonoBehaviour {

    public static RPGCameraManager sharedInstance = null;
    [HideInInspector]
    public CinemachineVirtualCamera virtualCamera;

    void Awake()
    {
        if (sharedInstance != null && sharedInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            sharedInstance = this;
        }
        GameObject vCamGameObject = GameObject.FindWithTag("VirtualCamera");
        virtualCamera = vCamGameObject.GetComponent<CinemachineVirtualCamera>();
    }
}