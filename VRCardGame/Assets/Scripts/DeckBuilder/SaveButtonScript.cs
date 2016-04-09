using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class SaveButtonScript : MonoBehaviour
{

    CardDictionary deck;

    // Use this for initialization
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SaveClick()
    {
        File.Delete("PlayerDeck.txt");

        deck = GameObject.Find("DeckInProgress").GetComponent<CardDictionary>(); //CardDictionary is a misnomer here, it's just a script containing a list.

        StreamWriter writer = new StreamWriter(File.OpenWrite("PlayerDeck.txt"));

        foreach (GameObject g in deck.cardList)
        {
            writer.WriteLine(g.transform.FindChild("Text").GetComponent<Text>().text);
        }
        writer.Close();
    }
}
