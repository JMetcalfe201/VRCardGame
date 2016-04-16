using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class SaveButtonScript : MonoBehaviour
{

    DeckInProgressScript deck;

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
        System.IO.Directory.CreateDirectory("Decks"); //Creates this if it hasn't already been created. Otherwise does nothing.
        File.Delete("Decks/PlayerDeck.txt");
        deck = GameObject.Find("DeckInProgress").GetComponent<DeckInProgressScript>();

        StreamWriter writer = new StreamWriter(File.OpenWrite("Decks/PlayerDeck.txt"));

        foreach (GameObject g in deck.cardList)
        {
            writer.WriteLine(g.transform.FindChild("Text").GetComponent<Text>().text);
        }

        writer.Close();
    }
}
