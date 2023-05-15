using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkyBoxManager : MonoBehaviour
{
    [Tooltip("�J�����̉�]�X�s�[�h")]
    [Range(0.01f, 1f)]
    [SerializeField] float rotateSpeed = 0.01f;

    // �X�J�C�{�b�N�X�}�e���A��
    [SerializeField] Material sky;

    // skybox�̃u�����h�l
    float alphaValue = 0f;

    Camera cam;

    // �J�����̉�]
    Vector3 newAngle = new Vector3(0f, 0f, 0f);


    void Start()
    {
        cam = Camera.main;
        
        alphaValue = 0f;
        sky.SetFloat("_value", alphaValue);
    }

    void Update()
    {
        // �t�F�[�h���l��1�bskybox�̍X�V������҂�
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
