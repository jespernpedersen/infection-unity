using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSingleton : MonoBehaviour
{
    public static SceneSingleton Instance;
    public LevelController level;
    public CanvasController canvas;
    public VirusController virus;
    public TraitsList traitsList = new TraitsList();
    public MutationsList mutationsList = new MutationsList();
    public UpgradesList upgradesList = new UpgradesList();

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance != this && Instance != null)
        {
            Destroy(this);
        } else if (Instance == null)
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
