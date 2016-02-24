using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class Deck
{
    // Index 0 is considered the "bottom"
    private List<int> cards;

    public Deck()
    {
        cards = new List<int>();
        addCardTop(0);
    }
    // Construct a deck from a list of integer card IDs
    public Deck(List<int> idlist)
    {
        cards = idlist;
    }

    public void Shuffle()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int temp = cards[i];
            int randomIndex = Random.Range(i, cards.Count);
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    public int DrawTop()
    {
        if (cards.Count < 1)
        {
            Debug.Log("Tried to draw top card from empty deck");
        }
        int last_index = cards.Count - 1;
        int topcard = cards[last_index];
        cards.RemoveAt(last_index);
        return topcard;
    }
    public int DrawBottom()
    {
        if (cards.Count < 1)
        {
            Debug.Log("Tried to draw bottom card from empty deck");
        }
        int bottomcard = cards[0];
        cards.RemoveAt(0);
        return bottomcard;
    }
    public void addCardBottom(int c) { cards.Insert(0, c); }
    public void addCardTop(int c) { cards.Add(c); }

    /*
    public ICard takeCard(string card_name)
    {
        int card_index = cards.FindIndex(
        delegate (ICard c)
        {
            return c.cardName.Equals(card_name);
        }
        );
        if (card_index != -1)
        {
            ICard taken_card = cards[card_index];
            cards.RemoveAt(card_index);
            return taken_card;
        }
        else
        {
            Debug.Log("Tried to take a card that does not exist in the deck");
            return null;
        }
    }
    */

    public void print()
    {
        string outputString = "";
        foreach (int c in cards)
        {
            outputString = outputString + c + ' ';
        }
        Debug.Log("Deck Contents: " + outputString);
    }
}
