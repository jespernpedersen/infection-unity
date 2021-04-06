using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionCloudController : MonoBehaviour
{

    private IEnumerator StartCountdown(float timer)
    {
        yield return new WaitForSeconds(timer);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        iInfectable infectable = collision.transform.GetComponent(typeof(iInfectable)) as iInfectable;

        if (infectable == null) return;

        Mutation mutation = SceneSingleton.Instance.virus.FindMutation(1);
        if (mutation.id != -1)// if the virus can survive in surfaces
        {
            infectable.Infect(mutation.duration);
        } else if(collision.gameObject.GetComponent<CharacterController>()) //if not, infect only humans
        {
            infectable.Infect();
        }

    }

}
