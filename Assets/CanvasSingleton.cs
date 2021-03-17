using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSingleton : MonoBehaviour
{
    public static CanvasSingleton Instance;
    public GameObject ButtonPause;
    public GameObject ButtonPlay;

    void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else if (Instance == this) return;

        Instance = this;

    }
}
