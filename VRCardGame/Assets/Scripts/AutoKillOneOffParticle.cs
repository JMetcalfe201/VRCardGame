using UnityEngine;
using System.Collections;

public class AutoKillOneOffParticle : MonoBehaviour 
{
    ParticleSystem system;

	// Use this for initialization
	void Start () 
    {
        system = GetComponent<ParticleSystem>();

        if(system == null)
        {
            system = transform.GetComponentInChildren<ParticleSystem>();
        }

        StartCoroutine(DelayedDestroy(system.duration));
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    IEnumerator DelayedDestroy(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        Destroy(gameObject);
    }
}
