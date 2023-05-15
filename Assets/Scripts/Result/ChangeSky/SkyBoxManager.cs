using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkyBoxManager : MonoBehaviour
{
    [Tooltip("カメラの回転スピード")]
    [Range(0.01f, 1f)]
    [SerializeField] float rotateSpeed = 0.01f;

    // スカイボックスマテリアル
    [SerializeField] Material sky;

    // skyboxのブレンド値
    float alphaValue = 0f;

    Camera cam;

    // カメラの回転
    Vector3 newAngle = new Vector3(0f, 0f, 0f);


    void Start()
    {
        cam = Camera.main;
        
        alphaValue = 0f;
        sky.SetFloat("_value", alphaValue);
    }

    void Update()
    {
        // フェードを考え1秒skyboxの更新処理を待つ
        Invoke("ChangeSkyBox", 1);
        
        CamRotate();
    }

    void CamRotate()
    {
        newAngle.y += rotateSpeed;

        cam.gameObject.transform.localEulerAngles = newAngle;
    }

    void ChangeSkyBox()
    {
        if (SceneManager.GetActiveScene().name == "ResultScene")
        {
            sky.SetFloat("_value", alphaValue);

            if (alphaValue <= 1)
            {
                alphaValue += 0.005f;
            }
        }
    }
}
