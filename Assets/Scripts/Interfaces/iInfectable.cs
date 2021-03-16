using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iInfectable
{
    public bool IsInfected { get; }
    public float infectionDuration { get; set; }
    public void Infect();
    public IEnumerator Desinfect();
}
