using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusController : MonoBehaviour
{
    public float points;
    public float growth;
    public float infectCount;

    public List<Mutation> mutations;
    public List<Upgrade> upgrades;

    private void Start()
    {
        // make a surface spreadable virus
        mutations.Add(SceneSingleton.Instance.mutationsList.mutations[0]);
        mutations.Add(SceneSingleton.Instance.mutationsList.mutations[2]);
    }

}
