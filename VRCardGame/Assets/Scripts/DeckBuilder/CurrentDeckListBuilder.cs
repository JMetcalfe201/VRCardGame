using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CurrentDeckListBuilder : MonoBehaviour
{

    public GameObject listItemPrefab;
    public int scrollTracker = 0; //For pressing the key down.
    public int fastTracker = 0; //For holding the key down.
    ScrollRect scrollrect;
    GameObject viewport;
    GameObject content;
    bool listMover = true; //True means right, false means left.

    CardDictionary deck;

    // Use this for initialization
    void Start()
    {
        deck = GameObject.Find("DeckInProgress").GetComponent<CardDictionary>(); //CardDictionary is a misnomer here, it's just a script containing a list.

        GameObject tmplist = GameObject.Find("CurrentDeck");
        scrollrect = tmplist.GetComponent<ScrollRect>();
        scrollrect.elasticity = 0;
        scrollrect.decelerationRate = 0.000001f;

        viewport = tmplist.transform.FindChild("Viewport").gameObject;
        content = viewport.transform.FindChild("Content").gameObject;

        (content.transform as RectTransform).sizeDelta = new Vector2((content.transform as RectTransform).sizeDelta.x, ((listItemPrefab.transform as RectTransform).rect.height) * (180 + 2)); //deck.cardList.Count

    }


    // Update is called once per frame
    void Update()
    {

        if (!listMover)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) //Probably more efficient with an event callback?
            {
                if (scrollTracker == 0)
                {
                    scrollrect.velocity = new Vector2(0, -500);
                    fastTracker = 25;
                }
                else
                {
                    scrollTracker--;
                }
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (fastTracker > 0)
                {
                    fastTracker--;
                }
                else
                {
                    fastTracker = 4;
                    scrollrect.velocity = new Vector2(0, -500);
                    scrollTracker = 0;
                }
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (scrollTracker == 9)
                {
                    scrollrect.velocity = new Vector2(0, 500);
                    fastTracker = 65;
                }
                else
                {
                    scrollTracker++;
                }
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                if (fastTracker < 90)
                {
                    fastTracker++;
                }
                else
                {
                    fastTracker = 86;
                    scrollrect.velocity = new Vector2(0, 500);
                    scrollTracker = 9;
                }
            }


            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                listMover = true;
            }

        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (deck.cardList.Count > 2)
            {
                listMover = false;
                scrollrect.velocity = new Vector2(0, -20000);
                scrollTracker = 0;
                fastTracker = 0;

                GameObject tmp = deck.cardList[2];
                Button tmpButton = tmp.GetComponent<Button>();
                tmpButton.Select();
            }
            else if (deck.cardList.Count > 1)
            {
                listMover = false;
                scrollrect.velocity = new Vector2(0, -20000);
                scrollTracker = 0;
                fastTracker = 0;

                GameObject tmp = deck.cardList[1];
                Button tmpButton = tmp.GetComponent<Button>();
                tmpButton.Select();
            }
            else if (deck.cardList.Count > 0)
            {
                listMover = false;
                scrollrect.velocity = new Vector2(0, -20000);
                scrollTracker = 0;
                fastTracker = 0;

                GameObject tmp = deck.cardList[0];
                Button tmpButton = tmp.GetComponent<Button>();
                tmpButton.Select();
            }
        }
    }
}
