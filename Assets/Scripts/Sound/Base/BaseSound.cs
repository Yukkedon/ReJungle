using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class BaseSound : MonoBehaviour
{

    [SerializeField] AudioSource bgmSource;
    public List<AudioClip> bgmClip;

    [SerializeField] AudioSource seSuorce;
    [SerializeField] List<AudioClip> seClip;

    bool isPlayedMusic = false;

    protected void LoadMusic(string songName)
    {
        bgmSource.clip = (AudioClip)Resources.Load($"Musics/{songName}");
    }

    protected void LoadCutMusic()
    {
        Object[] clips = Resources.LoadAll("CutMusic", typeof(AudioClip));

        foreach (AudioClip clip in clips)
        {
            bgmClip.Add(clip);
        }
    }

    public List<string> GetMusicNames()
    {
        List<string> nameList = new List<string>();

        foreach (AudioClip clip in bgmClip)
        {
            Debug.Log(clip.name);
            nameList.Add(clip.name);
        }

        return nameList;
    }

    public void PlayBGM()
    {
        isPlayedMusic = true;
        bgmSource.Play();
    }

    public void PlayBGM(int num)
    {
        bgmSource.clip = bgmClip[num];
        bgmSource.Play();
    }

    public void PlaySE(int num)
    {
        seSuorce.PlayOneShot(seClip[num]);
    }

    public bool IsCheckEndBGM()
    {
        if (!isPlayedMusic)
        {
            return true;
        }
        return false;
    }

    public bool IsEndBGM()
    {
        return bgmSource.isPlaying;
    }
   
}
