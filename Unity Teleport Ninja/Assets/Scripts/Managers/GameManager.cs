using System;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Instances
    private CameraController cameraController;
    private PlayerMovement playerMovement;
    private AttackMoveController attackMoveController;
    private TimeManager timeManager;
    private ThirdPersonInput thirdPersonInput;
    private InGameUI inGameUI;
    private PlayerAnim playerAnim;
    private WinScreenUI winScreenUI;
    private LossScreenUI lossScreenUI;
    private MenuUI menuUI;
    private ClickDetection clickDetection;

    public bool levelStarted;
    public bool isBonus;
    public bool dontRagdoll;
    private bool isStarting = false;
    public bool enemyHasWon; 

    //GM
    public static GameManager Instance;
    public GameState State;
    public static event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        Instance = this;
        clickDetection = FindObjectOfType<ClickDetection>();
        menuUI = FindObjectOfType<MenuUI>();
        lossScreenUI = FindObjectOfType<LossScreenUI>();
        winScreenUI = FindObjectOfType<WinScreenUI>();
        playerAnim = FindObjectOfType<PlayerAnim>();
        cameraController = FindObjectOfType<CameraController>();
        attackMoveController = FindObjectOfType<AttackMoveController>();
        timeManager = FindObjectOfType<TimeManager>();
        thirdPersonInput = FindObjectOfType<ThirdPersonInput>();
        inGameUI = FindObjectOfType<InGameUI>();
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    private void Start()
    {
        UpdateGameState(GameState.Menu);
    }


    private void Update()
    {
        CheckForStartingTouch();
    }

    public void UpdateGameState(GameState newState)
    {

        switch (newState)
        {
            case GameState.Walking:
                cameraController.SwitchToWalk();
                timeManager.RemoveSlowMotion();
                break;
            case GameState.Aiming:
                cameraController.SwitchToAim();
                timeManager.DoSlowmotion();
                break;
            case GameState.Killing:
                timeManager.RemoveSlowMotion();
                cameraController.SwitchToWalk();
                break;
            case GameState.QuickKilling:
                break;
            case GameState.Victory:
                if (State == GameState.Victory)
                    break;
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, PlayerPrefs.GetInt("lvl").ToString());

                inGameUI.ShowMultiplier();
                cameraController.SwitchToWalk();
                playerAnim.Idle();
                StartCoroutine(winScreenUI.DisplayWinScreen());
                break;
            case GameState.Lose:
                if (State == GameState.Lose)
                    break;
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, PlayerPrefs.GetInt("lvl").ToString());

                if (enemyHasWon)
                {
                    StartCoroutine(lossScreenUI.DislayFinishFail());
                }
                else
                {
                    playerAnim.Die();
                    StartCoroutine(lossScreenUI.DisplayLossScreen());
                }
                playerAnim.Idle();
                timeManager.RemoveSlowMotion();
                //inGameUI.DisableInGameUI();
                break;
        }

        State = newState;
        OnGameStateChanged?.Invoke(newState);
    }

    public bool AreEnemiesDead()
    {
        if (GameObject.FindGameObjectsWithTag("Target").Length == 0)
        {
            GameManager.Instance.UpdateGameState(GameState.Victory);
            return true;
        }
        return false;
    }

    private void CheckForStartingTouch()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            if (State == GameState.Menu && !isStarting)
            {
                if (!clickDetection.IsClickOverUI())
                {
                    GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, PlayerPrefs.GetInt("lvl").ToString());
                    cameraController.DisableMenuCam();
                    menuUI.RemoveMenu();
                    isStarting = true;
                    playerAnim.Run();
                    levelStarted = true;
                    State = GameState.Walking;
                }
            }
        }
        
    }

    public void FinishedTheTrack()
    {
        if (!enemyHasWon)
        {
            isBonus = true;
            attackMoveController.ThrowForBonus();
            cameraController.SwitchToSwordCam();
            timeManager.DoBonusSlowMotion();
        }
        else
        {
            UpdateGameState(GameState.Lose);
        }
    }
}

public enum GameState
{
    Menu,
    Walking,
    Aiming,
    Killing,
    QuickKilling,
    Victory,
    Lose
}
