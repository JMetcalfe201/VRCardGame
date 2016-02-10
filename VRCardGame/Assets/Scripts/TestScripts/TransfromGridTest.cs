using UnityEngine;
using System.Collections;

public class TransfromGridTest : MonoBehaviour 
{
    public TransformGrid grid;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            grid.GetPositionAt(1, 0);
        }

        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            grid.GetPositionAt(1, 1);
        }

        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            grid.GetPositionAt(1, 2);
        }

        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            grid.GetPositionAt(1, 3);
        }

        if(Input.GetKeyDown(KeyCode.Alpha5))
        {
            grid.GetPositionAt(1, 4);
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            grid.GetPositionAt(0, 0);
        }

        if(Input.GetKeyDown(KeyCode.W))
        {
            grid.GetPositionAt(0, 1);
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            grid.GetPositionAt(0, 2);
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            grid.GetPositionAt(0, 3);
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            grid.GetPositionAt(0, 4);
        }
	}
}
