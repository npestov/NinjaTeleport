using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndShoot : MonoBehaviour
{
    private Vector3 mousePressDownPos;
    private Vector3 mouseReleasePos;

    private bool isShoot;
    private Rigidbody rb;

    [SerializeField]
    private float forceMultiplier = 2;

    private DrawTrajectory drawTrajectory;
    private AttackMoveController attackMoveController;
    private HitDetection hitDetection;

    private Vector3 startingHeightOffset = new Vector3(0, 100, 0);

    private bool firstPressDown;

    //Arch
    /*
    void Awake()
    {
        attackMoveController = FindObjectOfType<AttackMoveController>();
        rb = GetComponent<Rigidbody>();
        drawTrajectory = FindObjectOfType<DrawTrajectory>();
        hitDetection = FindObjectOfType<HitDetection>();
    }

    private void MouseDown()
    {
        GameManager.Instance.UpdateGameState(GameState.Aiming);
        mousePressDownPos = Input.mousePosition;
    }

    private void Dragging()
    {
        Vector3 forceInt = (Input.mousePosition - mousePressDownPos + startingHeightOffset);
        Vector3 forceV = (new Vector3(forceInt.x, forceInt.y, forceInt.y)) * forceMultiplier;
        if (Input.mousePosition.y >= mousePressDownPos.y)
            drawTrajectory.UpdateTrajectory(forceV, rb, transform.position);
        else
            drawTrajectory.UpdateTrajectory(new Vector3(forceV.x, 20, forceV.z), rb, transform.position);
    }

    private void MouseUp()
    {
        firstPressDown = true;
        if (GameManager.Instance.isBonus)
        {
            //GameManager.Instance.isBonus = false;
            attackMoveController.ThrowForBonus(drawTrajectory.GetAimTarget());
        }
        else if (GameManager.Instance.State == GameState.Aiming || attackMoveController.enemyToKill != null)
        {
            hitDetection.KillIfPossible();
        }
        drawTrajectory.HideLine();
        mouseReleasePos = Input.mousePosition;
    }


    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.State == GameState.Lose || GameManager.Instance.State == GameState.Victory || GameManager.Instance.State == GameState.Menu || GameManager.Instance.State == GameState.Killing)
            return;

        if (Input.GetMouseButtonDown(0))
            MouseDown();
        if (Input.GetMouseButtonUp(0))
            MouseUp();
        if (Input.GetMouseButton(0) && firstPressDown)
            Dragging();
    }
    */

    void Awake()
    {
        attackMoveController = FindObjectOfType<AttackMoveController>();
        hitDetection = FindObjectOfType<HitDetection>();
    }

    private void Update()
    {
        if (GameManager.Instance.State == GameState.Walking && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Lol im in");
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Enemy")))
            {
                hitDetection.CheckIfHitEnemy(hit);
                return;
            }
        }
    }
}

