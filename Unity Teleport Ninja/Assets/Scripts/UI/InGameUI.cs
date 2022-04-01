using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUI : MonoBehaviour
{
    private GameObject container;
    [SerializeField]
    private GameObject killScreen;
    [SerializeField]
    private GameObject other;
    [SerializeField]
    private TextMeshProUGUI coins;

    public Button killButton;
    public Button cancelKillButton;
    public DynamicJoystick mainJoystick;
    private AttackMoveController attackMoveController;

    public TextMeshProUGUI multiplierTxt;



    private Button btnQue;


    // Start is called before the first frame update
    void Start()
    {
        container = gameObject.transform.GetChild(0).gameObject;
        attackMoveController = FindObjectOfType<AttackMoveController>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void EnableInGameUI()
    {
        other.SetActive(true);
    }
    public void DisableInGameUI()
    {
        other.SetActive(false);
    }

    public void DisableJoystick()
    {
        mainJoystick.gameObject.SetActive(false);
    }


    public void ShowMultiplier()
    {
        StartCoroutine(Mult());
    }

    IEnumerator Mult()
    {
        yield return new WaitForSeconds(0.1f);
        multiplierTxt.text = "X" + FindObjectOfType<EndingBonus>().finalMultiplier;
    }
}
