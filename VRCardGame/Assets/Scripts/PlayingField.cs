using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

using DeckClass;

namespace PlayingFieldClass
{
    public class PlayingField : NetworkBehaviour
    {

        private CardTestType[] monsterCards;
        private CardTestType[] effectCards;
        private Deck playerDeck;
        private Deck graveyard;

        public PlayingField(Deck d)
        {
            monsterCards = new CardTestType[5];
            effectCards = new CardTestType[5];
            playerDeck = d;
            graveyard = new Deck();
        }

        public Deck getDeck()
        {
            return playerDeck;
        }

        public CardTestType getMonsterCardByIndex(int i)
        {
            if (i > -1 && i < 5)
            {
                return monsterCards[i];
            }
            else
            {
                Debug.Log("Error: index out of range");
                return null;
            }
        }

        public CardTestType getEffectCardByIndex(int i)
        {
            if (i > -1 && i < 5)
            {
                return effectCards[i];
            }
            else
            {
                Debug.Log("Error: index out of range");
                return null;
            }
        }

        public bool setMonsterCardByIndex(CardTestType c, int i)
        {
            if (i > -1 && i < 5)
            {
                if (null == monsterCards[i])
                {
                    monsterCards[i] = c;
                    Debug.Log("Card placed");
                    return true;
                }
                else
                {
                    Debug.Log("Cannot place card: spot is full");
                    return false;
                }
            }
            else
            {
                Debug.Log("Cannot place card: index out of range");
                return false;
            }
        }

        public bool setEffectCardByIndex(CardTestType c, int i)
        {
            if (i > -1 && i < 5)
            {
                if (null == effectCards[i])
                {
                    effectCards[i] = c;
                    Debug.Log("Card placed");
                    return true;
                }
                else
                {
                    Debug.Log("Cannot place card: spot is full");
                    return false;
                }
            }
            else
            {
                Debug.Log("Cannot place card: index out of range");
                return false;
            }
        }

        // for testing/debugging
        public void print()
        {
            string outputString = "Monster Cards: ";
            for (int i = 0; i < 5; i++)
            {
                if(monsterCards[i] != null)
                    outputString = outputString + monsterCards[i].getid() + ' ';
            }
            outputString = outputString + "  Effect Cards: ";
            for (int i = 0; i < 5; i++)
            {
                if (effectCards[i] != null)
                    outputString = outputString + effectCards[i].getid() + ' ';
            }
            Debug.Log(outputString);
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
}