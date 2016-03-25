using UnityEngine;
using System.Collections;

public abstract class ICard : MonoBehaviour
{

    [SerializeField]
    public string cardName;

    public int cardID;

    [SerializeField]
    public string description;

    [SerializeField]
    public ECardType cardtype;

    [SerializeField]
    public GameObject _3DmonsterModel;

    public GameObject _3DcardModel;

    public bool revealed = false;


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

    public void Placed(bool onField)
    {
        if(onField)
        {
            _3DmonsterModel = Instantiate(_3DmonsterModel, Vector3.zero, Quaternion.identity) as GameObject;
        }
        else
        {
            _3DmonsterModel = null;
        }
    }

    public void SetMonsterModelTransform(Vector3 pos, Vector3 angles)
    {
        _3DmonsterModel.transform.position = pos;
        _3DmonsterModel.transform.localEulerAngles = angles;
    }

    public void Reveal()
    {
        if(!revealed)
        {
            revealed = true;

            _3DcardModel.transform.localEulerAngles += new Vector3(0f, 0f, 180f);

            if (_3DmonsterModel != null)
            {
                _3DmonsterModel.GetComponentInChildren<Renderer>().enabled = true;
            }
        }
    }
}



public enum ECardType
{
    MONSTER_CARD = 1,
    MAGIC_CARD = 2,
    TRAP_CARD = 3,
    UNKNOWN = 4
};