using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitScenes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!Application.isEditor)
        {
            // Only specifying the sceneName or sceneBuildIndex will load the Scene with the Single mode
            SceneManager.LoadScene("Verkauf", LoadSceneMode.Additive);
        SceneManager.LoadScene("Werkstatt", LoadSceneMode.Additive);
        SceneManager.LoadScene("Lager", LoadSceneMode.Additive);
        SceneManager.LoadScene("DemoScene", LoadSceneMode.Additive);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
