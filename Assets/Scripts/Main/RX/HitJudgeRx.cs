using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HitJudgeRx : MonoBehaviour
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
    
    enum eTiming
    {
        Perfect,
        Great,
        Bad,
        Miss,
    }

    bool[] touchKeyState = new bool[4] { false, false, false, false };
    bool[] pushingKeyState = new bool[4] { false, false, false, false };


    // Update is called once per frame
    void Update()
    {

        UpdatePushingKeyState();

        if (!GameManager.Instance.isStart ||notesManager.NoteDataAll.Count == 0)
        {
            return;
        }

        // 各押されているボタンを参照して対応するレーン番号を処理する
        for (int i = 3; i >= 0; i--)
        {
            // ロングノーツ押しているときの処理
            if (!touchKeyState[i])
            {
                continue;
            }

            // ノーツの種類によって処理する先を変更する
            for (int j = 0; j <= 3; j++)
            {
                if (notesManager.NoteDataAll.Count < j)
                {
                    continue;
                }
                if (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - j].GetType() == typeof(LongNoteData))
                {
                    LongNoteJudge(notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - j].CalcTime(mainManager.startTime), i, j);
                    break;
                }

                if (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - j].GetLaneNum() == i)
                {
                    NormalNoteJudge(notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - j].CalcTime(mainManager.startTime), i,j);
                    break;
                }
            }
        }

        // ロングノーツを押している・離した時の処理
        for (int i = 3; i >= 0; i--)
        {

            for (int j = 3; j >= 0; j--)
            {
                // ロングノーツ押しているときの処理
                if (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - j].GetType() != typeof(LongNoteData))
                {
                    continue;
                }

                if (!((LongNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - j]).GetIsPush())
                {
                    continue;
                }

                if (pushingKeyState[i])
                {
                    continue;
                }

                if (CheckHitTiming(((LongNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - j]).GetBehindTime(mainManager.startTime)
                     , ((LongNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - j]).behindNotes[0].GetLaneNum()))
                {
                    ((LongNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - j]).DeleteAllObject();
                    ((LongNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - j]).DeleteOneBehindData();
                    notesManager.NoteDataAll.RemoveAt(notesManager.NoteDataAll.Count - 1 - j);
                }
                else
                {
                    DeleteData(i);
                }

            }


        }

        // ボタンが押されていない場合
        /*
        if (!pushingKeyState.Contains(true))
        {
            for (int i = 3; i >= 0; i--)
            {
                if (notesManager.NoteDataAll.Count < i)
                {
                    continue;
                }
                if (IsCheckMissTime(i))
                {
                    Debug.Log("ボタン押されていない削除");
                    PopupJudgeMsg((int)eTiming.Miss, notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - i].GetLaneNum());
                    DeleteData(i);
                }
            }
            return;
        }*/

        // ノーツ情報をレーンの数分調べ「経過時間 + ミスの時間」
        for (int i = 3; i >= 0; i--)
        {
            if (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - i].GetType() == typeof(LongNoteData))
            {
                if (((LongNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - i]).GetIsPush())
                {
                    continue;
                }
            }

            if (IsCheckMissTime(i))
            {   
                DeleteData(i);
            }
        }
    }

    private void FixedUpdate()
    {
        UpdateText();
    }

    void NormalNoteJudge(float time,int iLaneNum,int notedataOffset)
    {
        if (CheckHitTiming(time, iLaneNum))
        {
            ((NormalNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - notedataOffset]).DeleteMyObject();
            notesManager.NoteDataAll.RemoveAt(notesManager.NoteDataAll.Count - 1 - notedataOffset);
        }

        soundMain.PlaySE((int)SoundMain.SE.Touch);
    }

    void LongNoteJudge(float time,int iLaneNum,int notedataOffset)
    {
        if (CheckHitTiming(time, iLaneNum))
        {
            ((LongNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - notedataOffset]).DeleteOneObject(0);
            ((LongNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - notedataOffset]).UpdateIsPush();
        }

        soundMain.PlaySE((int)SoundMain.SE.Touch);
    }


    bool CheckHitTiming(float timeLag, int offset)
    {
        if (timeLag > MissSecond)
        {
            return false;
        }
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
        else if (timeLag <= MissSecond)
        {
            PopupJudgeMsg(3, offset);
            mainManager.ResetCombo();
            mainManager.AddJudgeCount(3);
        }
        return true;
    }

    bool IsCheckMissTime(int count)
    {
        if ( MissSecond + notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count].GetTime() <= Time.time - mainManager.startTime)
        {
            return true;
        }
        return false;
    }

    float CalcHitTiming(float time)
    {
        return Mathf.Abs(Time.time - (time + mainManager.startTime));
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
        PopupJudgeMsg((int)eTiming.Miss, notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - offset].GetLaneNum());
        if (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - offset].GetType() == typeof(NormalNoteData))
        {
            ((NormalNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - offset]).DeleteMyObject();
            notesManager.NoteDataAll.RemoveAt(notesManager.NoteDataAll.Count - 1 - offset);
        }
        else if (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - offset].GetType() == typeof(LongNoteData))
        {
            ((LongNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - offset]).DeleteAllObject();
            notesManager.NoteDataAll.RemoveAt(notesManager.NoteDataAll.Count - 1 - offset);
        }
        

        if (notesManager.NoteDataAll.Count <= 0)
        {
            GameManager.Instance.isEnd = true;
        }
    }

    void UpdateText()
    {
        //comboText.text = "Combo\n" + mainManager.GetCombo().ToString();
        //scoreText.text = "Score:" + mainManager.GetPoint().ToString();
    }
    void PopupJudgeMsg(int judge, int laneNum)
    {
        // Instanceの削除処理はオブジェクト(prefab化したStringオブジェクト)に記述
        Instantiate(JudgeMsgObj[judge], new Vector3(laneNum - 1.5f, 0.76f, 0.15f), Quaternion.Euler(45, 0, 0));

        if (judge != 3)
        {
            Instantiate(hitEffect, new Vector3(laneNum - 1.5f, 0.6f, 0f), Quaternion.Euler(90, 0, 0));
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
