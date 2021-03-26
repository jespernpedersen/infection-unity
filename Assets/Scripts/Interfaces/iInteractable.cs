using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iInteractable 
{
    public Interactable objectType { get; }
    public bool Interact(iInfectable trigger = null);

}
