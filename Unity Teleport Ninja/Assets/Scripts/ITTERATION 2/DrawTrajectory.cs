using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTrajectory : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRendererNoTarget;

    [SerializeField]
    private LineRenderer lineRendererWithTarget;

    [SerializeField]
    private LineRenderer lineRendererWithRunner;

    [SerializeField]
    [Range(3, 100)]

    private int lineSegmentCount = 20;

    [SerializeField]
    float amplitude;

    public float yFactor;
    public float xTurningRate;

    private List<Vector3> linePoints = new List<Vector3>();

    private HitDetection hitDetection;

    public GameObject aimPoint;
    private AttackMoveController attackMoveController;




    void Awake()
    {
        hitDetection = FindObjectOfType<HitDetection>();
        attackMoveController = FindObjectOfType<AttackMoveController>();
    }


    public void UpdateTrajectory(Vector3 forceVector, Rigidbody rb, Vector3 startingPoint) {

        Vector3 velocity = (forceVector / rb.mass) * Time.fixedDeltaTime;

        float FlightDuration = (2 * velocity.y) / Physics.gravity.y;

        float stepTime = FlightDuration / lineSegmentCount;

        linePoints.Clear();
        linePoints.Add(startingPoint);

        for (int i = 1; i < lineSegmentCount; i++)
        {
            float stepTimePassed = stepTime * i;
            Vector3 MovementVector = new Vector3(
                velocity.x * -stepTimePassed * xTurningRate,
                velocity.y * stepTimePassed - yFactor * Physics.gravity.y * stepTimePassed * stepTimePassed,
                velocity.y * amplitude * -stepTimePassed
                );

            Vector3 newPointOnLine = -MovementVector + startingPoint;

            RaycastHit hit;
            if (Physics.Raycast(linePoints[i-1], newPointOnLine - linePoints[i-1], out hit, (newPointOnLine - linePoints[i - 1]).magnitude) && hit.transform.gameObject.layer != 11 && hit.transform.gameObject.layer != 6) // 11 is the unhittable layer
            {
                linePoints.Add(hit.point);
                hitDetection.CheckIfHitEnemy(hit);
                break;
            }

            linePoints.Add(newPointOnLine);
        }

        if (!hitDetection.targetSelected && !GameManager.Instance.isBonus)
        {
            lineRendererNoTarget.positionCount = linePoints.Count;
            lineRendererNoTarget.SetPositions(linePoints.ToArray());
            lineRendererNoTarget.gameObject.SetActive(true);
            lineRendererWithTarget.gameObject.SetActive(false);
            lineRendererWithRunner.gameObject.SetActive(false);
        }

        else
        {
            if (attackMoveController.isRunnerSelected)
            {
                lineRendererWithRunner.positionCount = linePoints.Count;
                lineRendererWithRunner.SetPositions(linePoints.ToArray());
                lineRendererWithRunner.gameObject.SetActive(true);
                lineRendererWithTarget.gameObject.SetActive(false);
            }
            else
            {
                lineRendererWithTarget.positionCount = linePoints.Count;
                lineRendererWithTarget.SetPositions(linePoints.ToArray());
                lineRendererWithTarget.gameObject.SetActive(true);
                lineRendererWithRunner.gameObject.SetActive(false);
            }

            lineRendererNoTarget.gameObject.SetActive(false);
        }

        aimPoint.transform.position = linePoints[linePoints.Count / 2];
    }

    public void HideLine()
    {
        lineRendererWithTarget.positionCount = 0;
        lineRendererNoTarget.positionCount = 0;
        lineRendererWithRunner.positionCount = 0;
    }

    public Vector3 GetAimTarget()
    {
        return linePoints[linePoints.Count - 1];
    }


}
