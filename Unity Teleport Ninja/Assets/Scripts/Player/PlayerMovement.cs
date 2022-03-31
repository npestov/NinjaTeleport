using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed;
    Vector3 lastPos;
    private PlayerAnim anim;
    bool isFalling;
    bool fallingActivated;

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
        if (Input.GetKey(KeyCode.S))
        {
            return;
        }

        if (GameManager.Instance.State == GameState.Walking || GameManager.Instance.State == GameState.Aiming)
        {
            transform.position += new Vector3(0, 0, -playerSpeed);
        }
        if (GameManager.Instance.State == GameState.Killing)
        {
            transform.position += new Vector3(0, 0, -playerSpeed/1.2f);
        }


    }
    private void Update()
    {

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
                Debug.Log("switched to run");
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
                Debug.Log("switched fall called");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("bonusWarning"))
        {
            Destroy(other.gameObject);
            transform.DOMoveX(GameObject.Find("BonusTarget").transform.position.x, 1);
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
            Debug.Log("actually switched to fall");
            fallingActivated = true;
            anim.Falling();
        }
    }
}
