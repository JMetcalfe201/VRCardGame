using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour 
{
    [SerializeField]
    List<TextMesh> blueLifePoints;
    [SerializeField]
    List<TextMesh> redLifePoints;
    [SerializeField]
    List<TextMesh> phaseText;
    [SerializeField]
    List<GameObject> turnIndicator;

	// Use this for initialization
	void Start ()
    {
	    foreach(TextMesh t in blueLifePoints)
        {
            t.gameObject.GetComponent<Renderer>().material.color = Color.Lerp(Color.cyan, Color.blue, .5f);
        }
        
        foreach(TextMesh t in redLifePoints)
        {
            t.gameObject.GetComponent<Renderer>().material.color = Color.red;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetBlueLifePoints(int lp)
    {
        foreach (TextMesh t in blueLifePoints)
        {
            t.text = lp.ToString();
        }
    }

    public void SetRedLifePoits(int lp)
    {
        foreach (TextMesh t in redLifePoints)
        {
            t.text = lp.ToString();
        }
    }

    public void SetPhaseText(string s)
    {
        foreach (TextMesh t in phaseText)
        {
            t.text = s;
        }
    }

    public void SetTurn(int playerTurn)
    {
        if(playerTurn == 1)
        {
            foreach (GameObject g in turnIndicator)
            {
                g.GetComponent<Renderer>().material.color = Color.red;
            }
        }
        else
        {
            foreach (GameObject g in turnIndicator)
            {
                g.GetComponent<Renderer>().material.color = Color.blue;
            }
        }
    }
}
