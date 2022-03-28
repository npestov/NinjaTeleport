using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickDetection : MonoBehaviour
{
    private const float DOUBLE_CLICK_TIME = 0.2f;
    private float lastClickTime;
    private PlayerVision playerVision;
    public LayerMask clickThroughLayers;
    private AttackMoveController attackMoveController;


    private void Awake()
    {
        playerVision = FindObjectOfType<PlayerVision>();
        attackMoveController = FindObjectOfType<AttackMoveController>();
    }
    // Update is called once per frame
    void Update()
    {
        /*
        //start the game is the state is menu
        if (Input.GetMouseButtonDown(0))
        {
            float timeSinceLastClick = Time.time - lastClickTime;
            if (timeSinceLastClick * (1 / Time.timeScale) <= DOUBLE_CLICK_TIME)
            {
                //attack que has an option to be activated after the aiming has behun, upon double tap on top of killing another attack will be qued
                if (GameManager.Instance.State == GameState.Aiming || GameManager.Instance.State == GameState.Killing)
                {
                    //attackMoveController.isAttackQueued = true;
                }
                
                if (GameManager.Instance.State == GameState.Walking)
                {
                    playerVision.ProposeAim();
                    attackMoveController.isAttackQueued = false;
                }
            }

            lastClickTime = Time.time;
            }
        */


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = false;
        }
    }

    public bool IsClickOverUI()
    {
        return IsClickOverIgnore();
    }

    private bool IsClickOverIgnore()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
        for (int i = 0; i < raycastResultList.Count; i++)
        {
            if (clickThroughLayers == (clickThroughLayers | (1 << raycastResultList[i].gameObject.layer)))
            {
                raycastResultList.RemoveAt(i);
                i--;
            }
        }
        return raycastResultList.Count > 0;
    }
}
