using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    private float timeSpeed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = timeSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
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
                CanvasSingleton.Instance.ButtonPause.SetActive(false);
                CanvasSingleton.Instance.ButtonPlay.SetActive(true);
                break;
            case 1:
                CanvasSingleton.Instance.ButtonPause.SetActive(true);
                CanvasSingleton.Instance.ButtonPlay.SetActive(false);
                break;
        }
    }
}
