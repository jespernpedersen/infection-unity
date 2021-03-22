using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusController : MonoBehaviour
{
    public float points;
    public float growth;
    public float infectCount;
    public float surfaceLifespan = 10f;
    public List<Mutation> MutationsList = new List<Mutation>(0); 

    // Start is called before the first frame update
    void Start()
    {    
        MutationsList.Add(new Mutation(0, "My First Mutation", "This evolves", 2));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
