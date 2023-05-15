using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using LitJson;


public class SelectViewer : MonoBehaviour
{
    public List<string> musicName = new List<string>();

    [SerializeField] ScrollView scrollview;

    List<Sprite> images = new List<Sprite>();

    TextAsset json;
    
    void Awake()
    {
        LoadJacketImages();
    }

    void LoadJacketImages()
    {
        Object[] jackets = Resources.LoadAll("JacketImages", typeof(Sprite));

        foreach (Sprite jacket in jackets)
        {
            images.Add(jacket);
            musicName.Add(jacket.name);
        }
    }

    void LoadNameList()
    {
        json = Resources.Load("NameList") as TextAsset;

        JsonData jsonData = JsonMapper.ToObject(json.text);

        for (int i = 0; i < jsonData.Count; i++)
        {
            musicName.Add(jsonData[i].ToString());
        }
    }

    void Start()
    {
        StartCoroutine(CreateScrollObj());
    }

    IEnumerator CreateScrollObj()
    {
        yield return new WaitForSeconds(2f);
        var items = Enumerable.Range(0, musicName.Count).
          Select(i => new MusicItemData(i, musicName[i], images[i])).ToArray();
        scrollview.UpdateData(items);
    }
}
