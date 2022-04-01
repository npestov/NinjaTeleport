using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
public class ChaserEnemy : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;              
    public float speedRun = 9;
    private Animator anim;
    private TargetScript targetScript;
    private GameObject player;   
    Vector3 m_PlayerPosition;
    public Transform sword;
    private Vector3 swordShootOffset;

    void Start()
    {
        targetScript = GetComponentInChildren<TargetScript>();
        m_PlayerPosition = Vector3.zero;
        player = GameObject.FindGameObjectWithTag("Player");              
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedRun;
        swordShootOffset = new Vector3(0, 1, 0);
    }

    private void Update()
    {
        if (targetScript.isDead || !GameManager.Instance.levelStarted || GameManager.Instance.State == GameState.Lose)
            return;

        Chasing();

        if (IsPlayerTooClose() && GameManager.Instance.State != GameState.Killing)
        {
            SlashPlayer();
        }
    }

    private void Chasing()
    {
        m_PlayerPosition = player.transform.position;
        if (CanReachPosition(m_PlayerPosition) && gameObject.layer != 13)
        {
            Move(speedRun);
            anim.SetInteger("state", 6);
            navMeshAgent.SetDestination(m_PlayerPosition);
        }
        else if (gameObject.layer != 13)
        {
            anim.SetInteger("state", 7);
        }
    }

    public bool CanReachPosition(Vector3 position)
    {
        NavMeshPath path = new NavMeshPath();
        navMeshAgent.CalculatePath(position, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }

    private void SlashPlayer()
    {
        transform.DOLookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z), 0.1f);
        sword.parent = null;
        sword.DOMove(player.transform.position + swordShootOffset, 0.4f).OnComplete(()=> sword.gameObject.SetActive(false));
        sword.DOLookAt(player.transform.position + swordShootOffset, .2f, AxisConstraint.None);
        anim.SetInteger("state", 7);
        navMeshAgent.isStopped = true;
        GameManager.Instance.UpdateGameState(GameState.Lose);
        
    }


    bool IsPlayerTooClose()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < 2 && GameManager.Instance.State != GameState.Killing && transform.gameObject.layer != 13)
        {
            return true;
        }
        return false;
    }

    void Move(float speed)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
    }

   
    
}