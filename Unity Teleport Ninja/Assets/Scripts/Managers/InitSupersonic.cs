using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.SceneManagement;
using Facebook.Unity;

public class InitSupersonic : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
        GameAnalytics.Initialize();
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "World_01", "Stage_01", "Level_Progress");

    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
            TestEvent();
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }

        int sceneToLoad = PlayerPrefs.GetInt("lvl", 1) % SceneManager.sceneCountInBuildSettings;
        if (sceneToLoad == 0)
            PlayerPrefs.SetInt("lvl", PlayerPrefs.GetInt("lvl") + 1);

        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    private void TestEvent()
    {
        var softPurchaseParameters = new Dictionary<string, object>();
        softPurchaseParameters["mygame_purchased_item"] = "bag";
        FB.LogAppEvent(
          Facebook.Unity.AppEventName.SpentCredits,
          (float)100,
          softPurchaseParameters
        );

    }
}
