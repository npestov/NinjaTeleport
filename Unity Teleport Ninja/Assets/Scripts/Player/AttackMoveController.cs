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

    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponent<PlayerAnim>();
        cameraController = FindObjectOfType<CameraController>();
        swordOrigRot = sword.localEulerAngles;
        swordOrigPos = sword.localPosition;
        swordMesh = sword.GetComponentInChildren<MeshRenderer>();
        swordMesh.enabled = true;
        swordShootOffset = new Vector3(0, 1, 5);
        bonusTarget = GameObject.Find("BonusTarget").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.State == GameState.Victory)
            return;

        //Rotate towards target if in process of warping
        if (isLocked && enemyToKill != null)
            transform.DOLookAt(enemyToKill.transform.position, 0);

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
            GameManager.Instance.isBonus = false;
            sword.parent = null;
            sword.DOMove(bonusTargetPos, 0.4f);
            sword.DOLookAt(bonusTargetPos, .2f, AxisConstraint.None);
            StartCoroutine(BackupWinState());
        }
        //else this is called if an enemy is too close to you
        else
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
        transform.DOLookAt(new Vector3(enemyToKill.transform.position.x, transform.position.y, enemyToKill.transform.position.z), 0.2f);
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
        transform.DOMove(tpPos, warpDuration).SetEase(Ease.InExpo).OnComplete(() => DoneWarp());

        sword.parent = null;
        sword.DOMove(tpPos, warpDuration / 1.2f);
        sword.DOLookAt(tpPos, .2f, AxisConstraint.None);

        //Lens Distortion
        DOVirtual.Float(0, -80, .2f, DistortionAmount);
        DOVirtual.Float(1, 2f, .2f, ScaleAmount);
    }

    //Called as oncomplete in DoTween above
    void DoneWarp()
    {
        //TEMP FIX
        StopCoroutine(lastWarpRoutine);
        doneWarp = true;

        sword.parent = swordHand;
        sword.localPosition = swordOrigPos;
        sword.localEulerAngles = swordOrigRot;
        FinishAttack();
        //rotate straight
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, -180, transform.eulerAngles.z);

        enemyToKill.layer = 0;
        enemyToKill.GetComponent<Animator>().SetInteger("state", 5);
        enemyToKill.GetComponentInChildren<TargetScript>().DeadHighlight();
        enemyToKill.GetComponentInChildren<TargetScript>().DeleteEnemy();
        StartCoroutine(FixSword());

        GameManager.Instance.UpdateGameState(GameState.Walking);
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

    public void ThrowForBonus(Vector3 targetPos)
    {
        bonusTargetPos = targetPos;
        playerAnim.QuickSlash();
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
