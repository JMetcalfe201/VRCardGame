using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class NetworkMenuRelay : MonoBehaviour
{
    NetworkManager manager;

    public string deckpath;
    public int[] loadedDeck;

    bool destroyOnLoad = false;

    void OnLevelWasLoaded(int i)
    {
        if(Application.loadedLevelName == "arena_level")
        {
            destroyOnLoad = true;
        }

        if (Application.loadedLevelName == "main_menu")
        {
            if(destroyOnLoad)
            {
                DestroyImmediate(gameObject);
            }
        }
    }

    IEnumerator WaitToStartHost()
    {
        while(loadedDeck == null)
        {
            yield return 0;
        }

        manager.StartHost();
    }

    IEnumerator WaitToStartClient()
    {
        while(loadedDeck == null)
        {
            yield return 0;
        }

        manager.networkAddress = GameObject.Find("IPField").GetComponent<InputField>().text;
        manager.StartClient();
    }

    void Start()
    {
        manager = GetComponent<NetworkManager>();
        loadedDeck = null;
    }

    public void StartHost()
    {
        loadedDeck = DeckSelector.LoadDeck(deckpath);

        StartCoroutine(WaitToStartHost());
    }

    public void StartClient()
    {
        loadedDeck = DeckSelector.LoadDeck(deckpath);

        StartCoroutine(WaitToStartClient());
    }

    public void Quit()
    {
        Application.Quit();
    }
}
