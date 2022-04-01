using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed;
    Vector3 lastPos;
    private PlayerAnim anim;
    bool isFalling;
    bool fallingActivated;

    //Side to side
    [SerializeField]
    float sideSpeed;
    Vector3 lastClickPos;


    // Start is called before the first frame update
    void Awake()
    {
        anim = FindObjectOfType<PlayerAnim>();
    }

    private void Start()
    {
        lastPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.Instance.isBonus)
        {
            return;
        }

        if (GameManager.Instance.State == GameState.Walking || GameManager.Instance.State == GameState.Aiming)
        {
            Move();
            //transform.position += new Vector3(0, 0, -playerSpeed);
        }
        if (GameManager.Instance.State == GameState.Killing)
        {
            Move();
            //transform.position += new Vector3(0, 0, -playerSpeed/1.2f);
        }


    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastClickPos = Input.mousePosition;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
         if (collision.gameObject.CompareTag("floor"))
        {
            if ((GameManager.Instance.State == GameState.Walking || GameManager.Instance.State == GameState.Aiming) && isFalling)
            {
                fallingActivated = false;
                isFalling = false;
                anim.Run();
            }
        }

         if (collision.gameObject.CompareTag("Obstacle") && GameManager.Instance.State != GameState.Killing)
        {
            GameManager.Instance.UpdateGameState(GameState.Lose);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("floor"))
        {
            if ((GameManager.Instance.State == GameState.Walking || GameManager.Instance.State == GameState.Aiming) && !isFalling)
            {
                isFalling = true;
                StartCoroutine(SwitchToFall());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("bonusWarning"))
        {
            Destroy(other.gameObject);
            transform.DOMoveX(GameObject.Find("BonusTarget").transform.position.x + 0.7f, 0.7f);
        }
    }

    public void IncreaseSpeed()
    {
        playerSpeed += 0.002f;
    }

    IEnumerator SwitchToFall()
    {
        yield return new WaitForSeconds(0.6f);
        if (!fallingActivated && GameManager.Instance.State != GameState.Killing && isFalling)
        {
            fallingActivated = true;
            anim.Falling();
        }
    }

    void Move()
    {
        //Move less if killing
        Vector3 deltaPosition;
        if (GameManager.Instance.State == GameState.Killing)
            deltaPosition = transform.forward * playerSpeed / 1.2f;
        else
            deltaPosition = transform.forward * playerSpeed;

        //SIDE TO SIDE START, comment this
        /*
        if (Input.GetMouseButton(0) && GameManager.Instance.State != GameState.Killing)
        {
            Vector3 touchPosition = Input.mousePosition;
            if (touchPosition.x > lastClickPos.x * 0.5f)
                deltaPosition += transform.right * sideSpeed * Mathf.Sqrt(Mathf.Abs(lastClickPos.x- touchPosition.x));
            else
                deltaPosition -= transform.right * sideSpeed * Mathf.Sqrt(Mathf.Abs(lastClickPos.x - touchPosition.x));
        }
       
        if (deltaPosition.x != float.NaN && deltaPosition.x != float.PositiveInfinity && deltaPosition.x != float.NegativeInfinity)
        { 
            transform.position += deltaPosition * Time.deltaTime;
        }
         */
        //uncomment if not using side
        transform.position += deltaPosition * Time.deltaTime;
        //SIDE TO SIDE END

    }
}
