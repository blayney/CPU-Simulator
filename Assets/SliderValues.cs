using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderValues : MonoBehaviour
{

    public Slider CVSlider, PLSlider, TLSlider, CCSlider, MCSlider, FSSlider;
    public Text CVText, PLText, TLText, CCText, MCText, FSText;
    // Start is called before the first frame update
    void Start()
    {
        CVSlider.onValueChanged.AddListener(delegate { CVSliderMoved(); });
        PLSlider.onValueChanged.AddListener(delegate { PLSliderMoved(); });
        TLSlider.onValueChanged.AddListener(delegate { TLSliderMoved(); });
        CCSlider.onValueChanged.AddListener(delegate { CCSliderMoved(); });
        MCSlider.onValueChanged.AddListener(delegate { MCSliderMoved(); });
        FSSlider.onValueChanged.AddListener(delegate { FSSliderMoved(); });
    }

    public void CVSliderMoved() {
        CVText.text = CVSlider.value.ToString("0.0");
    }
    public void PLSliderMoved()
    {
        PLText.text = PLSlider.value.ToString();
    }
    public void TLSliderMoved() {
        TLText.text = TLSlider.value.ToString();
    }
    public void CCSliderMoved() {
        CCText.text = CCSlider.value.ToString();
    }
    public void MCSliderMoved() {
        MCText.text = MCSlider.value.ToString();

    }
    public void FSSliderMoved() {
        FSText.text = FSSlider.value.ToString("00.0");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
