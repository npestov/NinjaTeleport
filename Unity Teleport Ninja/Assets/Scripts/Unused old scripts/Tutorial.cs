using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public GameObject doubleTapTip;
    public GameObject walkBehindTip;
    public GameObject comboTip;

    private bool doubleTapTipShown;
    private bool walkBehindShown;
    private bool comboShown;

    private void OnEnable()
    {
        Debug.Log("xistsa");
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }
    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }
    private void GameManagerOnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Walking:
                comboTip.SetActive(false);
                if (!doubleTapTipShown)
                {
                    doubleTapTipShown = true;
                    StartCoroutine(DoubleTapTip());
                }
                break;
            case GameState.Aiming:
                doubleTapTip.SetActive(false);
                break;
            case GameState.QuickKilling:
                walkBehindTip.SetActive(false);
                break;
            case GameState.Killing:
                if (!comboShown)
                {
                    Time.timeScale = 0.05f;
                    Time.fixedDeltaTime = Time.timeScale * .02f;
                    comboShown = true;
                    comboTip.SetActive(true);
                }
                break;
            case GameState.Lose:
                break;
        }
    }

    // Start is called before the first frame update
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!walkBehindShown)
                ShowWalkingTip();
        }

        if (GameManager.Instance.State == GameState.Killing){
            if (Input.GetMouseButtonDown(0))
            {
                Time.timeScale = 1;
            }
        }
    }

    // Update is called once per frame

    IEnumerator DoubleTapTip()
    {
        yield return new WaitForSeconds(0.5f);
        doubleTapTip.SetActive(true);
    }

    private void ShowWalkingTip()
    {
        walkBehindTip.SetActive(true);
        walkBehindShown = true;
    }
}
