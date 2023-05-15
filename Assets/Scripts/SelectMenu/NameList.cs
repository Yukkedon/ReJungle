using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class NameList
{
    List<string> nameList = new List<string>();

    public NameList()
    {
        LoadNameList();
    }

    void LoadNameList()
    {

        TextAsset json = Resources.Load("NameList") as TextAsset;

        JsonData jsonData = JsonMapper.ToObject(json.text);

        for (int i = 0; i < jsonData.Count; i++)
        {
            nameList.Add(jsonData[i].ToString());
        }
    }

    public List<string> getNameList()
    {
        return nameList;
    }
}
