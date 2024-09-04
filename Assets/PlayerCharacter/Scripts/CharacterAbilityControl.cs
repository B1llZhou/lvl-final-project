using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using Unity.VisualScripting;

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

    [Header("New Combo System")]
    private List<EAttackMethod> currentCombo = new List<EAttackMethod>();
    private bool canCombo = true;
    public MusicManager musicManager;
    private float beatLength;
    private float delayOffset = 0f;
    private Coroutine attackCoroutine;
    public GameObject timingFeedback01;
    public GameObject timingFeedback02;
    public GameObject timingFeedback03;
    
    private void Start() {
        musicManager = FindObjectOfType<MusicManager>();
        beatLength = musicManager.GetBeatLength();
        delayOffset = musicManager.GetDelayOffset();

        timingFeedback01.SetActive(false);
        timingFeedback02.SetActive(false);
        timingFeedback03.SetActive(false);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.J)) {
            // if (guitar.Release(curSeq.Count)) {
            //
            //     AddSeq(EAttackMethod.Guitar);
            // };
            
            
            StartCoroutine(WaitUntilBeat());
        }

        if (Input.GetKeyDown(KeyCode.K))  {
            // if (keyboard.Release(curSeq.Count)) {
            //
            //     AddSeq(EAttackMethod.Keyboard);
            // }
            DrumAttack();
        }

        // if (Input.GetKeyDown(KeyCode.L)) {
        //     if (drum.Release(curSeq.Count)) {
        //
        //         AddSeq(EAttackMethod.Drum);
        //     }
        // }

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

        // LogCurSeq();

        // CheckAbilitySeq();
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
    
    /// <summary>
    /// Below are new stuffs
    /// </summary>

    public void ClearCurrentCombo() {
        currentCombo.Clear();
        // Debug.Log("Cleared combo");
    }

    private void AddCombo(EAttackMethod method) {
        var guitarCount = currentCombo.Count(n => n == EAttackMethod.Guitar);
        var drumCount = currentCombo.Count(n => n == EAttackMethod.Drum);

        // Define what sequence of combos are allowed
        switch (method) {
            case EAttackMethod.Drum: {
                if (drumCount >= 3) ClearCurrentCombo();
                break;
            }
            case EAttackMethod.Guitar: {
                if (drumCount >= 3 || guitarCount >= 3) ClearCurrentCombo();
                break;
            }
        }

        currentCombo.Add(method);
    }

    private void GuitarAttack() {
        if (!canCombo) return;
        
        AddCombo(EAttackMethod.Guitar);
        var comboSequence = currentCombo.Count(n => n == EAttackMethod.Guitar);
        // guitar.Release(comboSequence - 1);
        guitar.Release();
        // Debug.Log("Guitar attack no." + comboSequence);
    }

    private void DrumAttack() {
        if (!canCombo) return;
        
        AddCombo(EAttackMethod.Drum);
        var comboSequence = currentCombo.Count(n => n == EAttackMethod.Drum);
        drum.Release(comboSequence - 1);
        // Debug.Log("Drum attack no." + comboSequence);
    }

    private IEnumerator WaitUntilBeat() {
        var curTime = Time.time;
        var isOnBeat = musicManager.IsOnBeat();
        var beatCount = curTime / beatLength;
        var delay = beatCount - Math.Round(beatCount);

        StartCoroutine(ShowFeedbackText(delay));
        
        if (!isOnBeat) {
            var waitTime = Mathf.Ceil(beatCount) * beatLength - curTime;
            // Debug.Log("Wait time: " + waitTime);
            yield return new WaitForSeconds(waitTime);
            Debug.Log(musicManager.IsOnBeat());
        }
        GuitarAttack();
    }

    private IEnumerator ShowFeedbackText(double delay) {
        if (Math.Abs(delay) <= 0.1f) {
            timingFeedback01.SetActive(true);
            // timingFeedback02.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            timingFeedback01.SetActive(false);
        }
        else if (Math.Abs(delay) <= 0.2f) {
            timingFeedback02.SetActive(true);
            // timingFeedback01.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            timingFeedback02.SetActive(false);
        }
        else {
            timingFeedback03.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            timingFeedback03.SetActive(false);
        }
    }
}
