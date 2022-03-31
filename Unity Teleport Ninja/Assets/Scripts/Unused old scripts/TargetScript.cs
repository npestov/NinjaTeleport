using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMeshRenderer;
    public Material highlightedColor;
    private Material usualColor;
    public Material blackColor;
    private PlayerVision playerVision;
    private EnemyGun enemyGun;
    [HideInInspector]
    public bool isDead = false;

    void Start()
    {
        playerVision = FindObjectOfType<PlayerVision>();
        enemyGun = transform.parent.GetComponentInChildren<EnemyGun>();
        skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
        usualColor = skinnedMeshRenderer.material;
        //AddTarget();
    }

    public void AddTarget()
    {
        if (!playerVision.screenTargets.Contains(transform))
            playerVision.screenTargets.Add(transform);
    }

    public void RemoveTarget()
    {
        if (playerVision.screenTargets.Contains(transform))
            playerVision.screenTargets.Remove(transform);
    }

    public void HighLight()
    {
         skinnedMeshRenderer.material = highlightedColor;
    }
    public void UnHighlight()
    {
        if (!isDead && skinnedMeshRenderer != null)
            skinnedMeshRenderer.material = usualColor;
    }

    public void DeadHighlight()
    {
        isDead = true;
        skinnedMeshRenderer.material = blackColor;
    }

    public void DeleteEnemy()
    {
        CreatePlaceholder();
        StartCoroutine(KillMyself());
    }
    private void CreatePlaceholder()
    {
        gameObject.tag = "Untagged";
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.layer = 11;
        cube.GetComponent<MeshRenderer>().enabled = false;
        cube.GetComponent<Collider>().enabled = false;
        cube.transform.position = transform.position;
        cube.tag = "Target";
    }

    IEnumerator KillMyself()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject.transform.parent.gameObject);
    }
}
