using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering.PostProcessing;

public class AttackMoveController : MonoBehaviour
{
    public bool isAttackQueued;

    public GameObject enemyToKill;
    private PlayerAnim playerAnim;
    private CameraController cameraController;
    private EndingBonus endingBonus;

    private bool isLocked; //for rotation towards enemy, this stops once teleport happens
    private PostProcessProfile postProfile;

    private Transform bonusTarget;

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

    private Vector3 bonusTargetPos;
    private float maxSwordScale = 1.7f;

    //temp fix
    bool doneWarp;
    Coroutine lastWarpRoutine;

    //HOOKING
    [Space]

    public bool isRunnerSelected;
    public GameObject hookOBJ;
    GameObject currentHook;
    public Transform hookParent;
    //Bonus
    public GameObject bonusSlicer;
    bool bonusThrow;

    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponent<PlayerAnim>();
        cameraController = FindObjectOfType<CameraController>();
        endingBonus = FindObjectOfType<EndingBonus>();
        swordOrigRot = sword.localEulerAngles;
        swordOrigPos = sword.localPosition;
        swordMesh = sword.GetComponentInChildren<MeshRenderer>();
        swordMesh.enabled = true;
        swordShootOffset = new Vector3(0, Y_OFFSET, 5);
        bonusTarget = GameObject.Find("BonusTarget").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.State == GameState.Victory)
            return;

        //Rotate towards target if in process of warping
        /*
        if (isLocked && enemyToKill != null)
            transform.DOLookAt(enemyToKill.transform.position, 2);
        */

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GrapplingHook();
        }
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

    //QUICK SLASH

    //gets called by animator
    public void Slash()
    {
        //transform.DOLookAt(new Vector3(bonusTargetPos.x,transform.position.y, bonusTargetPos.z), 0.3f);
        //isLocked = true;

        //if this is called during the bonus stage
        if (GameManager.Instance.isBonus)
        {

        }
        //else this is called if an enemy is too close to you
        else if (!bonusThrow)
        {
            Warp();
        }
    }

    //QUICK SLASH END

    //WARP START
    public void WarpKill()
    {
        //TEMP FIX
        doneWarp = false;
        transform.DOLookAt(enemyToKill.transform.position, 1);

        GameManager.Instance.UpdateGameState(GameState.Killing);
        Kill();
        isLocked = true;

        //Check weather to quick slash or to do a proper warp
        if (IsEnemyTooCLose())
        {
            lastWarpRoutine = StartCoroutine(BackupWarpAnim(true));
            playerAnim.QuickSlash();
        }
        else
        {
            lastWarpRoutine = StartCoroutine(BackupWarpAnim(false));
            playerAnim.WarpAnim();
        }
    }

    public void Kill()
    {
        //enemyToKill.transform.parent.tag = "KilledTarget";
        //enemyToKill.GetComponentInChildren<TargetScript>().RemoveTarget();

        swordMesh.enabled = true;
        //rotate toward starget
        //transform.DOLookAt(new Vector3(enemyToKill.transform.position.x, transform.position.y, enemyToKill.transform.position.z), 0.2f);
    }
    public void Warp()
    {
        if (GameManager.Instance.State == GameState.Lose)
            return;

        isLocked = false;
        //rotate towards starget
        transform.DOLookAt(new Vector3(enemyToKill.transform.position.x, transform.position.y, enemyToKill.transform.position.z), 0.2f);

        Vector3 tpPos = enemyToKill.transform.position + swordShootOffset;

        ShowBody(false);
        transform.DOMove(tpPos - new Vector3(0, Y_OFFSET, 0), warpDuration).SetEase(Ease.InExpo).OnComplete(() => DoneWarp());

        Vector3 swordTpPos = enemyToKill.transform.position + new Vector3(Random.Range(-0.02f, 0.02f), 1.5f);
        sword.parent = null;
        sword.DOMove(swordTpPos, warpDuration / 1.2f).OnComplete(()=> enemyToKill.GetComponent<Animator>().SetInteger("state", 5));
        sword.DOLookAt(swordTpPos, .2f, AxisConstraint.None);

        //Lens Distortion
        DOVirtual.Float(0, -80, .2f, DistortionAmount);
        DOVirtual.Float(1, 2f, .2f, ScaleAmount);

        enemyToKill.layer = 13;
        GameManager.Instance.UpdateGameState(GameState.Walking);
    }

    //Called as oncomplete in DoTween above
    void DoneWarp()
    {
        //TEMP FIX
        StopCoroutine(lastWarpRoutine);
        doneWarp = true;

        playerAnim.StrikeToHalf();

        sword.parent = swordHand;
        sword.localPosition = swordOrigPos;
        sword.localEulerAngles = swordOrigRot;
        FinishAttack();
        //rotate straight
        DOTween.Kill(transform);
        transform.DORotate(new Vector3(0, -180, 0), 1f);
        enemyToKill.GetComponent<Animator>().SetInteger("state", 5);
        //enemyToKill.GetComponentInChildren<TargetScript>().DeadHighlight();
        enemyToKill.GetComponentInChildren<TargetScript>().DeleteEnemy();
        StartCoroutine(FixSword());
    }
    //WARP END

    //Called as oncomplete in DoTween above


    void FinishAttack()
    {
        ShowBody(true);


        isLocked = false;
        //Shake
        cameraController.ShakeCam();

        //Lens Distortion
        DOVirtual.Float(-80, 0, .2f, DistortionAmount);
        DOVirtual.Float(2f, 1, .1f, ScaleAmount);
    }

    public void ThrowForBonus()
    {
        playerAnim.QuickSlash();
        bonusThrow = true;
        Destroy(GameObject.Find("Slicer"));
        bonusSlicer.SetActive(true);
        //GameManager.Instance.isBonus = false;
        StartCoroutine(ThrowDelay());
    }
    IEnumerator ThrowDelay()
    {
        Transform myBonusTarget = GameObject.Find("BonusTarget").transform;
        yield return new WaitForSeconds(0.3f);
        sword.parent = null;
        sword.DOMove(myBonusTarget.position, Mathf.Sqrt(endingBonus.targetIndex * 1.5f)).SetEase(Ease.InSine).OnComplete(() => BonusComplete());
        sword.DOLookAt(myBonusTarget.position, .2f, AxisConstraint.None);
    }

    void BonusComplete()
    {
        Destroy(sword.gameObject);
        GameManager.Instance.UpdateGameState(GameState.Victory);
    }

    IEnumerator StopParticles()
    {
        yield return new WaitForSeconds(.2f);
        //blueTrail.Stop();
        //whiteTrail.Stop();
    }

    IEnumerator FixSword()
    {
        yield return new WaitForSeconds(.2f);
        sword.parent = swordHand;
        sword.localPosition = swordOrigPos;
        sword.localEulerAngles = swordOrigRot;
        //scale up the sword
        /*
        if (sword.GetChild(1).localScale.x < maxSwordScale)
            sword.GetChild(1).localScale += new Vector3(0.07f, 0.07f, 0.07f);
        */
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

    void GlowAmount(float x)
    {
        SkinnedMeshRenderer[] skinMeshList = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer smr in skinMeshList)
        {
            smr.material.SetVector("_FresnelAmount", new Vector4(x, x, x, x));
        }
    }

    void DistortionAmount(float x)
    {
        postProfile.GetSetting<LensDistortion>().intensity.value = x;
    }
    void ScaleAmount(float x)
    {
        postProfile.GetSetting<LensDistortion>().scale.value = x;
    }

    public void KillSwordTween()
    {
        DOTween.Kill(sword);
        sword.parent = bonusTarget;
        GameManager.Instance.UpdateGameState(GameState.Victory);
    }

    IEnumerator BackupWinState()
    {
        yield return new WaitForSeconds(0.5f);
        if (GameManager.Instance.State != GameState.Victory)
            GameManager.Instance.UpdateGameState(GameState.Victory);
    }
    IEnumerator BackupWarpAnim(bool isQuick)
    {
        if (isLocked && !doneWarp)
        {
            if (isQuick)
            {
                yield return new WaitForSeconds(1f);
                playerAnim.QuickSlash();
            }
            else
            {
                yield return new WaitForSeconds(2f);
                playerAnim.WarpAnim();
            }
        }
    }
}
