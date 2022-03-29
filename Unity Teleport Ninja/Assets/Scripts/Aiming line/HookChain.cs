using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookChain : MonoBehaviour
{
    Vector3 startPoint;
    Vector3 endPoint;
    AttackMoveController attackMoveController;
    LineRenderer line;

    // Start is called before the first frame update
    void Awake()
    {
        attackMoveController = FindObjectOfType<AttackMoveController>();
        line = GetComponent<LineRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        startPoint = attackMoveController.hookParent.position;
        endPoint = transform.parent.position;

        line.SetPosition(0, startPoint);
        line.SetPosition(1, endPoint);
    }
}
