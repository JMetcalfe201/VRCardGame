using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class GrassRandomizer : MonoBehaviour
{
    public GameObject grassPrefab;
    public GameObject ground;
    public float placementDensity;

	// Use this for initialization
	void Start ()
    {
        BoxCollider col = GetComponent<BoxCollider>();

        Vector3 currentPos = new Vector3(transform.position.x - col.size.x/2, transform.position.y, transform.position.z - col.size.z/2);
        GameObject grass;
        while (currentPos.x < transform.position.x + col.size.x / 2)
        {
            Debug.Log(currentPos);
            currentPos.z = transform.position.z - col.size.z / 2;

            while(currentPos.z < transform.position.z + col.size.z / 2)
            {
                grass = GameObject.Instantiate(grassPrefab, currentPos + new Vector3(Random.Range(0f, placementDensity / 5), -0.1f, Random.Range(0f, placementDensity / 5)), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)) as GameObject;
                grass.transform.localScale = new Vector3(Random.Range(0.5f, 0.8f), 0.6f, Random.Range(0.5f, 0.8f));

                currentPos += new Vector3(0, 0, placementDensity);
            }

            currentPos += new Vector3(placementDensity, 0, 0);

            Debug.Log(currentPos);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
