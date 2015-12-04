using UnityEngine;
using System.Collections;

public abstract class ICard : MonoBehaviour
{

    [SerializeField]
    public string cardName;
    public int cardID;
    [SerializeField]
    string description;
    [SerializeField]
    ECardType cardtype;
    [SerializeField]
    public GameObject _3Dmodel;
    bool revealed;
    //event OnSummoned; Commented out so it can still be playable.
    //event OnToGraveyard;
    //event OnToHand;
    //event OnReveal;
    //event OnAttack;
    //event OnDefend;


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
    MONSTER_CARD = 1,
    MAGIC_CARD = 2,
    TRAP_CARD = 3
};