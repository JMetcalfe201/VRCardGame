using UnityEngine;
using System.Collections;

public abstract class IEffectCard : ICard
{
    bool blocked = false;

    //event OnActivate(); Commented out so that it is playable.
    public virtual bool CanActivate()
    {
        if (!blocked)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Block()
    {
        blocked = false;
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
