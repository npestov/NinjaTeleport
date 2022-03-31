using UnityEngine;
using Cinemachine;


public class TimeManager : MonoBehaviour
{
	public static TimeManager Instance;
	public float slowdownFactor = 0.05f;
	public float slowdownLength = 2f;

    private void Start()
    {
		Instance = this;
	}
    public void DoSlowmotion()
	{
		/*
		if (FindObjectsOfType<TutorialTwo>().Length == 0)
        {
			Time.timeScale = slowdownFactor;
			Time.fixedDeltaTime = Time.timeScale * .02f;
		}
		*/
	}
	public void DoBonusSlowMotion()
    {

		//Time.timeScale = 0.1f;
		//Time.fixedDeltaTime = Time.timeScale * .02f;
	}

	public void RemoveSlowMotion()
    {
		if (FindObjectsOfType<TutorialTwo>().Length == 0)
			Time.timeScale = 1;
	}

}