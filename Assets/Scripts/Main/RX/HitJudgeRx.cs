using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

using UniRx;
using UniRx.Triggers;


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

    private IInputEventProvider _inputEventProvider;

    private readonly ReactiveProperty<Unit> _keyStateD = new ReactiveProperty<Unit>();
    private readonly ReactiveProperty<Unit> _keyStateF = new ReactiveProperty<Unit>();
    private readonly ReactiveProperty<Unit> _keyStateJ = new ReactiveProperty<Unit>();
    private readonly ReactiveProperty<Unit> _keyStateK = new ReactiveProperty<Unit>();

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

    void Start()
    {
        

        _inputEventProvider = GetComponent<IInputEventProvider>();

        SubscribeInputEvent();
    }

    void SubscribeInputEvent()
    {
        _inputEventProvider.OnKeyStateD
            .DistinctUntilChanged()
            .Subscribe(_ => {
                if (_)
                {
                    UpdateTapNote(0);
                }
                else
                {
                    UpdatePushingNote(0);
                }
        });
        
        _inputEventProvider.OnKeyStateF
            .DistinctUntilChanged()
            .Subscribe(_ => {
                if (_)
                {
                    UpdateTapNote(1);
                }
                else
                {
                    UpdatePushingNote(1);
                }
        });
        
        _inputEventProvider.OnKeyStateJ
            .DistinctUntilChanged()
            .Subscribe(_ => {
                if (_)
                {
                    UpdateTapNote(2);
                }
                else
                {
                    UpdatePushingNote(2);
                }
        });
        
        _inputEventProvider.OnKeyStateK
            .DistinctUntilChanged()
            .Subscribe(_ => {
                if (_)
                {
                    UpdateTapNote(3);
                }
                else
                {
                    UpdatePushingNote(3);
                }
        });
    }

    // Update is called once per frame
    void Update()
    {

        //UpdatePushingKeyState();

        if (!GameManager.Instance.isStart ||notesManager.NoteDataAll.Count == 0)
        {
            return;
        }


        for (int LaneNum = 3; LaneNum >= 0; LaneNum--)
        {
            if (notesManager.NoteDataAll.Count - 1 < LaneNum)
            {
                continue;
            }

/*            if (IsCheckSameLane(LaneNum))
            {
                continue;
            }*/
            // 押した瞬間
            //UpdateTapNote(LaneNum);
            // ロングノーツ長押し処理
            //UpdatePushingNote(LaneNum);
            // スルー処理
            UpdateOverLookedNote(LaneNum);
        }

        if (notesManager.NoteDataAll.Count == 0)
        {
            GameManager.Instance.isEnd = true;
        }

    }

    bool IsCheckSameLane(int count)
    {
        if (count == 0)
        {
            return false;
        }

        for (int i = count; i >= 0; i--)
        {
            if (i == count) continue;

            if (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count].GetLaneNum()
                ==
                notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - i].GetLaneNum())
            {
                return true;
            }
        }
        return false;
    }
/*
    void UpdateTapNote(int count)
    {
        // ボタンを押した瞬間の処理
        for (int laneNum = 3; laneNum >= 0; laneNum--)
        {
            // ロングノーツ押しているときの処理
            if (!touchKeyState[laneNum])
            {
                continue;
            }

            if (notesManager.NoteDataAll.Count < count)
            {
                continue;
            }

            if (laneNum != notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count].GetLaneNum())
            {
                continue;
            }

            if (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count].GetType() == typeof(LongNoteData)
                &&
                !((LongNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count]).GetIsPush())
            {
                LongNoteJudge(notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count].CalcTime(mainManager.startTime), laneNum, count);
                break;
            }

            if (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count].GetLaneNum() == laneNum)
            {
                NormalNoteJudge(notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count].CalcTime(mainManager.startTime), laneNum, count);
                break;
            }
        }
    }*/
    void UpdateTapNote(int laneNum)
    {
        // ボタンを押した瞬間の処理
        for (int count = 3; count >= 0; count--)
        {
            if (notesManager.NoteDataAll.Count < count)
            {
                continue;
            }

            if (laneNum != notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count].GetLaneNum())
            {
                continue;
            }

            if (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count].GetType() == typeof(LongNoteData)
                &&
                !((LongNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count]).GetIsPush())
            {
                LongNoteJudge(notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count].CalcTime(mainManager.startTime), laneNum, count);
                break;
            }

            if (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count].GetLaneNum() == laneNum)
            {
                NormalNoteJudge(notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count].CalcTime(mainManager.startTime), laneNum, count);
                break;
            }
        }
    }
/*
    void UpdatePushingNote(int count)
    {

        if (notesManager.NoteDataAll.Count < count)
        {
            return;
        }

        if (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count].GetType() != typeof(LongNoteData))
        {
            return;
        }

        if (!((LongNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count]).GetIsPush())
        {
            return;
        }

        // 1,レーン番号が違えばcontinue
        // 2,離した時 押した瞬間と長押しを管理している情報がfalseになれば長押ししたことにして処理
        for (int laneNum = 3; laneNum >= 0; laneNum--)
        {
            if (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count].GetLaneNum() != laneNum)
            {
                continue;
            }

            if (MissSecond + ((LongNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count]).GetLastBehindTime() <= Time.time - mainManager.startTime)
            {
                DeleteData(count);
                break;
            }

            // 長押し中であるかどうか
            if (touchKeyState[laneNum] == true || pushingKeyState[laneNum] == true)
            {
                break;
            }

            if (CheckHitTiming(((LongNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count]).GetLastBehindTime(mainManager.startTime), laneNum))
            {
                ((LongNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count]).DeleteAllObject();
                notesManager.NoteDataAll.RemoveAt(notesManager.NoteDataAll.Count - 1 - count);
                soundMain.PlaySE((int)SoundMain.SE.Touch);
                break;
            }
            else
            {
                DeleteData(count);
                break;
            }
        }
    }*/
    void UpdatePushingNote(int laneNum)
    {
        // 1,レーン番号が違えばcontinue
        // 2,離した時 押した瞬間と長押しを管理している情報がfalseになれば長押ししたことにして処理
        for (int count = 3; count >= 0; count--)
        {
            if (notesManager.NoteDataAll.Count < count)
            {
                continue;
            }

            if (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count].GetLaneNum() != laneNum)
            {
                continue;
            }

            if (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count].GetType() != typeof(LongNoteData))
            {
                continue;
            }
            
            if (!((LongNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count]).GetIsPush())
            {
                continue;
            }



/*            if (MissSecond + ((LongNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count]).GetLastBehindTime() <= Time.time - mainManager.startTime)
            {
                DeleteData(count);
                break;
            }*/

            if (CheckHitTiming(((LongNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count]).GetLastBehindTime(mainManager.startTime), laneNum))
            {
                ((LongNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count]).DeleteAllObject();
                notesManager.NoteDataAll.RemoveAt(notesManager.NoteDataAll.Count - 1 - count);
                soundMain.PlaySE((int)SoundMain.SE.Touch);
                break;
            }
            else
            {
                DeleteData(count);
                break;
            }
        }
    }

    void UpdateOverLookedNote(int count)
    {
        // ノーツ見逃し処理
        if (IsCheckMissTime(count))
        {
            if (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count].GetType() == typeof(LongNoteData)
                &&
                !((LongNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count]).GetIsPush())
            {
                DeleteData(count);
                return;
            }
            else if (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count].GetType() == typeof(LongNoteData))
            {
                return;
            }

            DeleteData(count);
        }
    }

    private void FixedUpdate()
    {
        UpdateText();
    }

    void LongNoteJudge(float time, int iLaneNum, int count)
    {
        if (CheckHitTiming(time, iLaneNum))
        {
            ((LongNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count]).DeleteOneObject(0);
            ((LongNoteData)notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count]).UpdateIsPush();
        }

        soundMain.PlaySE((int)SoundMain.SE.Touch);
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

    bool CheckHitTiming(float timeLag, int laneNum)
    {
        if (timeLag > MissSecond)
        {
            return false;
        }
        if (timeLag <= PerfectSecond)
        {
            PopupJudgeMsg(0, laneNum);
            mainManager.AddCombo();
            mainManager.AddJudgeCount(0);
        }
        else if (timeLag <= GreatSecond)
        {
            PopupJudgeMsg(1, laneNum);
            mainManager.AddCombo();
            mainManager.AddJudgeCount(1);
        }
        else if (timeLag <= BadSecond)
        {
            PopupJudgeMsg(2, laneNum);
            mainManager.ResetCombo();
            mainManager.AddJudgeCount(2);
        }
        else if (timeLag <= MissSecond)
        {
            PopupJudgeMsg(3, laneNum);
            mainManager.ResetCombo();
            mainManager.AddJudgeCount(3);
        }
        return true;
    }

    bool IsCheckMissTime(int count)
    {
        if (notesManager.NoteDataAll.Count == 0)
        {
            return false;
        }

        if ( MissSecond + notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - count].GetTime() <= Time.time - mainManager.startTime)
        {
            return true;
        }
        return false;
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

    void DeleteData(int offset)
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
    }

    void UpdateText()
    {
        comboText.text = "Combo\n" + mainManager.GetCombo().ToString();
        scoreText.text = "Score:" + mainManager.GetPoint().ToString();
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
