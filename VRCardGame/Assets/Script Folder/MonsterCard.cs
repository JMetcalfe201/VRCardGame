using UnityEngine;
using System.Collections;

class MonsterCard : ICard
{

    public int attack;
    public int defense;
    public int rating;
    [SerializeField]
    ECardType monsterType;
    [SerializeField]
    bool attackMode;


    public void SetAttackMode()
    {
        //Console.WriteLine ("Hello World!");
    }
    public void SetDefenseMode()
    {


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