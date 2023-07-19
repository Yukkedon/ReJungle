using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SoundMain : BaseSound
{
    [SerializeField] MainManager mainManager;

    public enum SE
    {
        Touch,
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadMusic(mainManager.songName);
    }

    // Update is called once per frame
    void Update()
    {


        if (GameManager.Instance.isStart && !GameManager.Instance.isEnd && IsCheckEndBGM())
        {
            PlayBGM();
            return;
        }

        if (!IsEndBGM() && GameManager.Instance.isStart)
        {
            GameManager.Instance.isEnd = true;
        }

    }
}
