using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyScrollView;

class MusicItemData
{
    public int    id;
    public string musicName;
    public Sprite image;

    public MusicItemData(int id ,string musicName,Sprite image)
    {
        this.id        = id;
        this.musicName = musicName;
        this.image     = image;
    }
}

class ScrollView : FancyScrollView<MusicItemData>
{
    [SerializeField] Scroller   _scroller;
    [SerializeField] GameObject _cellPrefab;

    protected override GameObject CellPrefab => _cellPrefab;

    protected override void Initialize()
    {
        _scroller.OnValueChanged(UpdatePosition);
    }

    public void UpdateData(IList<MusicItemData> music)
    {
        UpdateContents(music)
;       _scroller.SetTotalCount(music.Count);
    }
}
