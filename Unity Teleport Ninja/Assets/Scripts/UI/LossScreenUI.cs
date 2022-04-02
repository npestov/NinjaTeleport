using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LossScreenUI : MonoBehaviour
{
    [SerializeField]
    private Button btnRestart;
    [SerializeField]
    private Button btnRestartTwo;
    private GameObject container;
    private GameObject containerFisnishedLast;

    // Start is called before the first frame update
    void Start()
    {
        btnRestart.onClick.AddListener(RestartClicked);
        btnRestartTwo.onClick.AddListener(RestartClicked);
        container = gameObject.transform.GetChild(0).gameObject;
        containerFisnishedLast = gameObject.transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            RestartClicked();
        }
    }

    void RestartClicked()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        Destroy(btnRestart.gameObject);
        Destroy(btnRestartTwo.gameObject);
    }

    public IEnumerator DisplayLossScreen()
    {
        yield return new WaitForSeconds(2);
        container.SetActive(true);
    }

    public IEnumerator DislayFinishFail()
    {
        yield return new WaitForSeconds(0.3f);
        containerFisnishedLast.SetActive(true);
    }
}
