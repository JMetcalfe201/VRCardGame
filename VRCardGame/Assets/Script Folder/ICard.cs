using UnityEngine;
using System.Collections;

public abstract class ICard : MonoBehaviour
{


    string name;
    string description;
    ECardType cardtype;
    GameObject _3Dmodel;
    bool revealed;
    event OnSummoned;
    event OnToGraveyard;
    event OnToHand;
    event OnReveal;
    event OnAttack;
    event OnDefend;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}



enum ECardType
{

};