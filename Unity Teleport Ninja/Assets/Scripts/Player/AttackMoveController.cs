using System.Collections;
using UnityEngine;
using DG.Tweening;

public class AttackMoveController : MonoBehaviour
{
    public bool isAttackQueued;

    public GameObject enemyToKill;
    private PlayerAnim playerAnim;
    private CameraController cameraController;
    private EndingBonus endingBonus;

    [Space]
    public float warpDuration = .5f;

    [Space]
    //SWORD
    public Transform sword;
    public Transform swordHand;
    private Vector3 swordOrigRot;
    private Vector3 swordOrigPos;
    private MeshRenderer swordMesh;
    private Vector3 swordShootOffset;
    private float Y_OFFSET = 1;

    //HOOKING
    [Space]

    public bool isRunnerSelected;
    public GameObject hookOBJ;
    GameObject currentHook;
    public Transform hookParent;
    //Bonus
    public GameObject bonusSlicer;
    Rigidbody rb;

    // Start is called before the first frame update
    void Awake()
    {
        playerAnim = GetComponent<PlayerAnim>();
        cameraController = FindObjectOfType<CameraController>();
        endingBonus = FindObjectOfType<EndingBonus>();
        swordMesh = sword.GetComponentInChildren<MeshRenderer>();
    }
    void Start()
    {
        swordOrigRot = sword.localEulerAngles;
        swordOrigPos = sword.localPosition;
        swordMesh.enabled = true;
        swordShootOffset = new Vector3(0, Y_OFFSET, 0);
    }

    //HOOK
    public void GrapplingHook()
    {
        GameManager.Instance.UpdateGameState(GameState.Walking);
        enemyToKill.GetComponentInChildren<TargetScript>().UnHighlight();
        enemyToKill.GetComponent<AImovement>().ResetTimer();
        currentHook = Instantiate(hookOBJ, hookParent.transform.position, Quaternion.Euler(0, 0, 0));
        currentHook.transform.DOMove(enemyToKill.transform.position + swordShootOffset, 0.4f).SetEase(Ease.Linear).OnComplete(() => PullHook());
        currentHook.transform.DOLookAt(enemyToKill.transform.position, .2f, AxisConstraint.None);
    }
    private void PullHook()
    {
        enemyToKill.transform.parent = currentHook.transform;
        currentHook.transform.DOMove(hookParent.transform.position + new Vector3(0, 0, 10), 0.2f).SetEase(Ease.Linear).OnComplete(() => FinishedHook());
    }
    private void FinishedHook()
    {
        enemyToKill.transform.parent = GameObject.Find("other runners").transform;
        Destroy(currentHook);
        enemyToKill = null;
    }
    //END HOOK


    //WARP START
    public void WarpKill()
    {
        //TEMP FIX
        transform.DOLookAt(enemyToKill.transform.position, 1);

        GameManager.Instance.UpdateGameState(GameState.Killing);
        swordMesh.enabled = true;

        playerAnim.ResetAnims();
        //Check weather to quick slash or to do a proper warp
        if (IsEnemyTooCLose())
        {
            playerAnim.QuickSlash();
        }
        else
        {
            playerAnim.WarpAnim();
        }
    }

    public void Warp()
    {
        if (GameManager.Instance.State == GameState.Lose || GameManager.Instance.isBonus)
            return;

        //rotate towards starget
        transform.DOLookAt(new Vector3(enemyToKill.transform.position.x, transform.position.y, enemyToKill.transform.position.z), 0.2f);

        Vector3 swordTpPos = enemyToKill.transform.position + swordShootOffset;
        // + new Vector3(Random.Range(-0.02f, 0.02f), 1.5f);
        sword.parent = null;
        sword.DOMove(swordTpPos, warpDuration / 1.2f).OnComplete(() => SwordDoneFlying());
        sword.DOLookAt(swordTpPos, .2f, AxisConstraint.None);

        Vector3 tpPos = enemyToKill.transform.position + new Vector3(0, Y_OFFSET, 5);

        ShowBody(false);
        transform.DOMove(tpPos - new Vector3(0, Y_OFFSET, 0), warpDuration).SetEase(Ease.InExpo).OnComplete(() => DoneWarp());

        enemyToKill.layer = 13;
        GameManager.Instance.UpdateGameState(GameState.Walking);

    }

    void DoneWarp()
    {
        //TEMP FIX
        playerAnim.StrikeToHalf();
        ShowBody(true);
        //rotate straight
        DOTween.Kill(transform);
        transform.DORotate(new Vector3(0, -180, 0), 1f);
        enemyToKill.GetComponent<Animator>().SetInteger("state", 5);
        enemyToKill.GetComponentInChildren<TargetScript>().DeleteEnemy();

        StartCoroutine(FixSword());
        cameraController.ShakeCam();
    }

    void SwordDoneFlying()
    {
        sword.DOMoveZ(sword.transform.position.x - 3, 0.1f).OnComplete(() => SwordBackInHand());
        sword.DORotate(new Vector3(0, -180, 0), 0.05f);
    }
    void SwordBackInHand()
    {
        sword.parent = swordHand;
        sword.localPosition = swordOrigPos;
        sword.localEulerAngles = swordOrigRot;
    }

    //WARP END

    public void ThrowForBonus()
    {
        playerAnim.QuickSlash();
        Destroy(GameObject.Find("Slicer"));
        bonusSlicer.SetActive(true);
        StartCoroutine(ThrowDelay());
    }

    void BonusComplete()
    {
        Destroy(sword.gameObject);
        GameManager.Instance.UpdateGameState(GameState.Victory);
    }

    private bool IsEnemyTooCLose()
    {
        if (Vector3.Distance(transform.position, enemyToKill.transform.position) < 15)
            return true;

        return false;
    }


    void ShowBody(bool state)
    {
        SkinnedMeshRenderer[] skinMeshList = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer smr in skinMeshList)
        {
            smr.enabled = state;
        }
    }

    IEnumerator FixSword()
    {
        yield return new WaitForSeconds(.2f);
        sword.parent = swordHand;
        sword.localPosition = swordOrigPos;
        sword.localEulerAngles = swordOrigRot;
    }

    IEnumerator ThrowDelay()
    {
        Transform myBonusTarget = GameObject.Find("BonusTarget").transform;
        yield return new WaitForSeconds(0.3f);
        sword.parent = null;
        sword.DOMove(myBonusTarget.position, Mathf.Sqrt(endingBonus.targetIndex * 1.5f)).SetEase(Ease.InOutSine).OnComplete(() => BonusComplete());
        sword.DOLookAt(myBonusTarget.position, .2f, AxisConstraint.None);
    }
}
