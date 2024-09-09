using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public enum EAttackMethod { 
    Guitar,
    Keyboard,
    Drum
}

public class CharacterAbilityControl : MonoBehaviour
{
    [Header("Previous")]
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
    public bool canCombo = true;
    private List<EAttackMethod> currentCombo = new List<EAttackMethod>();
    public MusicManager musicManager;
    private float beatLength;
    private float delayOffset = 0f;
    private Coroutine attackCoroutine;
    public float globalDelay;
    private float lastComboTime;
    private float comboFinishTime;
    public float comboInputWindow;
    public Text comboSeqText;

    [Header("Score")] 
    public GameObject[] timingFeedbacks;
    public int score = 0;
    private int[] scoreBonus = { 50, 20, 10 };
    [SerializeField] private Text scoreText;
    
    private void Start() {
        musicManager = FindObjectOfType<MusicManager>();
        beatLength = musicManager.GetBeatLength();
        delayOffset = musicManager.GetDelayOffset();

        foreach (var v in timingFeedbacks)
        {
            v.SetActive(false);
        }

        comboInputWindow = beatLength * 2;
    }

    private void Update()
    {
        globalDelay = musicManager.globalDelay;
        
        // if (Input.GetKeyDown(KeyCode.J)) {
        //     // if (guitar.Release(curSeq.Count)) {
        //     //
        //     //     AddSeq(EAttackMethod.Guitar);
        //     // };
        //     
        //     
        //     // StartCoroutine(WaitUntilBeat(EAttackMethod.Guitar));
        //     SnapToBeat(EAttackMethod.Guitar);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.K))  {
        //     // if (keyboard.Release(curSeq.Count)) {
        //     //
        //     //     AddSeq(EAttackMethod.Keyboard);
        //     // }
        //     
        //     // StartCoroutine(WaitUntilBeat(EAttackMethod.Drum));
        //     SnapToBeat(EAttackMethod.Drum);
        //
        // }

        // if (Input.GetKeyDown(KeyCode.L)) {
        //     if (drum.Release(curSeq.Count)) {
        //
        //         AddSeq(EAttackMethod.Drum);
        //     }
        // }

        // if (curSeq.Count > 0 && Time.time >= lastAddSeqTime + clearSeqInterval) {
        //     ClearAbilitySeq();
        // }

        if (currentCombo.Count > 0 && Time.time >= comboFinishTime + comboInputWindow)
        {
            ClearCurrentCombo();
        }
        
        ShowScore();

        // if (comboText.gameObject.activeInHierarchy && Time.time >= lastSetComboTextTime + clearComboTextInterval) {
        //     comboText.text = "";
        //     comboText.gameObject.SetActive(false);
        // }
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
    /// Below are new functions
    /// </summary>

    public void ClearCurrentCombo() {
        currentCombo.Clear();
        comboSeqText.text = "Combo: ";
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
        
        string text = "";
        comboSeqText.text = "Combo: ";
        for (int i = 0; i < currentCombo.Count; i++)
        {
            text += currentCombo[i].ToString();
            if (i != currentCombo.Count - 1) text += " > ";
        }
        // Debug.Log(comboSeqText);
        comboSeqText.text = "Combo: " + text;
    }

    // private void GuitarAttack() {
    //     if (!canCombo) return;
    //     
    //     AddCombo(EAttackMethod.Guitar);
    //     var comboSequence = currentCombo.Count(n => n == EAttackMethod.Guitar);
    //     guitar.Release(comboSequence - 1);
    //
    //     StartCoroutine(DisableCombo(comboSequence - 1));
    //
    //     comboFinishTime = Time.time + comboSequence switch
    //     {
    //         0 => beatLength * 2,
    //         1 => beatLength * 2,
    //         2 => beatLength * 4,
    //         _ => beatLength * 2
    //     };
    //     // guitar.Release();
    //     // Debug.Log("Guitar attack no." + comboSequence);
    // }
    //
    // private void DrumAttack() {
    //     if (!canCombo) return;
    //     
    //     AddCombo(EAttackMethod.Drum);
    //     var comboSequence = currentCombo.Count(n => n == EAttackMethod.Drum);
    //     drum.Release(comboSequence - 1);
    //     // Debug.Log("Drum attack no." + comboSequence);
    // }

    private void DoAttack(EAttackMethod method, float skipTime = 0f)
    {
        if (!canCombo) return;
        
        var dif = musicManager.DifferenceFromBeat();
        StartCoroutine(TimingFeedback(dif));
        AddCombo(method);
        
        var comboSequence = currentCombo.Count(n => n == method);
        switch (method)
        {
            case EAttackMethod.Guitar:
                guitar.Release(comboSequence - 1, skipTime);
                break;
            case EAttackMethod.Drum:
                drum.Release(comboSequence - 1, skipTime);
                break;
            case EAttackMethod.Keyboard:
                // TODO 
                break;
            default: 
                guitar.Release(comboSequence - 1);
                break;
        }
        
        StartCoroutine(DisableCombo(comboSequence - 1));

        comboFinishTime = Time.time + comboSequence switch
        {
            0 => beatLength * 2,
            1 => beatLength * 2,
            2 => beatLength * 4,
            _ => beatLength * 2
        };
    }

    private void SnapToBeat(EAttackMethod method)
    {
        var dif = musicManager.DifferenceFromBeat();
        // StartCoroutine(TimingFeedback(dif));

        if (dif < 0)
        {
            StartCoroutine(WaitUntilBeat(method));
        }
        else
        {
            var beatCount = musicManager.BeatCount();
            var skipTime = (beatCount - Mathf.Floor(beatCount)) * beatLength;
            DoAttack(method, skipTime);
            // Debug.Log("SkipTime: " + skipTime + "; dif: " + dif);
        }
    }
    
    private IEnumerator WaitUntilBeat(EAttackMethod method) {
        // var dif = musicManager.DifferenceFromBeat();
        // StartCoroutine(TimingFeedback(dif));

        var beatCount = musicManager.BeatCount();
        
            var waitTime = Mathf.Ceil(beatCount) * beatLength - (Time.time - globalDelay);
            // Debug.Log("WaitTime: " + waitTime);
            yield return new WaitForSeconds(waitTime);
            // Debug.Log("WaitTime: " + waitTime);
            // Debug.Log("BeatCount: " + musicManager.BeatCount());
        DoAttack(method);
    }
    

    private IEnumerator TimingFeedback(double dif) {
        if (Math.Abs(dif) <= musicManager.onBeatAccuracy) {
            timingFeedbacks[0].SetActive(true);
            score += scoreBonus[0];
            yield return new WaitForSeconds(0.3f);
            timingFeedbacks[0].SetActive(false);
        }
        else if (Math.Abs(dif) <= musicManager.onBeatAccuracy * 2) {
            timingFeedbacks[1].SetActive(true);
            score += scoreBonus[1];
            yield return new WaitForSeconds(0.3f);
            timingFeedbacks[1].SetActive(false);
        }
        else {
            timingFeedbacks[2].SetActive(true);
            score += scoreBonus[2];
            yield return new WaitForSeconds(0.3f);
            timingFeedbacks[2].SetActive(false);
        }
    }
    
    private void ShowScore()
    {
        if (scoreText == null) return;
        scoreText.text = "Score: " + score.ToString();
    }

    private IEnumerator DisableCombo(int comboSeq)
    {
        var waitTime = comboSeq switch
        {
            0 => beatLength * 1.5f,
            1 => beatLength * 1.5f,
            2 => beatLength * 3.5f,
            _ => beatLength * 1.5f
        };
        canCombo = false;
        yield return new WaitForSeconds(waitTime);
        canCombo = true;
    }

    public void OnAttack1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SnapToBeat(EAttackMethod.Guitar);
        }
    }
    
    public void OnAttack2(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SnapToBeat(EAttackMethod.Drum);
        }
    }
    
    public void OnAttack3(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SnapToBeat(EAttackMethod.Keyboard);
        }
    }
}
