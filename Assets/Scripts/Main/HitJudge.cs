using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HitJudge : MonoBehaviour
{

    [SerializeField] MainManager mainManager;
    [SerializeField] GameObject[] JudgeMsgObj;
    [SerializeField] NotesManager notesManager;
    [SerializeField] SoundMain soundMain;

    [SerializeField] TextMeshProUGUI comboText;
    [SerializeField] TextMeshProUGUI scoreText;

    [SerializeField] GameObject hitEffect;


    [SerializeField] float PerfectSecond = 0.10f;
    [SerializeField] float GreatSecond = 0.15f;
    [SerializeField] float BadSecond = 0.20f;
    [SerializeField] float MissSecond = 0.20f;

    enum eStateKey
    {
        D, F, J, K,
    }

    bool[] touchKeyState = new bool[4] { false, false, false, false };
    bool[] pushingKeyState = new bool[4] { false, false, false, false };

    List<NoteData> longNoteDataList = new List<NoteData>();

    // Update is called once per frame
    void Update()
    {

        UpdatePushingKeyState();

        if (!GameManager.Instance.isStart || (notesManager.NoteDataAll.Count == 0 && longNoteDataList.Count == 0))
        {
            return;
        }

        NormalNoteCheck();
        LongNoteCheck();

    }

    private void FixedUpdate()
    {
        UpdateText();
    }

    void NormalNoteCheck()
    {
        if (!touchKeyState.Contains(true))
        {
            return;
        }

        // 通常ノーツ判定
        // レーンの総数分判定を行う
        for (int laneNum = 0; laneNum < touchKeyState.Length; laneNum++)
        {
            // 対象のレーンが押されているか
            if (!touchKeyState[laneNum])
            {
                break;
            }

            // タッチキーステートの総数分回す
            for (int noteTiming = 0; noteTiming < touchKeyState.Length; noteTiming++)
            {
                // ノーツの総数がノーツタイミング以下になったらやめる
                if (notesManager.NoteDataAll.Count - 1 < noteTiming)
                {
                    break;
                }

                // ノーツ情報のレーンと調べてるレーンが同一なら判定
                if (laneNum == notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - noteTiming].laneNum)
                {
                    // ロングノーツであるかどうか
                    if (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - noteTiming].type != 2)
                    {
                        CheckHitTiming(Mathf.Abs(Time.time - (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - noteTiming].time + mainManager.startTime)), notesManager.NoteDataAll.Count - 1 - noteTiming);

                    }
                    else
                    {
                        // ロングノーツの最初がミスではない場合
                        if (Mathf.Abs(Time.time - (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - noteTiming].time + mainManager.startTime)) <= MissSecond)
                        {
                            // ノーツデータをコピー
                            NoteData tmpNote = new NoteData(notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - noteTiming]);
                            longNoteDataList.Add(tmpNote);
                            // タイミングを計算
                            CheckHitTiming(Mathf.Abs(Time.time - (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - noteTiming].time + mainManager.startTime)), notesManager.NoteDataAll.Count - 1 - noteTiming);
                        }
                    }
                    break;
                }
            }
            soundMain.PlaySE((int)SoundMain.SE.Touch);
            
        }
        

        // 上で処理された物で最後なら処理しない
        if (notesManager.NoteDataAll.Count != 0)
        {

            // ミス判定
            if (Time.time - mainManager.startTime >= (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1].time + MissSecond))
            {
                PopupJudgeMsg(3, notesManager.NoteDataAll.Count - 1);
                if (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1].type == 2)
                {
                    DeleteData(notesManager.NoteDataAll.Count - 1, true);
                }
                else
                {
                    DeleteData(notesManager.NoteDataAll.Count - 1);
                }
                mainManager.ResetCombo();
                mainManager.AddJudgeCount(3);
            }
        }
    }

    void LongNoteCheck()
    {
        // ロングノーツが保存されている状態であればここに入る
        if (longNoteDataList.Count > 0)
        {
            // NoteDataを個別で見る
            for (int noteNum = longNoteDataList.Count - 1; noteNum >= 0; noteNum--)
            {
                // 途中でボタンを離した時判定
                if (!PushingKey() && Mathf.Abs(Time.time - (longNoteDataList[noteNum].longNotes[longNoteDataList[noteNum].longNotes.Count - 1].time + mainManager.startTime)) > BadSecond)
                {
                    foreach (LongNoteData longnote in longNoteDataList[noteNum].longNotes)
                    {
                        PopupJudgeLongMsg(3, longnote.laneNum);
                        mainManager.ResetCombo();
                        mainManager.AddJudgeCount(3);
                        Destroy(longnote.notes);

                    }
                    longNoteDataList.RemoveAt(noteNum);
                }
                else
                {
                    UpdateLongNotes(longNoteDataList[noteNum]);
                }
            }
            for (int i = longNoteDataList.Count - 1; i >= 0; i--)
            {
                if (longNoteDataList[i].isEnd)
                {
                    foreach (LongNoteData longnote in longNoteDataList[i].longNotes)
                    {
                        Destroy(longnote.notes);
                    }
                    longNoteDataList.RemoveAt(i);
                }
            }
        }
    }

    void CheckHitTiming(float timeLag, int offset, bool isLong = false)
    {
        if (timeLag <= PerfectSecond)
        {
            PopupJudgeMsg(0, offset);
            mainManager.AddCombo();
            mainManager.AddJudgeCount(0);
        }
        else if (timeLag <= GreatSecond)
        {
            PopupJudgeMsg(1, offset);
            mainManager.AddCombo();
            mainManager.AddJudgeCount(1);
        }
        else if (timeLag <= BadSecond)
        {
            PopupJudgeMsg(2, offset);
            mainManager.ResetCombo();
            mainManager.AddJudgeCount(2);
        }
        // ロングノーツである場合はすべて削除しない
        if (!isLong && timeLag <= BadSecond)
        {
            DeleteData(offset);
        }
        else if (isLong && timeLag <= BadSecond)
        {
            notesManager.NoteDataAll.RemoveAt(offset);
            Destroy(notesManager.NotesObj[offset]);
        }
    }

    void UpdateLongNotes(NoteData notedata)
    {
        for (int i = notedata.longNotes.Count - 1; i >= 0; i--)
        {
            // ロングノーツ中に出てくるノーツ判定
            if (i != notedata.longNotes.Count - 1)
            {
                // パーフェクト判定の範囲に入っていてまだ判定していない状態であれば処理
                if ((Mathf.Abs(Time.time - (notedata.longNotes[i].time + mainManager.startTime))) <= PerfectSecond && !notedata.longNotes[i].passnext)
                {
                    if (pushingKeyState[notedata.longNotes[i].laneNum])
                    {
                        PopupJudgeLongMsg(0, notedata.longNotes[i].laneNum);
                        mainManager.AddCombo();
                        mainManager.AddJudgeCount(0);
                        notedata.longNotes[i].passnext = true;
                        soundMain.PlaySE((int)SoundMain.SE.Touch);

                    }
                }
            }
            // 最後のロングノーツの判定
            else
            {
                if ((Mathf.Abs(Time.time - (notedata.longNotes[i].time + mainManager.startTime))) <= MissSecond && !notedata.isEnd)
                {
                    if (!pushingKeyState[notedata.longNotes[i].laneNum])
                    {
                        if ((Mathf.Abs(Time.time - (notedata.longNotes[i].time + mainManager.startTime)) <= PerfectSecond))
                        {
                            PopupJudgeLongMsg(0, notedata.longNotes[i].laneNum);
                            mainManager.AddCombo();
                            mainManager.AddJudgeCount(0);
                        }
                        else if ((Mathf.Abs(Time.time - (notedata.longNotes[i].time + mainManager.startTime)) <= GreatSecond))
                        {
                            PopupJudgeLongMsg(1, notedata.longNotes[i].laneNum);
                            mainManager.AddCombo();
                            mainManager.AddJudgeCount(1);
                        }
                        else if ((Mathf.Abs(Time.time - (notedata.longNotes[i].time + mainManager.startTime)) <= BadSecond))
                        {

                            PopupJudgeLongMsg(2, notedata.longNotes[i].laneNum);
                            mainManager.ResetCombo();
                            mainManager.AddJudgeCount(2);
                        }
                        else if ((Mathf.Abs(Time.time - (notedata.longNotes[i].time + mainManager.startTime)) <= MissSecond))
                        {
                            PopupJudgeLongMsg(3, notedata.longNotes[i].laneNum);
                            mainManager.ResetCombo();
                            mainManager.AddJudgeCount(3);
                        }
                        soundMain.PlaySE((int)SoundMain.SE.Touch);
                        notedata.isEnd = true;
                    }
                }
            }
        }

    }


    void UpdatePushingKeyState()
    {
        touchKeyState[(int)eStateKey.D] = Input.GetKeyDown(KeyCode.D);
        touchKeyState[(int)eStateKey.F] = Input.GetKeyDown(KeyCode.F);
        touchKeyState[(int)eStateKey.J] = Input.GetKeyDown(KeyCode.J);
        touchKeyState[(int)eStateKey.K] = Input.GetKeyDown(KeyCode.K);

        pushingKeyState[(int)eStateKey.D] = Input.GetKey(KeyCode.D);
        pushingKeyState[(int)eStateKey.F] = Input.GetKey(KeyCode.F);
        pushingKeyState[(int)eStateKey.J] = Input.GetKey(KeyCode.J);
        pushingKeyState[(int)eStateKey.K] = Input.GetKey(KeyCode.K);
    }

    void DeleteData(int offset, bool isPassLong = false)
    {
        if (isPassLong)
        {
            foreach (LongNoteData longnote in notesManager.NoteDataAll[offset].longNotes)
            {
                PopupJudgeLongMsg(3, longnote.laneNum);
                mainManager.ResetCombo();
                mainManager.AddJudgeCount(3);
                Destroy(longnote.notes);
            }
        }

        notesManager.NoteDataAll.RemoveAt(offset);
        Destroy(notesManager.NotesObj[offset]);
        notesManager.NotesObj.RemoveAt(offset);

        if (notesManager.NoteDataAll.Count <= 0)
        {
            GameManager.Instance.isEnd = true;
        }
    }

    void UpdateText()
    {
        comboText.text = "Combo\n" + mainManager.GetCombo().ToString();
        scoreText.text = "Score:" + mainManager.GetPoint().ToString();
    }
    void PopupJudgeMsg(int judge, int offset = 0)
    {
        // Instanceの削除処理はオブジェクトに記述
        Instantiate(JudgeMsgObj[judge], new Vector3(notesManager.NoteDataAll[offset].laneNum - 1.5f, 0.76f, 0.15f), Quaternion.Euler(45, 0, 0));

        if (judge != 3)
        {
            Instantiate(hitEffect, new Vector3(notesManager.NoteDataAll[offset].laneNum - 1.5f, 0.6f, 0f), Quaternion.Euler(90, 0, 0));
        }
    }
    void PopupJudgeLongMsg(int judge, int laneNum)
    {
        // Instanceの削除処理はオブジェクトに記述
        Instantiate(JudgeMsgObj[judge], new Vector3(laneNum - 1.5f, 0.76f, 0.15f), Quaternion.Euler(45, 0, 0));
    }


    bool PushingKey()
    {
        if (Input.anyKey)
        {
            return true;
        }
        return false;
    }
}
