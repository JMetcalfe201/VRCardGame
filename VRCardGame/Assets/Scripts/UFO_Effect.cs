using UnityEngine;
using System.Collections;

public class UFO_Effect : MonoBehaviour 
{
    Material mat;

    Vector3 targetColor;
    Vector3 colorVelocity = Vector3.zero;

    public float newColorTimer = 0.5f;
    public float ChangeSpeed = 0.05f;

    float time;

	// Use this for initialization
	void Start () 
    {
        time = 0;
        mat = GetComponent<Renderer>().materials[3];

        targetColor = Random.onUnitSphere;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(time > newColorTimer)
        {
            targetColor = Random.onUnitSphere;
            time = 0;
        }

        Vector3 oldCol = new Vector3(mat.color.r, mat.color.g, mat.color.b);
        Vector3 newCol = Vector3.SmoothDamp(oldCol, targetColor, ref colorVelocity, ChangeSpeed);

        mat.color = new Color(newCol.x, newCol.y, newCol.z);

        time += Time.deltaTime;
	}
}
