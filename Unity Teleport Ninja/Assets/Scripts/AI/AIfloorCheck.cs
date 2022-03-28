using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIfloorCheck : MonoBehaviour
{
    private AImovement aImovement;

    // Start is called before the first frame update
    void Awake()
    {
        aImovement = GetComponentInParent<AImovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("floor"))
        {
            if (!aImovement.isThrowingSword)
            {
                aImovement.FindClosestTarget(false);
                aImovement.ThrowSword();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            aImovement.FindClosestTarget(false);
            aImovement.ThrowSword();
        }
    }

}
