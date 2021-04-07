using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float speed = 2f;
    [SerializeField]
    private Vector2 screenLimitsX;
    [SerializeField]
    private Vector2 screenLimitsY;

    private float horizontal = 0f;
    private float vertical = 0f;

    // Update is called once per frame
    void Update()
    {

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (transform.position.x < screenLimitsX.x && horizontal < 0 ||
            transform.position.x > screenLimitsX.y && horizontal > 0)
        {
            horizontal = 0;
        }

        if (transform.position.y < screenLimitsY.x && vertical < 0 ||
            transform.position.y > screenLimitsY.y && vertical > 0)
        {
            vertical = 0;
        }

        transform.Translate(new Vector2(horizontal, vertical) * speed * Time.unscaledDeltaTime);

        if (transform.position.x < screenLimitsX.x)
        {
            transform.position = new Vector3(screenLimitsX.x, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > screenLimitsX.y)
        {
            transform.position = new Vector3(screenLimitsX.y, transform.position.y, transform.position.z);
        }

        if (transform.position.y < screenLimitsY.x)
        {
            transform.position = new Vector3(transform.position.x, screenLimitsY.x, transform.position.z);
        }
        else if (transform.position.y > screenLimitsY.y)
        {
            transform.position = new Vector3(transform.position.x, screenLimitsY.y, transform.position.z);
        }

    }
}
