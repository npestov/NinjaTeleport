using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EndTarget : MonoBehaviour
{

    [SerializeField]
    Transform startPos;

    [SerializeField]
    Transform endPos;

    void Start()
    {
        MoveToEnd();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void MoveToEnd()
    {
        transform.DOMove(new Vector3(endPos.position.x, endPos.position.y, endPos.position.z), 3).SetEase(Ease.Linear).OnComplete(() => MoveToStart());
    }

    void MoveToStart()
    {
        transform.DOMove(new Vector3(startPos.position.x, startPos.position.y, startPos.position.z), 3).SetEase(Ease.Linear).OnComplete(() => MoveToEnd());
    }
}
