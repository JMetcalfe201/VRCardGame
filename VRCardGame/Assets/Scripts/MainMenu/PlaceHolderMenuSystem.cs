using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaceHolderMenuSystem : MonoBehaviour
{
    public List<GameObject> menuItems;
    public GameObject selectionIndicator;
    private bool vAxisInUse;
    private bool hAxisInUse;

    private int selectedIndex;

    // Use this for initialization
    void Start()
    {
        vAxisInUse = false;
        hAxisInUse = false;

        if (menuItems.Count > 0)
        {
            InitSelection();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (menuItems.Count > 0)
            HandleInput();
    }

    private void HandleInput()
    {
        float v = Input.GetAxisRaw("Menu Vertical");
        float h = Input.GetAxisRaw("Menu Horizontal");

        if ((v > 0.5 || v < -0.5) && !vAxisInUse )
        {
            if (v > 0)
            {
                PreviousSelection();
            }
            else
            {
                NextSelection();
            }

            vAxisInUse = true;
        }
        else if (v < 0.5 && v > -0.5)
        {
            vAxisInUse = false;
        }

        if ((h > 0.5 || h < -0.5) && !hAxisInUse)
        {
            if (h > 0)
            {
                menuItems[selectedIndex].GetComponent<PHMenuItem>().Right();
            }
            else
            {
                menuItems[selectedIndex].GetComponent<PHMenuItem>().Left();
            }

            hAxisInUse = true;
        }
        else if (h < 0.5 && h > -0.5)
        {
            hAxisInUse = false;
        }

        if (Input.GetButtonDown("Menu Accept"))
        {
            menuItems[selectedIndex].GetComponent<PHMenuItem>().Select();
        }
    }

    private void InitSelection()
    {
        selectedIndex = 0;

        UpdateSelectionIndicator();
    }

    private void UpdateSelectionIndicator()
    {
        GameObject item = menuItems[selectedIndex];

        float width = item.GetComponent<BoxCollider>().size.x * item.transform.localScale.x * 1.1f;
        float height = item.GetComponent<BoxCollider>().size.y * item.transform.localScale.y * 1.1f;

        selectionIndicator.transform.localScale = new Vector3(width, height, 1);
        selectionIndicator.transform.position = item.transform.position;
    }

    private void NextSelection()
    {
        selectedIndex++;

        if (selectedIndex == menuItems.Count)
        {
            selectedIndex = 0;
        }

        UpdateSelectionIndicator();
    }

    private void PreviousSelection()
    {
        selectedIndex--;

        if (selectedIndex < 0)
        {
            selectedIndex = menuItems.Count - 1;
        }

        UpdateSelectionIndicator();
    }
}
