using UnityEngine;
using System.Collections;

public class TransformGrid : MonoBehaviour 
{
    [SerializeField]
    private Vector2 size;
    [SerializeField]
    private int xCells;
    [SerializeField]
    private int yCells;
    [SerializeField]
    private bool debugShowGrid;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if(debugShowGrid)
        {
            float currentX;
            for(int i = 0; i < xCells+1; i++)
            {
                currentX = i * size.x / xCells;

                Debug.DrawLine(new Vector3(transform.position.x + currentX, transform.position.y, transform.position.z), new Vector3(transform.position.x + currentX, transform.position.y, transform.position.z + size.y), Color.red, .05f);
            }

            float currentZ;
            for(int i = 0; i < yCells+1; i++)
            {
                currentZ = i * size.y / yCells;

                Debug.DrawLine(new Vector3(transform.position.x, transform.position.y, transform.position.z + currentZ), new Vector3(transform.position.x + size.x, transform.position.y, transform.position.z + currentZ), Color.red, .05f);
            }
        }
	}

    public Vector3 GetPositionAt(int x, int y)
    {
        if(x >= xCells || y >= yCells)
        {
            Debug.LogError("Index out of grid range.");
            return Vector3.zero;
        }

        Vector3 pos = new Vector3(transform.position.x + (x * size.x / xCells) + (size.x / xCells / 2), transform.position.y, transform.position.z + (y * size.y / yCells) + (size.y / yCells / 2));

        if(debugShowGrid)
        {
            Debug.Log(pos);
            Debug.DrawLine(pos, pos + new Vector3(0, .2f, 0), Color.red, 1.0f);
        }

        return pos;
    }
}
