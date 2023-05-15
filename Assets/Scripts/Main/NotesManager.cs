using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

using LitJson;


public class NoteData
{
    public int type;
    public float time;
    public int laneNum;
    public float LPB;
    public List<LongNoteData> longNotes;
    public bool isEnd = false;     // ロングノーツ判定用

    public NoteData(NoteData noteData)
    {
        type = noteData.type;
        time = noteData.time;
        laneNum = noteData.laneNum;
        LPB = noteData.LPB;
        longNotes = new List<LongNoteData>(noteData.longNotes);

    }

    public NoteData(int type, float time, int laneNum, float LPB)
    {
        this.type = type;
        this.time = time;
        this.laneNum = laneNum;
        this.LPB = LPB;


        if (this.type == 2)
        {
            longNotes = new List<LongNoteData>();
        }
    }
}

public class LongNoteData
{
    public int type;
    public float time;
    public int laneNum;
    public float LPB;
    public bool passnext = false;


    public LongNoteData(LongNoteData noteData)
    {
        this.type = noteData.type;
        this.time = noteData.time;
        this.laneNum = noteData.laneNum;
        this.LPB = noteData.LPB;
    }
    public LongNoteData(int type, float time, int laneNum, float LPB)
    {
        this.type = type;
        this.time = time;
        this.laneNum = laneNum;
        this.LPB = LPB;
    }

    public GameObject notes;
}

enum NotesType
{
    None,
    NormalNotes,
    LongNotes
}

public class NotesManager : MonoBehaviour
{
    public int noteNum;

    [SerializeField] MainManager mainManager;
    [SerializeField] Material NotesLineMaterial;

    public List<NoteData> NoteDataAll = new List<NoteData>();
    public List<GameObject> NotesObj = new List<GameObject>();


    [SerializeField] float NotesSpeed;
    [SerializeField] GameObject noteObj;

    void Start()
    {
        noteNum = 0;
        Load(GameManager.Instance.songName);
        GameManager.Instance.MAX_COMBO = noteNum;
    }

    private void Load(string SongName)
    {
        TextAsset json;
        json = Resources.Load($"Scores/{GameManager.Instance.songName}",typeof(TextAsset)) as TextAsset;

        JsonData jsonData = JsonMapper.ToObject(json.ToString());


        noteNum = jsonData["notes"].Count;

        string BPM = jsonData["BPM"].ToString();
        string OFFSET = jsonData["offset"].ToString();

        for (int i = jsonData["notes"].Count - 1; i >= 0; i--)
        {

            // JsonDataクラスで(flaot)(int)キャストするとエラーになるため
            // 一時的に保存する変数を作成
            string LPB = jsonData["notes"][i]["LPB"].ToString();
            string NUM = jsonData["notes"][i]["num"].ToString();
            string BLOCK = jsonData["notes"][i]["block"].ToString();
            string TYPE = jsonData["notes"][i]["type"].ToString();


            float space = 60 / (float.Parse(BPM) * float.Parse(LPB));
            float beatSec = space * float.Parse(LPB);
            float time = (beatSec * float.Parse(NUM) / float.Parse(LPB) + float.Parse(OFFSET) * 0.01f);

            NoteData noteData = new NoteData(int.Parse(TYPE), time, int.Parse(BLOCK), float.Parse(LPB));


            float z = noteData.time * NotesSpeed;
            NotesObj.Add(Instantiate(noteObj, new Vector3(noteData.laneNum - 1.5f, 0.55f, z), Quaternion.identity));
            NoteDataAll.Add(noteData);

            // ロングノーツ判定
            if (noteData.type == 2)
            {
                // ロングノーツ作成処理
                for (int j = 0; j < jsonData["notes"][i]["notes"].Count; j++)
                {

                    LPB = jsonData["notes"][i]["notes"][j]["LPB"].ToString();
                    NUM = jsonData["notes"][i]["notes"][j]["num"].ToString();
                    BLOCK = jsonData["notes"][i]["notes"][j]["block"].ToString();
                    TYPE = jsonData["notes"][i]["notes"][j]["type"].ToString();

                    space = 60 / (float.Parse(BPM) * float.Parse(LPB));
                    beatSec = space * float.Parse(LPB);
                    time = (beatSec * float.Parse(NUM) / float.Parse(LPB) + float.Parse(OFFSET) * 0.01f);

                    noteData = new NoteData(int.Parse(TYPE), time, int.Parse(BLOCK), float.Parse(LPB));

                    z = noteData.time * NotesSpeed;

                    noteNum++;

                    LongNoteData longnoteData = new LongNoteData(int.Parse(TYPE), time, int.Parse(BLOCK), float.Parse(LPB));
                    longnoteData.notes = (GameObject)Instantiate(noteObj, new Vector3(noteData.laneNum - 1.5f, 0.55f, z), Quaternion.identity);
                    NoteDataAll[NoteDataAll.Count - 1].longNotes.Add(longnoteData);

                    // 最初に軌道する場合はこちら
                    if (j == 0)
                    {
                        LongNotesCreate(NotesObj[NotesObj.Count - 1].transform, longnoteData.notes.transform, longnoteData.notes);
                    }
                    else
                    {
                        // 2個以上は1つ前のノーツ情報を取得するため j - 1 で指定している
                        LongNotesCreate(NoteDataAll[NoteDataAll.Count - 1].longNotes[j - 1].notes.transform, longnoteData.notes.transform, longnoteData.notes);
                    }
                }

            }
        }
        mainManager.maxScore = noteNum * mainManager.MAX_RAITO_POINT;
    }

    private const float LANE_WIDTH = 1.0f;
    private const float NOTES_SIZE_OFFSET = 0.05f;  // ロングノーツの軌道サイズを調整
    private void LongNotesCreate(Transform start, Transform end, GameObject notes)
    {


        GameObject longNotesLine = new GameObject();
        longNotesLine.AddComponent<MeshFilter>();
        longNotesLine.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        longNotesLine.GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[6];

        vertices[0] = start.position + new Vector3(-LANE_WIDTH / 2.0f + NOTES_SIZE_OFFSET, 0, 0);
        vertices[1] = start.position + new Vector3(LANE_WIDTH / 2.0f - NOTES_SIZE_OFFSET, 0, 0);
        vertices[2] = end.position + new Vector3(LANE_WIDTH / 2.0f - NOTES_SIZE_OFFSET, 0, 0);
        vertices[3] = end.position + new Vector3(-LANE_WIDTH / 2.0f + NOTES_SIZE_OFFSET, 0, 0);

        triangles = new int[6] { 0, 2, 1, 3, 2, 0 };

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        longNotesLine.GetComponent<Renderer>().material = NotesLineMaterial;

        mesh.RecalculateNormals();

        longNotesLine.transform.parent = notes.transform;
    }
}