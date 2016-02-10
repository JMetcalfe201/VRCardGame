using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DeckBuilderListPopulation : MonoBehaviour {

    CardDictionary dictionary;
    public GameObject listItemPrefab;
    int incrementer = 30;

	// Use this for initialization
	void Start () {

        dictionary = GameObject.Find("CardDictionary").GetComponent<CardDictionary>();

        foreach(GameObject g in dictionary.cardList)
        {
            GameObject listItem = GameObject.Instantiate(listItemPrefab);
            GameObject tmplist = GameObject.Find("CardDictionaryList");
            if (tmplist)
            {
                Debug.Log("tmplist works");
            }
            GameObject content = tmplist.transform.FindChild("Viewport").gameObject;
            if (content)
            {
                Debug.Log("content works");
            }

            RectTransform listItemRect = listItem.transform as RectTransform;

            listItemRect.SetParent(content.transform as RectTransform);
            listItemRect.GetComponent<Text>().text = g.GetComponent<ICard>().cardName;
            listItemRect.anchoredPosition = new Vector2(0, (content.transform as RectTransform).rect.height/2 - incrementer);
            incrementer += 30;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
