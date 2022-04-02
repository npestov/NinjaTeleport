using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class WinScreenUI : MonoBehaviour
{
    [SerializeField]
    private Button btnNext;
    private GameObject container;
    [SerializeField]
    private TextMeshProUGUI addedCoins;
    private CoinCOllection coinCOllection;
    bool displayedOnce;
    EndingBonus endingBonus;

    // Start is called before the first frame update
    void Awake()
    {
        btnNext.onClick.AddListener(NextClicked);
        container = gameObject.transform.GetChild(0).gameObject;
        coinCOllection = FindObjectOfType<CoinCOllection>();
        endingBonus = FindObjectOfType<EndingBonus>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void NextClicked()
    {
        PlayerPrefs.SetFloat("sharpness", PlayerPrefs.GetFloat("sharpness") + 1);
        //this is so that the levels loop
        int levelIndex = (PlayerPrefs.GetInt("lvl") + 1);
        if (levelIndex == 1)
        {
            PlayerPrefs.SetInt("lvl", 2);
            levelIndex = 2;
        }
        if (levelIndex % SceneManager.sceneCountInBuildSettings == 0)
        {
            PlayerPrefs.SetInt("lvl", PlayerPrefs.GetInt("lvl") + 3);
            levelIndex = (PlayerPrefs.GetInt("lvl") + 3);
        }
        SceneManager.LoadSceneAsync(levelIndex % SceneManager.sceneCountInBuildSettings);
        PlayerPrefs.SetInt("lvl", levelIndex);
        Destroy(btnNext.gameObject);
    }

    public IEnumerator DisplayWinScreen()
    {
        if (!displayedOnce)
        {
            displayedOnce = true;
            yield return new WaitForSeconds(1);

            if (GameObject.FindObjectsOfType<InGameUI>().Length != 0)
                FindObjectOfType<InGameUI>().gameObject.SetActive(false);

            float multiplier = endingBonus.finalMultiplier;
            int coinsEarned = coinCOllection.coinsEarned;
            if (multiplier < 1)
                multiplier = 1;
            addedCoins.text = coinsEarned.ToString() + " X " + multiplier + " = " + (int)(coinsEarned * multiplier);
            PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") + (int)((coinsEarned * multiplier)));
            container.SetActive(true);
        }

    }
}
