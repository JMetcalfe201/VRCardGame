using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardDictionary : MonoBehaviour 
{
    public static CardDictionary singleton;

    public List<GameObject> cardList;

    // Use this for initialization
    void Start () 
    {
        if (singleton != null)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            singleton = this;
        }
	}
	
    public GameObject GetPrefabByID(int id)
    {
        return cardList[id];
    }

    public ICard GetInfoByID(int id)
    {
        return cardList[id].GetComponent<ICard>();
    }
}
