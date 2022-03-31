using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    public bool targetSelected;
    private AttackMoveController attackMoveController;


    private void Awake()
    {
        attackMoveController = FindObjectOfType<AttackMoveController>();
    }

    public void CheckIfHitEnemy(RaycastHit hit)
    {
        if (hit.transform.gameObject.GetComponentInChildren<TargetScript>() != null || hit.transform.gameObject.GetComponent<TargetScript>() != null)
        {
            foreach (TargetScript enemy in FindObjectsOfType<TargetScript>())
            {
                if (enemy.gameObject.layer != 7 && enemy.gameObject.layer != 12)
                    continue;
                enemy.UnHighlight();
            }
            //its an enemy
            if (hit.transform.tag == "Target" && hit.transform.gameObject.layer == 7)
            {
                attackMoveController.isRunnerSelected = false;
            }
            //itds a runner
            if (hit.transform.tag == "Runner")
            {
                attackMoveController.isRunnerSelected = true;
            }
            hit.transform.gameObject.GetComponentInChildren<TargetScript>().HighLight();
            targetSelected = true;
            attackMoveController.enemyToKill = hit.transform.gameObject;

            KillIfPossible();
        }
        else 
        {
            Debug.Log(hit.transform.gameObject.name);
            //uncomment to remove runner from targetting once u dont aim at him
            //attackMoveController.enemyToKill.GetComponentInChildren<TargetScript>().UnHighlight();
            //attackMoveController.enemyToKill = null;
        }
    }

    public void KillIfPossible()
    {
        if (GameManager.Instance.State == GameState.Killing)
            return;

        if (targetSelected && attackMoveController.enemyToKill != null)
        {
            if (!attackMoveController.isRunnerSelected)
            {
                GameManager.Instance.UpdateGameState(GameState.Killing);
                attackMoveController.WarpKill();
                targetSelected = false;
            }
            else
            {
                GameManager.Instance.UpdateGameState(GameState.Killing);
                attackMoveController.GrapplingHook();
                targetSelected = false;
            }
        }
        else
        {
            GameManager.Instance.UpdateGameState(GameState.Walking);
        }
    }
}
