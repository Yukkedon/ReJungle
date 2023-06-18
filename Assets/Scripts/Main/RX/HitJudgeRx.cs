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
        None,
        Perfect,
        Great,
        Bad,
        Miss,
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

        NormalNoteJudge();
        LongNoteJudge();

    }

    private void FixedUpdate()
    {
        UpdateText();
    }

    void NormalNoteJudge()
    {
        if (!touchKeyState.Contains(true))
        {
            return;
        }

        // �e���[�����Q��
        for (int laneNum = 0; laneNum < touchKeyState.Length; laneNum++)
        {
            // �Ώۂ̃��[����������Ă��邩
            if (!touchKeyState[laneNum])
            {
                continue;
            }

            for (int noteCount = 0; noteCount < touchKeyState.Length; noteCount++)
            {
                // �m�[�c�̑�����4�ȉ��ɂȂ������߂�
                if (notesManager.NoteDataAll.Count - 1 < noteCount)
                {
                    break;
                }

                // �m�[�c���̃��[���ƒ��ׂĂ郌�[��������Ȃ画��
                if (laneNum == notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - noteCount].laneNum)
                {
                    CheckNoteType(noteCount);
                    break;
                }
            }
            soundMain.PlaySE((int)SoundMain.SE.Touch);

        }


        // ��ŏ������ꂽ���ōŌ�Ȃ珈�����Ȃ�
        if (notesManager.NoteDataAll.Count != 0)
        {

            // �~�X����
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

    void LongNoteJudge()
    {
        // �����O�m�[�c���ۑ�����Ă����Ԃł���΂����ɓ���
        if (longNoteDataList.Count == 0)
        {
            return;
        }
        // NoteData���ʂŌ���
        for (int noteNum = longNoteDataList.Count - 1; noteNum >= 0; noteNum--)
        {
            // �r���Ń{�^���𗣂���������
            if (!PushingKey() && CalcHitTiming(longNoteDataList[noteNum].longNotes[longNoteDataList[noteNum].longNotes.Count - 1].time) > BadSecond)
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

    void CheckNoteType(int noteCount)
    {
        // �����O�m�[�c�ł��邩�ǂ���
        if (notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - noteCount].type != 2)
        {
            CheckHitTiming(CalcHitTiming(notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - noteCount].time), notesManager.NoteDataAll.Count - 1 - noteCount);
        }
        else
        {
            // �����O�m�[�c�̍ŏ����~�X�ł͂Ȃ��ꍇ
            if (CalcHitTiming(notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - noteCount].time) <= MissSecond)
            {
                // �m�[�c�f�[�^���R�s�[
                NoteData tmpNote = new NoteData(notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - noteCount]);
                longNoteDataList.Add(tmpNote);
                // �^�C�~���O���v�Z
                CheckHitTiming(CalcHitTiming(notesManager.NoteDataAll[notesManager.NoteDataAll.Count - 1 - noteCount].time), notesManager.NoteDataAll.Count - 1 - noteCount);
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
        // �����O�m�[�c�ł���ꍇ�͂��ׂč폜���Ȃ�
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
            // ���̃l�X�g�łǂ���������̂������߂Ȃ���
            // �l�X�g�[������肪��������

            // �����O�m�[�c���ɏo�Ă���m�[�c����
            if (i != notedata.longNotes.Count - 1)
            {
                if (!pushingKeyState[notedata.longNotes[i].laneNum])
                {
                    continue;
                }
                // �p�[�t�F�N�g����͈̔͂ɓ����Ă��Ă܂����肵�Ă��Ȃ���Ԃł���Ώ���
                if (CalcHitTiming(notedata.longNotes[i].time) <= PerfectSecond && !notedata.longNotes[i].passnext)
                {
                    PopupJudgeLongMsg(0, notedata.longNotes[i].laneNum);
                    mainManager.AddCombo();
                    mainManager.AddJudgeCount(0);
                    notedata.longNotes[i].passnext = true;
                    soundMain.PlaySE((int)SoundMain.SE.Touch);
                }
                continue;
            }
            // �Ō�̃����O�m�[�c�̔���
            if (CalcHitTiming(notedata.longNotes[i].time) <= MissSecond && !notedata.isEnd)
            {

                if (pushingKeyState[notedata.longNotes[i].laneNum])
                {
                    continue;
                }



                if (CalcHitTiming(notedata.longNotes[i].time) <= PerfectSecond)
                {
                    PopupJudgeLongMsg(0, notedata.longNotes[i].laneNum);
                    mainManager.AddCombo();
                    mainManager.AddJudgeCount(0);
                }
                else if (CalcHitTiming(notedata.longNotes[i].time) <= GreatSecond)
                {
                    PopupJudgeLongMsg(1, notedata.longNotes[i].laneNum);
                    mainManager.AddCombo();
                    mainManager.AddJudgeCount(1);
                }
                else if (CalcHitTiming(notedata.longNotes[i].time) <= BadSecond)
                {

                    PopupJudgeLongMsg(2, notedata.longNotes[i].laneNum);
                    mainManager.ResetCombo();
                    mainManager.AddJudgeCount(2);
                }
                else if (CalcHitTiming(notedata.longNotes[i].time) <= MissSecond)
                {
                    PopupJudgeLongMsg(3, notedata.longNotes[i].laneNum);
                    mainManager.ResetCombo();
                    mainManager.AddJudgeCount(3);
                }
                soundMain.PlaySE((int)SoundMain.SE.Touch);
                notedata.isEnd = true;

                continue;
            }

            if (IsCheckOverNote(notedata.longNotes[i].time) && !notedata.isEnd)
            {
                PopupJudgeLongMsg(3, notedata.longNotes[i].laneNum);
                mainManager.ResetCombo();
                mainManager.AddJudgeCount(3);
                notedata.isEnd = true;
            }

        }

    }

    bool IsCheckOverNote(float time)
    {
        if (Time.time - (mainManager.startTime + time) >= MissSecond)
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
        // Instance�̍폜�����̓I�u�W�F�N�g�ɋL�q
        Instantiate(JudgeMsgObj[judge], new Vector3(notesManager.NoteDataAll[offset].laneNum - 1.5f, 0.76f, 0.15f), Quaternion.Euler(45, 0, 0));

        if (judge != 3)
        {
            Instantiate(hitEffect, new Vector3(notesManager.NoteDataAll[offset].laneNum - 1.5f, 0.6f, 0f), Quaternion.Euler(90, 0, 0));
        }
    }
    void PopupJudgeLongMsg(int judge, int laneNum)
    {
        // Instance�̍폜�����̓I�u�W�F�N�g�ɋL�q
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
