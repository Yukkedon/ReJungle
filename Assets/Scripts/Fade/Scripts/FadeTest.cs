using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class FadeTest : MonoBehaviour
{
    public Fade fade;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {/*
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }*/
        
        if(Input.GetMouseButtonDown(0))
        {
           // fade.FadeIn( 10f, () => SceneManager.LoadScene("SelectScene"));
        }
    }

    public void ChangeSceneButton(string name)
    {
        fade.FadeIn( 1f, () => SceneManager.LoadScene(name));
    }

    
}

      /*  else if ( Input.GetMouseButtonDown(0))
        {
            fade.FadeIn( 0.5f);
        }*/

         //fade.FadeOut( 1f, () => SceneManager.LoadScene("MainScene"));
/*
                if ( Input.GetKeyDown(KeyCode.Z ))
        {
            fade.FadeIn( 0.5f, () => SceneManager.LoadScene("SelectScene"));
        }*/

       

