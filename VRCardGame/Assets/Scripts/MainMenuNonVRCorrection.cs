using UnityEngine;
using UnityEngine.VR;
using System.Collections;

public class MainMenuNonVRCorrection : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	    if(VRSettings.enabled)
        {
            Quaternion tempRot = transform.rotation;
            tempRot.eulerAngles = new Vector3(0, tempRot.eulerAngles.y, tempRot.eulerAngles.z);
            transform.rotation = tempRot;
        }     
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
