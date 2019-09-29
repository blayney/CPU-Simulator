using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SendText : MonoBehaviour
{
    public Button goButton;
    public Text writeSheet;
    public Text console;
    public InputField cmdln;


    // Start is called before the first frame update
    void Start()
    {
        
        Button btn = goButton.GetComponent<Button>();
        
        btn.onClick.AddListener(SendTextRX);
    }
    void SendTextRX() {
        Button btn = goButton.GetComponent<Button>();
        Text ws = transform.Find("writesheet").GetComponent<Text>();
        Text cs = transform.Find("console").GetComponent<Text>();
        ws.text = transform.Find("command").GetComponent<InputField>().text;
    }
    // Update is called once per frame

    void Update()
    {
        
    }
}
