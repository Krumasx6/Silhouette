using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ReactionQTE : MonoBehaviour
{
    [Header("UI")]
    public GameObject qtePanel;            // QTE_Panel reference
    public Transform sequenceParent;      // SequenceParent
    public GameObject keyPrefab;          // KeySlot prefab

    [Header("Settings")]
    public float reactionTime = 0.6f;     // seconds per key
    public int minLength = 3;
    public int maxLength = 5;

    private List<KeyCode> sequence = new List<KeyCode>();
    private int index;
    private float timer;
    private bool active;
    private Action onSuccess;
    private Action onFail;

    void Awake()
    {
        qtePanel.SetActive(false);
    }

    void Update()
    {
        if (!active) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Fail();
            return;
        }

        if (Input.anyKeyDown)
        {
            // Accept only W/A/S/D keys
            if (Input.GetKeyDown(sequence[index]))
            {
                index++;
                if (index >= sequence.Count) { Success(); return; }
                timer = reactionTime;
                HighlightCurrentKey();
            }
            else
            {
                // Any other key pressed -> fail
                Fail();
            }
        }
    }

    public void StartQTE(int length, Action successCallback, Action failCallback)
    {
        onSuccess = successCallback;
        onFail = failCallback;

        GenerateSequence(length);
        BuildUI();
        index = 0;
        timer = reactionTime;
        active = true;
        qtePanel.SetActive(true);
        HighlightCurrentKey();
    }

    private void GenerateSequence(int length)
    {
        sequence.Clear();
        KeyCode[] options = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
        for (int i = 0; i < length; i++)
            sequence.Add(options[UnityEngine.Random.Range(0, options.Length)]);
    }

    private void BuildUI()
    {
        // clear previous
        for (int i = sequenceParent.childCount - 1; i >= 0; i--)
            Destroy(sequenceParent.GetChild(i).gameObject);

        foreach (KeyCode k in sequence)
        {
            GameObject go = Instantiate(keyPrefab, sequenceParent);
            var txt = go.GetComponentInChildren<Text>();
            if (txt != null) txt.text = k.ToString();
        }
    }

    private void HighlightCurrentKey()
    {
        for (int i = 0; i < sequenceParent.childCount; i++)
        {
            var img = sequenceParent.GetChild(i).GetComponent<Image>();
            img.color = (i == index) ? Color.yellow : Color.white;
        }
    }

    private void Success()
    {
        active = false;
        qtePanel.SetActive(false);
        onSuccess?.Invoke();
    }

    private void Fail()
    {
        active = false;
        qtePanel.SetActive(false);
        onFail?.Invoke();
    }
}
