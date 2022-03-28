using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AImovement : MonoBehaviour
{
    PlayerMovement playerMovement;
    private Animator anim;

    GameObject player;
    Vector3 playerPos;

    private float timer;
    [SerializeField]
    private float timeToTp = 5;

    Transform closestTarget;

    [SerializeField]
    int timesToOvertake;
    private int timesOvertaken = 0;

    public Transform sword;
    private Vector3 swordShootOffset;
    public Transform swordHand;
    private Vector3 swordOrigRot;
    private Vector3 swordOrigPos;
    public bool isThrowingSword;

    private bool isStopped;
    private Rigidbody rb;


    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = FindObjectOfType<PlayerMovement>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        swordShootOffset = new Vector3(0, 1, 0);
        swordOrigRot = sword.localEulerAngles;
        swordOrigPos = sword.localPosition;
    }
    private void Update()
    {
        //increase timer
        if (GameManager.Instance.State != GameState.Menu && GameManager.Instance.State != GameState.Victory && !isStopped)
            timer += Time.deltaTime;

        //too close infornt of player
        if (player.transform.position.z > transform.position.z +1.5f && Vector3.Distance(player.transform.position,transform.position) < 4)
        {
            rb.AddForce(transform.forward * Random.Range(20,100));
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        playerPos = player.transform.position;
        //Running
        if (GameManager.Instance.State != GameState.Menu && GameManager.Instance.State != GameState.Victory && !isStopped)
        {
            transform.position += new Vector3(0, 0, -playerMovement.playerSpeed);
            anim.SetInteger("state", 6);
        }
        if (timer < timeToTp || timesToOvertake < timesOvertaken)
            return;

        FindClosestTarget(false);
        ThrowSword();
        timesOvertaken++;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("finish"))
        {
            //Destroy(other.gameObject);
            isStopped = true;
            GameManager.Instance.enemyHasWon = true;
            anim.SetInteger("state", 1);
        }
    }
    

    public void ThrowSword()
    {
        if (closestTarget.gameObject.name != "LastEnemy")
        {
            SwordToTarget();
            timer = 0;
        }
    }

    private void Teleport()
    {
        transform.position = closestTarget.position;
        //fix sword
        StartCoroutine(FixSword());
        //look straight
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, -180, transform.eulerAngles.z);

    }

    public void FindClosestTarget(bool teleportFurther)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
        closestTarget = GameObject.FindGameObjectsWithTag("bonus")[0].transform;
        foreach (GameObject target in targets)
        {
            if (Vector3.Distance(target.transform.position, transform.position) < Vector3.Distance(closestTarget.position, transform.position))
            {
                //if target behind then continue
                if (target.transform.position.z > transform.position.z)
                {
                    continue;
                }
                else
                //only teleport further than player with the timer tp
                if (teleportFurther && Vector3.Distance(closestTarget.transform.position, transform.position) < Vector3.Distance(playerPos, transform.position))
                    continue;

                closestTarget = target.transform;
            }

        }
        isThrowingSword = true;
    }

    private void SwordToTarget()
    {
        //transform.DOLookAt(new Vector3(closestTarget.transform.position.x, transform.position.y, closestTarget.transform.position.z), 0.1f);
        sword.parent = null;
        sword.DOMove(closestTarget.transform.position + swordShootOffset, 0.4f).OnComplete(() => sword.gameObject.SetActive(false)).OnComplete(()=>Teleport()
        );
        sword.DOLookAt(closestTarget.transform.position + swordShootOffset, .2f, AxisConstraint.None);
        //anim.SetInteger("state", 7);
    }

    IEnumerator FixSword()
    {
        yield return new WaitForSeconds(.3f);
        isThrowingSword = false;
        sword.parent = swordHand;
        sword.localPosition = swordOrigPos;
        sword.localEulerAngles = swordOrigRot;
    }

    public void ResetTimer()
    {
        timer = 0;
    }

}
