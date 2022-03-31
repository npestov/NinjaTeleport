using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTwo : MonoBehaviour
{
    public GameObject swipeTip;
    //public GameObject releaseTip;

    private AttackMoveController attackMoveController;
    bool shownSwipe;
    bool shownRelease;


    // Start is called before the first frame update
    void Awake()
    {
        attackMoveController = FindObjectOfType<AttackMoveController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!shownSwipe)
        {
            if (Input.GetMouseButtonDown(0))
            {
                shownSwipe = true;
                StartCoroutine(ShowSwipeTip());
            }
        }
        else
        {
            if (attackMoveController.enemyToKill != null)
            {
                if (!shownRelease)
                {
                    swipeTip.SetActive(false);
                    shownRelease = true;
                    //releaseTip.SetActive(true);
                }
                if (shownRelease && Input.GetMouseButtonUp(0))
                {
                    //releaseTip.SetActive(false);
                    Time.timeScale = 1f;
                }
            }
        }

    }

    IEnumerator ShowSwipeTip()
    {
        yield return new WaitForSeconds(1);
        swipeTip.SetActive(true);
        //Time.timeScale = 0.2f;
    }


}
