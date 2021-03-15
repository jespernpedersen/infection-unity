using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusProperties : MonoBehaviour
{
    public float points;
    public float growth;
    public float infectCount;
    public GameObject[] VirusMutations; 

    // Start is called before the first frame update
    void Start()
    {    
        VirusMutations = GameObject.FindGameObjectsWithTag("mutation");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
