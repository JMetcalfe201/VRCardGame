using UnityEngine;
using System.Collections;

public class ScoreboardFanSpin : MonoBehaviour 
{
    public float speed = 5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.eulerAngles = transform.eulerAngles + new Vector3(0f, speed * Time.deltaTime, 0f);
	}
}
