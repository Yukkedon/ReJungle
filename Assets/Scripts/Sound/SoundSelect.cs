using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSelect : BaseSound
{
    public enum SE
    {
        Select,
        Cansel,
    }
    // Start is called before the first frame update
    void Start()
    {
        LoadCutMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
