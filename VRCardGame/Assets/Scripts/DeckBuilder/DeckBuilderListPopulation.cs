using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DeckBuilderListPopulation : MonoBehaviour
{

    CardDictionary dictionary;
    public GameObject listItemPrefab;
    int cardDictionaryIncrementer = 30;
    public int cardDictionaryScrollTracker = 0; //For pressing the key down.
    public int cardDictionaryFastTracker = 0; //For holding the key down.
    ScrollRect cardDictionaryScrollrect;
    GameObject cardDictionaryViewport;
    GameObject cardDictionaryContent;
    bool listMover = true; //True means right, false means left.
    int selectedItem = 2; //Start with the third item in the list. Keeps track of the position of highlighted item.

    CardDictionary listItemContainer;

    // Use this for initialization
    void Start()
    {

        dictionary = GameObject.Find("CardDictionary").GetComponent<CardDictionary>();
        listItemContainer = GameObject.Find("ListItemContainer").GetComponent<CardDictionary>();

        GameObject tmplist = GameObject.Find("CardDictionaryList");
        cardDictionaryScrollrect = tmplist.GetComponent<ScrollRect>();
        cardDictionaryScrollrect.elasticity = 0;
        cardDictionaryScrollrect.decelerationRate = 0.000001f;

        cardDictionaryViewport = tmplist.transform.FindChild("Viewport").gameObject;
        cardDictionaryContent = cardDictionaryViewport.transform.FindChild("Content").gameObject;

        (cardDictionaryContent.transform as RectTransform).sizeDelta = new Vector2((cardDictionaryContent.transform as RectTransform).sizeDelta.x, ((listItemPrefab.transform as RectTransform).rect.height) * (dictionary.cardList.Count + 2));

        GameObject listItem;
        foreach (GameObject g in dictionary.cardList)
        {
            listItem = GameObject.Instantiate(listItemPrefab);
            Button listItemButton = listItem.GetComponent<Button>();
            listItemButton.onClick.AddListener(listItem.GetComponent<ClickHandler>().clickedRight);

            RectTransform listItemRect = listItem.transform as RectTransform;

            listItemRect.SetParent(cardDictionaryContent.transform as RectTransform);
            listItemRect.sizeDelta = new Vector2((cardDictionaryContent.transform as RectTransform).rect.width, listItemRect.rect.height);
            listItemRect.FindChild("Text").GetComponent<Text>().text = g.GetComponent<ICard>().cardName;
            listItemRect.anchoredPosition = new Vector2(0, (cardDictionaryContent.transform as RectTransform).rect.height / 2 - cardDictionaryIncrementer);
            cardDictionaryIncrementer += 30;

            listItemContainer.cardList.Add(listItem);
        }

        GameObject tmp = listItemContainer.cardList[selectedItem];
        Button tmpButton = tmp.GetComponent<Button>();
        tmpButton.Select();

    }

    // Update is called once per frame
    void Update()
    {

        if (listMover)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) //Probably more efficient with an event callback?
            {
                if (selectedItem > 0)
                {
                    selectedItem--;
                }
                if (cardDictionaryScrollTracker == 0)
                {
                    cardDictionaryScrollrect.velocity = new Vector2(0, -500);
                    cardDictionaryFastTracker = 25;
                }
                else
                {
                    cardDictionaryScrollTracker--;
                }
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (cardDictionaryFastTracker > 0)
                {
                    cardDictionaryFastTracker--;
                }
                else
                {
                    cardDictionaryFastTracker = 4;
                    cardDictionaryScrollrect.velocity = new Vector2(0, -500);
                    cardDictionaryScrollTracker = 0;
                    if (selectedItem > 0)
                    {
                        selectedItem--;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (selectedItem < listItemContainer.cardList.Count - 1)
                {
                    selectedItem++;
                }
                if (cardDictionaryScrollTracker == 9)
                {
                    cardDictionaryScrollrect.velocity = new Vector2(0, 500);
                    cardDictionaryFastTracker = 65;
                }
                else
                {
                    cardDictionaryScrollTracker++;
                }
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                if (cardDictionaryFastTracker < 90)
                {
                    cardDictionaryFastTracker++;
                }
                else
                {
                    cardDictionaryFastTracker = 86;
                    cardDictionaryScrollrect.velocity = new Vector2(0, 500);
                    cardDictionaryScrollTracker = 9;
                    if (selectedItem < listItemContainer.cardList.Count - 1)
                    {
                        selectedItem++;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                listMover = false;
            }

            //Redundent, but sometimes useful if we lose track of the scroller.
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                GameObject tmp = listItemContainer.cardList[selectedItem];
                Button tmpButton = tmp.GetComponent<Button>();
                tmpButton.Select();
            }

        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            listMover = true;

            GameObject tmp = listItemContainer.cardList[selectedItem];
            Button tmpButton = tmp.GetComponent<Button>();
            tmpButton.Select();
        }

    }

}
