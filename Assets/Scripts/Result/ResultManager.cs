using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [SerializeField]Fade fade;
    public void ChangeSceneButton(string name)
    {
        fade.FadeIn( 1f, () => SceneManager.LoadScene(name));
    }
}
