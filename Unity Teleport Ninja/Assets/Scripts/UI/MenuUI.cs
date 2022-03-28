using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuUI : MonoBehaviour
{
    private GameObject container;
    [SerializeField]
    private TextMeshProUGUI txtLevel;

    [SerializeField]
    private TextMeshProUGUI txtCoins;

    // Start is called before the first frame update
    void Awake()
    {
        container = gameObject.transform.GetChild(0).gameObject;
    }
    private void Start()
    {
        txtLevel.text = "LEVEL: " + PlayerPrefs.GetInt("lvl");
        txtCoins.text = PlayerPrefs.GetInt("coins").ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RemoveMenu()
    {
        container.SetActive(false);
    }
}
