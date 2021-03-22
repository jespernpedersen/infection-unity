using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iInfectable
{
    public bool IsInfected { get; }
    public void Infect(float duration = -1);
    public IEnumerator Desinfect(float waitBeforeDesinfect = -1);
}
