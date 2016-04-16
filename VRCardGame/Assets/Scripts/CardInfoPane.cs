using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardInfoPane : MonoBehaviour
{
    [SerializeField]
    Text CardName;
    [SerializeField]
    Text CardAttack;
    [SerializeField]
    Text CardDefense;
    [SerializeField]
    Text CardDesc;

    [SerializeField]
    float fadeTime = 0.5f;

    private float bgAlpha;

    void Awake()
    {
        bgAlpha = GetComponent<Image>().color.a;
    }

	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void UpdateFields(MonsterCard card)
    {
        CardAttack.enabled = true;
        CardDefense.enabled = true;
        CardName.text = card.cardName;
        CardAttack.text = card.attack.ToString();
        CardDefense.text = card.defense.ToString();
        CardDesc.text = card.description;
    }

    public void UpdateFields(IEffectCard card)
    {
        CardAttack.enabled = false;
        CardDefense.enabled = false;
        CardName.text = card.cardName;
        CardDesc.text = card.description;
    }

    public IEnumerator FadeOut()
    {
        float time = 0;
        Color col;
        while(time < fadeTime)
        {
            time += Time.deltaTime;

            col = GetComponent<Image>().color;
            GetComponent<Image>().color = new Color(col.r, col.g, col.b, Mathf.Lerp(bgAlpha, 0, time / fadeTime));

            foreach(Text t in transform.GetComponentsInChildren<Text>())
            {
                col = t.color;

                t.color = new Color(col.r, col.g, col.b, Mathf.Lerp(1, 0, time / fadeTime));
            }

            yield return 0;
        }

        TurnOff();
    }

    public IEnumerator FadeIn()
    {
        float time = 0;
        Color col;
        while (time < fadeTime)
        {
            time += Time.deltaTime;

            col = GetComponent<Image>().color;
            GetComponent<Image>().color = new Color(col.r, col.g, col.b, Mathf.Lerp(0, bgAlpha, time / fadeTime));

            foreach (Text t in transform.GetComponentsInChildren<Text>())
            {
                col = t.color;

                t.color = new Color(col.r, col.g, col.b, Mathf.Lerp(0, 1, time / fadeTime));
            }

            yield return 0;
        }

        TurnOn();
    }

    public void TurnOff()
    {
        Color col;

        col = GetComponent<Image>().color;
        GetComponent<Image>().color = new Color(col.r, col.g, col.b, 0);

        foreach (Text t in transform.GetComponentsInChildren<Text>())
        {
            col = t.color;
            t.color = new Color(col.r, col.g, col.b, 0);
        }
    }

    public void TurnOn()
    {
        Color col;

        col = GetComponent<Image>().color;
        GetComponent<Image>().color = new Color(col.r, col.g, col.b, bgAlpha);

        foreach (Text t in transform.GetComponentsInChildren<Text>())
        {
            col = t.color;
            t.color = new Color(col.r, col.g, col.b, 1);
        }
    }
}
