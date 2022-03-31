using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCOllection : MonoBehaviour
{
    public int coinsEarned;
    private AttackMoveController attackMoveController;
    public float bonusMultiplier = 1;

    // Start is called before the first frame update
    void Awake()
    {
        attackMoveController = FindObjectOfType<AttackMoveController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("coin"))
        {
            coinsEarned++;
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("finish"))
        {
            Destroy(other.gameObject);
            GameManager.Instance.FinishedTheTrack();
        }

        if (other.gameObject.CompareTag("bonus"))
        {

            //old bonus target
            /*
            attackMoveController.enemyToKill = null;
            attackMoveController.KillSwordTween();
            if (float.Parse(other.gameObject.name) > bonusMultiplier)
            {
                bonusMultiplier = float.Parse(other.gameObject.name);
            }
            */
        }

    }
}
