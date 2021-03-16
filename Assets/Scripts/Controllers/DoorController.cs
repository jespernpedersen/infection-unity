using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DoorController : MonoBehaviour, iInteractable, iInfectable
{
    private Animator animator;
    private ParticleSystem infectedParticles;

    private bool isOpen = false;
    [SerializeField]
    private bool isInfected = false;
    public bool IsInfected
    {
        get
        {
            return isInfected;
        }
    }
    public float infectionDuration { get; set; }

    // Start is called before the first frame update
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (isInfected) Infect();
    }

    public void Interact(iInfectable human = null)
    {
        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);

        //the door is infected and the thing that interacted with the door is infectable
        if (isInfected && human != null)
        {
            human.Infect();
        }
      
    }

    public void Infect()
    {
        isInfected = true;
        infectedParticles.Play();
        StartCoroutine(Desinfect());
    }

    public IEnumerator Desinfect()
    {
        yield return new WaitForSeconds(infectionDuration);
        isInfected = false;
        infectedParticles.Stop();
    }
}
