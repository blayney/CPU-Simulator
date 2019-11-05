using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class loadingBar : MonoBehaviour
{
    public Slider slidy;
    public Button exec;
    public void Start(){
        exec.onClick.AddListener(delegate {LoadLevel(1);});

    }
    public int sceneIndex = 1;
    public void LoadLevel (int sceneIndex){
        StartCoroutine(LoadAsync(1));
    }

    IEnumerator LoadAsync (int sceneIndex){
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);
        while(!op.isDone){
            float progress = Mathf.Clamp01(op.progress / .9f);
            slidy.value = progress;
            yield return null;
        }
        
    }
}
