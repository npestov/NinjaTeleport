using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IndieMarc.EnemyVision;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PlayerVision : MonoBehaviour
{
    private AttackMoveController attackMoveController;
    private CameraController cameraController;

    public List<Transform> screenTargets = new List<Transform>();
    private Transform oldTarget;
    public Transform target;
    private ClickDetection clickDetection;
    public bool isSwitchBlocked;



    // Start is called before the first frame update
    void Awake()
    {
        cameraController = FindObjectOfType<CameraController>();
        attackMoveController = GetComponent<AttackMoveController>();
        clickDetection = FindObjectOfType<ClickDetection>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.State == GameState.Victory)
            return;

        //Switch target in case new one is aimed at
        if (!isSwitchBlocked)
            ConstantlySwitchTarget();

        //quick kill if target is close
        RequestQuickKill();
    }

    public void ProposeAim()
    {
        //find the closest available target to aim towards, if none are in the view then decline aim and shake cam
        float closestDistance = 99999999999;
        Transform closestTarget = null;
        for (int i = 0; i < screenTargets.Count; i++)
        {
            if (IsTargetVisible(screenTargets[i]) && !screenTargets[i].GetComponentInParent<TargetScript>().isDead)
            {
                float distance = Vector3.Distance(screenTargets[i].transform.position, transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = screenTargets[i];
                }
            }
        }
        if (closestTarget != null)
        {
            target = closestTarget;
            HighlightTarget();
            GameManager.Instance.UpdateGameState(GameState.Aiming);
            isSwitchBlocked = true;
            transform.DOLookAt(target.position, 0.02f).OnComplete(()=> isSwitchBlocked = false);
        }
        else
        {
            cameraController.ShakeCam();
        }
    }

    private bool IsTargetVisible(Transform targetPos)
    {
        if (targetPos == null)
            return false;

        var ray = new Ray(transform.position, (targetPos.transform.parent.position - transform.position));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            //obstacel is 9
            if (hit.transform.gameObject.layer != 9)
            {
                return true;
            }

        }
        return false;
    }

    private void ConstantlySwitchTarget()
    {
        if (GameManager.Instance.State == GameState.Walking)
        {
            if (screenTargets.Count != 0)
                SwitchTarget(screenTargets[targetIndex()]);
        }

        if (Input.GetMouseButton(0))
        {
            if (GameManager.Instance.State == GameState.Aiming)
            {
                if (!clickDetection.IsClickOverUI())
                {
                    SwitchTarget(screenTargets[targetIndex()]);
                }
                
            }
        }
    }


    private void SwitchTarget(Transform proposedTarget)
    {
        //if new target is selected
        if (proposedTarget != oldTarget)
        {
            if (IsTargetVisible(proposedTarget))
            {
                target = proposedTarget;
                oldTarget = target;
                if (GameManager.Instance.State == GameState.Aiming)
                    HighlightTarget();
            }
            else
            {
                target = null;
                foreach (Transform t in screenTargets)
                {
                    t.GetComponent<TargetScript>().UnHighlight();
                }
            }
        }
    }

    bool IsTargetTooClose(float distance)
    {
        if (target != null && Vector3.Distance(target.transform.position, transform.position) < distance)
        {
            return true;
        }
        return false;
    }

    private void HighlightTarget()
    {
        foreach (Transform t in screenTargets)
        {
            t.GetComponent<TargetScript>().UnHighlight();
        }
        target.GetComponent<TargetScript>().HighLight();
    }

    private void RequestQuickKill()
    {
        //removed if isnt isLocked
        if (IsTargetTooClose(2) && GameManager.Instance.State == GameState.Walking && !target.GetComponent<TargetScript>().isDead)
        {
            GameManager.Instance.UpdateGameState(GameState.QuickKilling);
        }
    }

public int targetIndex()
    {
        float[] distances = new float[screenTargets.Count];

        for (int i = 0; i < screenTargets.Count; i++)
        {
            distances[i] = Vector2.Distance(Camera.main.WorldToScreenPoint(screenTargets[i].position), new Vector2(Screen.width / 2, Screen.height / 2));
        }

        float minDistance = Mathf.Min(distances);
        int index = 0;

        for (int i = 0; i < distances.Length; i++)
        {
            if (minDistance == distances[i])
                index = i;
        }
        return index;

    }


}
