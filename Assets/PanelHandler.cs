using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PanelHandler : MonoBehaviour
{

    public Button ViewDRAM;

    public Button Close;
    public Button ViewSchema;
    public Button ViewGraphs;
    public Image DRAMPanel;
    public Canvas DRAMCONTROL;
    public Canvas SCHEMACONTROL;
    public Canvas GRAPHSCNTRL;
    private CanvasGroup group;

    public Image DTBar;
    public Vector3 startPosition;
    private bool selected;
 
    // Start is called before the first frame update
    void Start()
    {
        DRAMCONTROL.gameObject.SetActive(false);
        Close.gameObject.SetActive(false);
        ViewDRAM.onClick.AddListener(delegate { ShowDRAMPane(); });
        ViewSchema.onClick.AddListener(delegate { ShowSCHEMPane(); });
        ViewGraphs.onClick.AddListener(delegate { ShowGRAPHPane();  });
        Close.onClick.AddListener(delegate { HideALLPane(); });
        SCHEMACONTROL.gameObject.SetActive(false);
        GRAPHSCNTRL.gameObject.SetActive(false);
    }
    public void HideALLPane() {
        DRAMCONTROL.gameObject.SetActive(false);
        SCHEMACONTROL.gameObject.SetActive(false);
        GRAPHSCNTRL.gameObject.SetActive(false);
        Close.gameObject.SetActive(false);
    }
    public void ShowDRAMPane() {
        float fadeTime = (float)0.1;
        /*
        HideDRAM.image.CrossFadeAlpha(0, 0, true);
        DRAMPanel.CrossFadeAlpha(0, (float)0, true);
        LBar.CrossFadeAlpha(0, (float)0, true);
        RBar.CrossFadeAlpha(0, (float)0, true);
        BBar.CrossFadeAlpha(0, (float)0, true);
        TBar.CrossFadeAlpha(0, (float)0, true);
        */
        DRAMCONTROL.gameObject.SetActive(true);
        Close.gameObject.SetActive(true);

        /*
        DRAMPanel.CrossFadeAlpha(1, fadeTime, true);

        LBar.CrossFadeAlpha(1, fadeTime, true);
        BBar.CrossFadeAlpha(1, fadeTime, true);
        RBar.CrossFadeAlpha(1, fadeTime, true);
        TBar.CrossFadeAlpha(1, fadeTime, true);
        HideDRAM.image.CrossFadeAlpha(1, (float)0, true);
        */
    }
    public void ShowSCHEMPane()
    {
        float fadeTime = (float)0.1;
        /*
        HideDRAM.image.CrossFadeAlpha(0, 0, true);
        DRAMPanel.CrossFadeAlpha(0, (float)0, true);
        LBar.CrossFadeAlpha(0, (float)0, true);
        RBar.CrossFadeAlpha(0, (float)0, true);
        BBar.CrossFadeAlpha(0, (float)0, true);
        TBar.CrossFadeAlpha(0, (float)0, true);
        */
        SCHEMACONTROL.gameObject.SetActive(true);
        Close.gameObject.SetActive(true);

        /*
        DRAMPanel.CrossFadeAlpha(1, fadeTime, true);

        LBar.CrossFadeAlpha(1, fadeTime, true);
        BBar.CrossFadeAlpha(1, fadeTime, true);
        RBar.CrossFadeAlpha(1, fadeTime, true);
        TBar.CrossFadeAlpha(1, fadeTime, true);
        HideDRAM.image.CrossFadeAlpha(1, (float)0, true);
        */
    }
    public void ShowGRAPHPane() {
        GRAPHSCNTRL.gameObject.SetActive(true);
        Close.gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }


}
