using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SceneManagementReverse : MonoBehaviour
{
    public Slider Slidy;
    public Button SwitchButton;

    // Start is called before the first frame update
    void Start()
    {
        SwitchButton.onClick.AddListener(delegate { LoadLevel(0); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

 
    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsync(0));
    }

    IEnumerator LoadAsync(int sceneIndex)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(0);
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / .9f);
            Slidy.value = progress;
            yield return null;
        }
       

    }
}