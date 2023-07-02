
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

using LitJson;

public abstract class BaseNoteData
{
    protected int type;
    protected float time;
    protected int laneNum;
    protected float LPB;
    protected float objPosz;

    abstract public void SetNoteObj(GameObject note);

    public float CalcTime(float startTime)
    {
        return Mathf.Abs(Time.time - (this.time + startTime));
    }

    public void SetObjPosz(float noteSpeed)
    {
        this.objPosz = this.time * noteSpeed;
    }

    public float GetTime()
    {
        return this.time;
    }
    public int GetNoteType()
    {
        return this.type;
    }
    public float GetLPB()
    {
        return this.LPB;
    }
    public int GetLaneNum()
    {
        return this.laneNum;
    }
    
}

public class NormalNoteData:BaseNoteData
{
    public GameObject note;
    public NormalNoteData(int type, float time, int laneNum, float LPB)
    {
        this.type = type;
        this.time = time;
        this.laneNum = laneNum;
        this.LPB = LPB;
    }

    public override void SetNoteObj(GameObject note)
    {
        this.note = note;
    }

    public Vector3 GetNotePos()
    {
        return note.transform.position;
    }

    public float GetObjPosz()
    {
        return objPosz;
    }

    public void DeleteMyObject()
    {
        UnityEngine.Object.Destroy(note);
    }

}

public class LongNoteData:BaseNoteData
{
    public List<GameObject> notes     = new List<GameObject>();
    public List<GameObject> noteTrail = new List<GameObject>();

    public List<BaseNoteData> behindNotes = new List<BaseNoteData>();

    public LongNoteData(int type, float time, int laneNum, float LPB)
    {
        this.type = type;
        this.time = time;
        this.laneNum = laneNum;
        this.LPB = LPB;
    }

    public override void SetNoteObj(GameObject note)
    {
        this.notes.Add(note);
    }

    public void SetNoteTrail(GameObject noteTrail)
    {
        this.noteTrail.Add(noteTrail);
    }

    public Vector3 GetNotePos(int count)
    {
        return notes[count].transform.position;
    }

    public void SetBehindNotesStatus(BaseNoteData behindNote)
    {
        behindNotes.Add(behindNote);
    }


    public void DeleteOneObject(int count)
    {
        UnityEngine.Object.Destroy(notes[count]);
        notes.RemoveAt(count);
    }
    public void DeleteAllObject()
    {
        for (int i = notes.Count - 1; i >= 0; i--)
        {
            UnityEngine.Object.Destroy(notes[i]);
            notes.RemoveAt(i);
        }
    }
}

enum NotesType
{
    None,
    NormalNotes,
    LongNotes
}

public class NotesManager : MonoBehaviour
{
    [SerializeField] GameObject _noteObj;
    [SerializeField] Mesh _longNoteTrail;
    public int noteNum;

    [SerializeField] MainManager mainManager;
    [SerializeField] Material NotesLineMaterial;

    public List<BaseNoteData> NoteDataAll = new List<BaseNoteData>();


    [SerializeField] float NotesSpeed;

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

            Vector3 pos = new Vector3(int.Parse(BLOCK) - 1.5f, 0.55f, NotesSpeed * time);
            // 通常のノーツ作成処理
            if (TYPE.Equals("1"))
            {
                BaseNoteData noteData = new NormalNoteData(int.Parse(TYPE), time, int.Parse(BLOCK), float.Parse(LPB));
                
                noteData.SetNoteObj((GameObject)Instantiate(_noteObj, pos, Quaternion.identity));
                noteData.SetObjPosz(NotesSpeed);
                NoteDataAll.Add(noteData);
                continue;
            }


            // 最初のロングノーツはここで作成
            BaseNoteData longNoteData = new LongNoteData(int.Parse(TYPE), time, int.Parse(BLOCK), float.Parse(LPB));
            longNoteData.SetNoteObj((GameObject)Instantiate(_noteObj, pos, Quaternion.identity));
            longNoteData.SetObjPosz(NotesSpeed);
            NoteDataAll.Add(longNoteData);

            // 残りのロングノーツ作成処理
            for (int j = 0; j < jsonData["notes"][i]["notes"].Count; j++)
            {

                LPB = jsonData["notes"][i]["notes"][j]["LPB"].ToString();
                NUM = jsonData["notes"][i]["notes"][j]["num"].ToString();
                BLOCK = jsonData["notes"][i]["notes"][j]["block"].ToString();
                TYPE = jsonData["notes"][i]["notes"][j]["type"].ToString();

                space = 60 / (float.Parse(BPM) * float.Parse(LPB));
                beatSec = space * float.Parse(LPB);
                time = (beatSec * float.Parse(NUM) / float.Parse(LPB) + float.Parse(OFFSET) * 0.01f);

                
                longNoteData.SetObjPosz(NotesSpeed);
                noteNum++;

                Vector3 subNotepos = new Vector3(int.Parse(BLOCK) - 1.5f, 0.55f, NotesSpeed * time);
                ((LongNoteData)longNoteData).SetNoteObj((GameObject)Instantiate(_noteObj, subNotepos, Quaternion.identity));
                LongNotesCreate(((LongNoteData)longNoteData).GetNotePos(j), ((LongNoteData)longNoteData).GetNotePos(j+1), ((LongNoteData)longNoteData).notes[j+1]);
            }
        }
        mainManager.maxScore = noteNum * mainManager.MAX_RAITO_POINT;
        Debug.Log(noteNum);
    }

    private const float LANE_WIDTH = 1.0f;
    private const float NOTES_SIZE_OFFSET = 0.05f;  // ロングノーツの軌道サイズを調整
    private void LongNotesCreate(Vector3 start, Vector3 end, GameObject notes)
    {


        GameObject longNotesLine = new GameObject();
        longNotesLine.AddComponent<MeshFilter>();
        longNotesLine.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        longNotesLine.GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[6];

        vertices[0] = start + new Vector3(-LANE_WIDTH / 2.0f + NOTES_SIZE_OFFSET, 0, 0);
        vertices[1] = start + new Vector3(LANE_WIDTH / 2.0f - NOTES_SIZE_OFFSET, 0, 0);
        vertices[2] = end + new Vector3(LANE_WIDTH / 2.0f - NOTES_SIZE_OFFSET, 0, 0);
        vertices[3] = end + new Vector3(-LANE_WIDTH / 2.0f + NOTES_SIZE_OFFSET, 0, 0);
        //vertices[2] = end + new Vector3(LANE_WIDTH / 2.0f - NOTES_SIZE_OFFSET, 0, 0);
        //vertices[3] = end + new Vector3(-LANE_WIDTH / 2.0f + NOTES_SIZE_OFFSET, 0, 0);

        triangles = new int[6] { 0, 2, 1, 3, 2, 0 };

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        longNotesLine.GetComponent<Renderer>().material = NotesLineMaterial;

        mesh.RecalculateNormals();

        longNotesLine.transform.parent = notes.transform;
    }
}