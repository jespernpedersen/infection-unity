using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObjectController : MonoBehaviour, iInteractable
{
    private Vector3 originalPos;
    public Interactable objectType {get; }
    private bool isFallen = false;

    private Rigidbody2D rb;


    // Start is called before the first frame update
    void Awake()
    {
        originalPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnMouseDown()
    {
        Debug.Log("HI");
      Interact();
    }

    public bool Interact(CharacterController trigger = null)
    {
        isFallen = !isFallen;
        if(isFallen)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
       return true;
    }
}
