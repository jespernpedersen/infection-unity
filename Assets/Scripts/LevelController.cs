using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public delegate void TimeCallback(float timeSpeed);
    private List<TimeCallback> timeChangeCallbacks = new List<TimeCallback>();
    private float timeSpeed = 0f;

    public float TimeSpeed
    {
        get{
            return timeSpeed;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeTimeSpeed(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float speed = timeSpeed == 0 ? 1 : 0;
            ChangeTimeSpeed(speed);
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
    }

    public void ChangeTimeSpeed(float speed)
    {
        timeSpeed = speed;
        Time.timeScale = timeSpeed;

        switch (speed)
        {
            case 0:
                SceneSingleton.Instance.canvas.ButtonPause.SetActive(false);
                SceneSingleton.Instance.canvas.ButtonPlay.SetActive(true);
                Camera.main.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case 1:
                SceneSingleton.Instance.canvas.ButtonPause.SetActive(true);
                SceneSingleton.Instance.canvas.ButtonPlay.SetActive(false);
                Camera.main.transform.GetChild(0).gameObject.SetActive(false);
                break;
        }

        foreach(TimeCallback callback in timeChangeCallbacks)
        {
            callback(timeSpeed);
        }
    }

    public void onTimeChange(TimeCallback callback)
    {
        timeChangeCallbacks.Add(callback);
    }
}
