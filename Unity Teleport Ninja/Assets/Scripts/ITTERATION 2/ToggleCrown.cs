using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleCrown : MonoBehaviour
{
    public GameObject crown;

    // Start is called before the first frame update
    void Start()
    {
        crown.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //place eveyrone whos ocmpeting in an array
        List<GameObject> runners = new List<GameObject>(GameObject.FindGameObjectsWithTag("Runner"));
        runners.Add(GameObject.FindGameObjectWithTag("Player"));

        Transform furthestRunner = runners[0].transform;
        foreach (GameObject runner in runners)
        {
            if (runner.transform.position.z < furthestRunner.position.z)
            {
                furthestRunner = runner.transform;
            }
        }
        if (furthestRunner.position.z >= transform.position.z - 0.1f)
        {
            crown.SetActive(true);
        }
        else
        {
            crown.SetActive(false);
        }
    }
}
