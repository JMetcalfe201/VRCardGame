using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DeckSelector : MonoBehaviour 
{
    FileInfo[] decks;
    int selectedIndex = 0;
    TextMesh mesh;

    public NetworkMenuRelay relay;

	// Use this for initialization
	void Start () 
    {
        mesh = GetComponent<TextMesh>();

        string path = Application.dataPath;

        path = Path.GetDirectoryName(path);
        path += "/Decks/";

        Debug.Log(path);

        DirectoryInfo dirInfo = new DirectoryInfo(path);
        decks = dirInfo.GetFiles();

        if(decks.Length == 0)
        {
            Debug.LogError("Could not load decks from /Decks/");
            return;
        }

        mesh.text = "< " + Path.GetFileNameWithoutExtension(decks[selectedIndex].Name) + " >";
        relay.deckpath = decks[selectedIndex].FullName;
	}

    public void NextDeck()
    {
        selectedIndex++;

        if(selectedIndex >= decks.Length)
        {
            selectedIndex = 0;
        }

        relay.deckpath = decks[selectedIndex].FullName;
    }

    public void PrevDeck()
    {
        selectedIndex--;

        if(selectedIndex < 0)
        {
            selectedIndex = decks.Length - 1;
        }

        relay.deckpath = decks[selectedIndex].FullName;
    }

    public static int[] LoadDeck(string path)
    {
        string[] lines = File.ReadAllLines(path);

        if(lines.Length == 0)
        {
            Debug.LogError("Loaded deck was empty.");
            return new int[0];
        }

        int[] cards = new int[lines.Length];

        for(int i = 0; i < cards.Length; i++)
        {
            cards[i] = int.Parse(lines[i]);
        }

        return cards;
    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
