using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    
    public GameObject TimeControl;
    public GameObject ButtonPause;
    public GameObject ButtonPlay;

    public GameObject ObjectivesList;
    public GameObject ObjectivePrefab;

    public GameObject LevelComplete;
    public GameObject LevelFailed;

    public void CloseInterfaces()
    {
        TimeControl.SetActive(false);
        ObjectivesList.SetActive(false);
        LevelComplete.SetActive(false);
        //LevelFailed.SetActive(false); - Not yet implemented
        
    }

}
