using UnityEngine;
using System.Collections;

public class MonsterCard : ICard
{
    public int attack;
    public int defense;
    public int rating;

    [SerializeField]
    public bool attackMode = false;


    public void SetAttackMode()
    {
        if(!attackMode)
        {
            attackMode = true;

            transform.localEulerAngles -= new Vector3(0f, 90f, 0f);
        }
    }

    public void SetDefenseMode()
    {
        if(attackMode)
        {
            attackMode = false;

            transform.localEulerAngles += new Vector3(0f, 90f, 0f);
        }
    }

    public int GetCombatValue()
    {
        if(attackMode)
        {
            return attack;
        }
        else
        {
            return defense;
        }
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