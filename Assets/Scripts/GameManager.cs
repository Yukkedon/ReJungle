using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;


    public string songName = "GurennoYumiya";


    public int MAX_RAITO_POINT = 5;
    public int MAX_DIGIT_POINT = 1000000;
    public int PERFECT_POINT = 5;

    public int MAX_COMBO = 0;

    public int point = 0;
    public int combo = 0;

    public int perfect = 0;
    public int great = 0;
    public int bad = 0;
    public int miss = 0;

    public bool isStart = false;
    public bool isEnd = false;
    
    private void Awake()
    {
        isStart = false;
        isEnd = false;

        if (Instance == null)
        {
            Instance = this;//���̃C���X�^���X��static �� instance�ɓo�^
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);//�Q��ڈȍ~�d�����č쐬���Ă��܂���gameObject���폜
        }
    }

    public void ResetState()
    {
        isStart = false;
        isEnd = false;
    }
}
