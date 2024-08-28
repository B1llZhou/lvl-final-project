using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EAttackMethod { 
    Guitar,
    Keyboard,
    Drum
}

public class CharacterAbilityControl : MonoBehaviour
{
    public Ability guitar;
    public Ability keyboard;
    public Ability drum;
    public Text seqText;
    public Text comboText;
    public int maxSeqCount = 4;
    public float clearSeqInterval = 5f;
    public float clearComboTextInterval = 2f;
    private List<EAttackMethod> curSeq = new List<EAttackMethod>();
    private float lastAddSeqTime;
    private float lastSetComboTextTime;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.J)) {
            if (guitar.Release(curSeq.Count)) {

                AddSeq(EAttackMethod.Guitar);
            };
        }

        if (Input.GetKeyDown(KeyCode.K))  {
            if (keyboard.Release(curSeq.Count)) {

                AddSeq(EAttackMethod.Keyboard);
            }
            
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            if (drum.Release(curSeq.Count)) {

                AddSeq(EAttackMethod.Drum);
            }
        }

        if (curSeq.Count > 0 && Time.time >= lastAddSeqTime + clearSeqInterval) {
            ClearAbilitySeq();
        }

        if (comboText.gameObject.activeInHierarchy && Time.time >= lastSetComboTextTime + clearComboTextInterval) {
            comboText.text = "";
            comboText.gameObject.SetActive(false);
        }
    }

    public void ClearAbilitySeq() {
        curSeq.Clear();
        Debug.Log("ClearSeq");
        LogCurSeq();
    }

    private void AddSeq(EAttackMethod method) {
        if (curSeq.Count >= maxSeqCount) {
            curSeq.RemoveAt(0);
        }

        curSeq.Add(method);
        lastAddSeqTime = Time.time;

        LogCurSeq();

        CheckAbilitySeq();
    }

    private void LogCurSeq() {
        string curSeqString = "";
        foreach (var v in curSeq)
        {
            curSeqString = curSeqString + System.Enum.GetName(v.GetType(), v) + " ";
        }
        seqText.text = curSeqString;
        Debug.Log(curSeqString);
    }

    private void CheckAbilitySeq() {
        if (curSeq.Count >= 4) {
            CheckComboOf4();
            CheckComboOf3();
            CheckComboOf2();
        } else if (curSeq.Count >= 3) {
            CheckComboOf3();
            CheckComboOf2();
        } else if (curSeq.Count >= 2) {
            CheckComboOf2();
        }
    }

    private void CheckComboOf2() {
        if (curSeq.Count < 2) {
            return;
        }

        if (curSeq[curSeq.Count - 1] == EAttackMethod.Guitar &&
            curSeq[curSeq.Count - 2] == EAttackMethod.Guitar)
        {
            SetComboText("Next guitar attack damage * 2");
            ClearAbilitySeq();
            return;
        }
    }

    private void CheckComboOf3() {
        if (curSeq.Count < 3) {
            return;
        }

        if (curSeq[curSeq.Count - 1] == EAttackMethod.Keyboard &&
            curSeq[curSeq.Count - 2] == EAttackMethod.Keyboard &&
            curSeq[curSeq.Count - 3] == EAttackMethod.Keyboard)
        {
            SetComboText("Heal up for 5 sec");
            ClearAbilitySeq();
            return;
        }

        if (curSeq[curSeq.Count - 1] == EAttackMethod.Drum &&
            curSeq[curSeq.Count - 2] == EAttackMethod.Keyboard &&
            curSeq[curSeq.Count - 3] == EAttackMethod.Guitar)
        {
            SetComboText("Speed up");
            ClearAbilitySeq();
            return;
        }
    }

    private void CheckComboOf4() {
        if (curSeq.Count < 4) {
            return;
        }

        if (curSeq[curSeq.Count - 1] == EAttackMethod.Drum &&
            curSeq[curSeq.Count - 2] == EAttackMethod.Keyboard &&
            curSeq[curSeq.Count - 3] == EAttackMethod.Keyboard &&
            curSeq[curSeq.Count - 4] == EAttackMethod.Guitar)
        {
            SetComboText("Stunning shockwave!");
            ClearAbilitySeq();
            return;
        }
    }

    private void SetComboText(string text) {
        comboText.text = "Combo: "+ text;
        comboText.gameObject.SetActive(true);
        lastSetComboTextTime = Time.time;
        Debug.Log(text);
    }
}
