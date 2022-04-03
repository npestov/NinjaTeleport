using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingBonus : MonoBehaviour
{
    [SerializeField]
    private GameObject[] allBonus;
    private GameObject bonusTarget;
    [HideInInspector]
    public float finalMultiplier;
    [HideInInspector]
    public int targetIndex;
    // Start is called before the first frame update

    void Start()
    {
        //PlayerPrefs.SetFloat("sharpness", 1010000);
        allBonus = GameObject.FindGameObjectsWithTag("bonus");
        bonusTarget = GameObject.Find("BonusTarget");

        SortByDistance();
        PlaceBonusTarget();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SortByDistance()
    {
        for (int i = 1; i < allBonus.Length; i++)
        {
            int j = i;
            while (j > 0 && IsFirstCloser(allBonus[j], allBonus[j - 1]))
            {
                GameObject temp = allBonus[j - 1];
                allBonus[j - 1] = allBonus[j];
                allBonus[j] = temp;
                j--;
            }
        }
    }

    bool IsFirstCloser(GameObject firstObj, GameObject secObj)
    {
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        if (Vector3.Distance(firstObj.transform.position, playerPos) < Vector3.Distance(secObj.transform.position, playerPos))
            return true;

        return false;
    }

    void PlaceBonusTarget()
    {
        float swordSharpness = PlayerPrefs.GetFloat("sharpness");
        targetIndex = Mathf.FloorToInt(swordSharpness);
        if (targetIndex >= allBonus.Length)
        {
            targetIndex = allBonus.Length - 1;
        }
        GameObject myTargetEnemey = allBonus[targetIndex];
        bonusTarget.transform.position = myTargetEnemey.transform.position + new Vector3(0,1,0);

        finalMultiplier = targetIndex * 0.2f + 1.2f;
    }


}
