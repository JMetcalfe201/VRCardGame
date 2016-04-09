using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClickHandler : MonoBehaviour
{

    public GameObject buttonPrefab;
    CardDictionary deck;

    int deckIncrementer = 30;
    GameObject deckViewport;
    GameObject deckContent;



    // Use this for initialization
    void Start()
    {
        deck = GameObject.Find("DeckInProgress").GetComponent<CardDictionary>(); //CardDictionary is a misnomer here, it's just a script containing a list.

        GameObject tmplist = GameObject.Find("CurrentDeck");

        deckViewport = tmplist.transform.FindChild("Viewport").gameObject;
        deckContent = deckViewport.transform.FindChild("Content").gameObject;

    }

    public void clickedRight()
    {
        int cardCount = 0;
        foreach (GameObject g in deck.cardList)
        {
            if(g.transform.FindChild("Text").GetComponent<Text>().text == gameObject.transform.FindChild("Text").GetComponent<Text>().text)
            {
                cardCount++;
            }
        }


        if (cardCount < 3)
        {
            GameObject listItem = GameObject.Instantiate(buttonPrefab); //Instantiate makes copies. I can learn things, I promise.
            listItem.transform.FindChild("Text").GetComponent<Text>().text = gameObject.transform.FindChild("Text").GetComponent<Text>().text;
            deck.cardList.Add(listItem);

            Button listItemButton = listItem.GetComponent<Button>();
            listItemButton.onClick.AddListener(listItem.GetComponent<ClickHandler>().clickedLeft);

            RectTransform listItemRect = listItem.transform as RectTransform;

            listItemRect.SetParent(deckContent.transform as RectTransform);
            listItemRect.anchoredPosition = new Vector2(0, (deckContent.transform as RectTransform).rect.height / 2 - (deck.cardList.Count * 30));
        }
    }

    public void clickedLeft()
    {
        bool found = false;
        int index = 0;
        int cardIndex = 0;
        foreach (GameObject g in deck.cardList)
        {
            if (g == gameObject)
            {
                found = true;
                cardIndex = index;
            }
            if (found)
            {
                RectTransform gRekt = g.transform as RectTransform;
                gRekt.anchoredPosition = new Vector2(0, (deckContent.transform as RectTransform).rect.height / 2 - (index * 30));
            }
            index++;
        }
        
        deck.cardList.Remove(gameObject);
        Destroy(gameObject);

        if(deck.cardList.Count > cardIndex)
        {
            GameObject tmp = deck.cardList[cardIndex];
            Button tmpButton = tmp.GetComponent<Button>();
            tmpButton.Select();
        }
        else if (deck.cardList.Count > cardIndex - 1 && deck.cardList.Count > 0)
        {
            GameObject tmp = deck.cardList[cardIndex - 1];
            Button tmpButton = tmp.GetComponent<Button>();
            tmpButton.Select();
        }
        else
        {
            //I should have it push the right key, effectively.
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
