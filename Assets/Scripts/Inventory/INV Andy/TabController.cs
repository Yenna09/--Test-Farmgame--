using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    public Image[] tabImages;

    [SerializeField]
    public GameObject[] pages;

    UnityEngine.Color gray = UnityEngine.Color.gray; //Gray
    UnityEngine.Color white = UnityEngine.Color.white; //White
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ActivateTab(0);
    }

    // Update is called once per frame
    public void ActivateTab(int tabNum)
    {
        for(int i=0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            tabImages[i].color = gray;
        }
        pages[tabNum].SetActive(true);
        tabImages[tabNum].color = white;
    }
}
