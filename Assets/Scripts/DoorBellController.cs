using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class DoorBellController : MonoBehaviour, iInteractable, iInfectable
{
    [SerializeField]
    private CharacterController HumanTarget;
    [SerializeField]
    private GameObject DoorTarget;
    [SerializeField]
    private AudioClip doorbellSound;
    
    private AudioSource source;
    private SpriteRenderer sprite;

    public Interactable objectType { get; }
    private bool isInfected = false;
    public bool IsInfected
    {
        get
        {
            return isInfected;
        }
    }


    private void Awake()
    {
        source = GetComponent<AudioSource>();
        sprite = GetComponent<SpriteRenderer>();

        source.clip = doorbellSound;
        source.time = 1f;
    }

    public void Infect(float duration = -1)
    {
        if (isInfected) return;

        isInfected = true;
        SceneSingleton.Instance.level.onTimeChange(ChangeColour);

        if (duration != -1)
        {
            StartCoroutine(Desinfect(duration));
        }
    }

    public IEnumerator Desinfect(float infectionDuration)
    {
        yield return new WaitForSeconds(infectionDuration);
        isInfected = false;
    }

    public bool Interact(CharacterController human = null)
    {
        //the door is infected and the thing that interacted with the door is infectable
        if (isInfected && human != null)
        {
            // germophobes can't be infected by touching infected surfaces
            if (!human.Traits.Contains(CharacterTraits.Germophobic))
            {
                human.Infect();
            }
        }

        source.Play();
        HumanTarget.StartCoroutine(HumanTarget.Interact(DoorTarget.transform));

        return true;
    }



    private void OnMouseDown()
    {
        Interact();
    }

    private void OnMouseOver()
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.5f);
    }
    private void OnMouseExit()
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, 1);
    }

    public void ChangeColour(float timeSpeed)
    {
        if (timeSpeed == 0 && isInfected)
        {
            sprite.color = new Color(0, 255, 0, sprite.color.a);
            return;
        }

        sprite.color = new Color(255, 255, 255, sprite.color.a);
    }
}
