using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CameraController : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera walkingCamera;
    [SerializeField]
    private CinemachineVirtualCamera aimingCamera;
    [SerializeField]
    private CinemachineVirtualCamera menuCamera;
    [SerializeField]
    private CinemachineVirtualCamera swordCam;

    private CinemachineImpulseSource impulse;
    private CinemachineImpulseSource impulseWalk;


    // Start is called before the first frame update
    void Start()
    {
        impulseWalk = walkingCamera.GetComponent<CinemachineImpulseSource>();
        impulse = aimingCamera.GetComponent<CinemachineImpulseSource>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SwitchToWalk()
    {
        walkingCamera.Priority = 1;
        aimingCamera.Priority = 0;
    }
    public void SwitchToAim()
    {
        walkingCamera.Priority = 0;
        aimingCamera.Priority = 1;
    }

    public void ShakeCam()
    {
        if (walkingCamera.Priority == 1)
        {
            impulseWalk.GenerateImpulse(Vector3.right);
        }
        else
            impulse.GenerateImpulse(Vector3.right);
    }
    public void SwitchToSwordCam()
    {
        swordCam.Priority = 2;
    }

    public void DisableMenuCam()
    {
        menuCamera.Priority = -1;
    }
}
