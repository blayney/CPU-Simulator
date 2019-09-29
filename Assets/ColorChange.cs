using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ColorChange : MonoBehaviour
{
    public Button CCB;
    public string CurrentState = "dark";
    public Image LPanelBG;
    // Start is called before the first frame update
    void Start()
    {
        CCB.onClick.AddListener(delegate { ChangeMode(); });
    }

    public void ChangeMode() {
        if (CurrentState.Equals("dark"))
        {
            LightMode();
        }
        else {
            DarkMode();
        }
    }

    public void LightMode() {
        LPanelBG.color = UnityEngine.Color.white;

        CurrentState = "light";
    }
    public void DarkMode()
    {
        LPanelBG.color = UnityEngine.Color.black;

        CurrentState = "dark";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
