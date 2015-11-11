using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;   // Needed for List<T>

namespace DeckClass
{
    public class Deck : MonoBehaviour
    {
        // Index 0 is considered the "bottom"
        private List<CardTestType> cards;

        public Deck() { cards = new List<CardTestType>(); }

        public void Shuffle()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                CardTestType temp = cards[i];
                int randomIndex = Random.Range(i, cards.Count);
                cards[i] = cards[randomIndex];
                cards[randomIndex] = temp;
            }
        }

        public CardTestType DrawTop()
        {
            if (cards.Count < 1)
            {
                Debug.Log("Tried to draw top card from empty deck");
            }
            int last_index = cards.Count - 1;
            CardTestType topcard = cards[last_index];
            cards.RemoveAt(last_index);
            return topcard;
        }
        public CardTestType DrawBottom()
        {
            if (cards.Count < 1)
            {
                Debug.Log("Tried to draw bottom card from empty deck");
            }
            CardTestType bottomcard = cards[0];
            cards.RemoveAt(0);
            return bottomcard;
        }
        public void addCardBottom(CardTestType c) { cards.Insert(0, c); }
        public void addCardTop(CardTestType c) { cards.Add(c); }
        public CardTestType takeCard(int card_id)
        {
            int card_index = cards.FindIndex(
            delegate (CardTestType c)
            {
                return c.getid() == card_id;
            }
            );
            if (card_index != -1)
            {
                CardTestType taken_card = cards[card_index];
                cards.RemoveAt(card_index);
                return taken_card;
            }
            else
            {
                Debug.Log("Tried to take a card that does not exist in the deck");
                return null;
            }
        }

        public void print()
        {
            string outputString = "";
            foreach (CardTestType c in cards)
            {
                outputString = outputString + c.getid() + ' ';
            }
            Debug.Log("Deck Contents: " + outputString);
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public class CardTestType : MonoBehaviour
    {
        private int id;

        public CardTestType(int id_val) { id = id_val; }
        public int getid() { return id; }
    }
}