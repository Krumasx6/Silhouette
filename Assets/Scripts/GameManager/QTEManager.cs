using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class QTEManager : MonoBehaviour
{
    [Header("QTE Settings")]
    [SerializeField] private float qteTimeLimit = 8f;
    [SerializeField] private KeyCode[] availableKeys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
    [SerializeField] private float cooldownAfterSuccess = 3f;
    
    [Header("UI References")]
    [SerializeField] private GameObject qtePanel;
    [SerializeField] private Transform keyContainer;
    [SerializeField] private GameObject keyButtonPrefab;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image timerBar;
    [SerializeField] private Image qteBackground;
    
    [Header("Colors")]
    [SerializeField] private Color pendingColor = Color.gray;
    [SerializeField] private Color currentColor = Color.white;
    [SerializeField] private Color completedColor = Color.green;
    [SerializeField] private Color failedColor = Color.red;
    
    [Header("Position Offsets")]
    [SerializeField] private float currentKeyYOffset = 70f;
    [SerializeField] private float nextKeyYOffset = 10f;
    
    // QTE State
    private bool qteActive = false;
    private List<KeyCode> qteKeys = new List<KeyCode>();
    private List<KeyCode> pressedKeys = new List<KeyCode>();
    private List<GameObject> keyButtons = new List<GameObject>();
    private List<float> originalYPositions = new List<float>();
    private float qteTimer = 0f;
    
    // Events
    public System.Action<bool> OnQTEComplete;
    
    void Update()
    {
        if (!qteActive) return;
        
        // Update timer
        qteTimer -= Time.deltaTime;
        if (timerBar != null)
        {
            timerBar.fillAmount = qteTimer / qteTimeLimit;
        }
        
        // Check for timeout
        if (qteTimer <= 0f)
        {
            FailQTE("TIME'S UP!");
            return;
        }
        
        // Check for key input
        foreach (KeyCode key in availableKeys)
        {
            if (Input.GetKeyDown(key))
            {
                HandleKeyPress(key);
                break;
            }
        }
    }
    
    public void StartQTE(int numKeys)
    {
        // Reset state
        qteActive = true;
        qteTimer = qteTimeLimit;
        qteKeys.Clear();
        pressedKeys.Clear();
        ClearKeyButtons();

        // Generate random key sequence
        for (int i = 0; i < numKeys; i++)
        {
            qteKeys.Add(availableKeys[Random.Range(0, availableKeys.Length)]);
        }

        // Setup UI
        qtePanel.SetActive(true);

        // Create key UI elements
        CreateKeyButtons();
        UpdateProgress();
    }
    
    public void StartQTEByDifficulty(QTEDifficulty difficulty)
    {
        int numKeys = 0;
        
        switch (difficulty)
        {
            case QTEDifficulty.Easy:
                numKeys = Random.Range(3, 6);
                break;
            case QTEDifficulty.Medium:
                numKeys = Random.Range(5, 8);
                break;
            case QTEDifficulty.Hard:
                numKeys = Random.Range(7, 11);
                break;
        }
        
        StartQTE(numKeys);
    }
    
    public void StartEasyQTE()
    {
        StartQTEByDifficulty(QTEDifficulty.Easy);
    }
    
    public void StartMediumQTE()
    {
        StartQTEByDifficulty(QTEDifficulty.Medium);
    }
    
    public void StartHardQTE()
    {
        StartQTEByDifficulty(QTEDifficulty.Hard);
    }
    
    private void HandleKeyPress(KeyCode key)
    {
        if (pressedKeys.Count >= qteKeys.Count) return;
        
        KeyCode expectedKey = qteKeys[pressedKeys.Count];
        
        if (key == expectedKey)
        {
            // Correct key
            pressedKeys.Add(key);
            UpdateKeyButton(pressedKeys.Count - 1, true);
            UpdateProgress();
            
            // Check if sequence complete
            if (pressedKeys.Count >= qteKeys.Count)
            {
                CompleteQTE();
            }
        }
        else
        {
            // Wrong key - reset progress
            ResetProgress();
        }
    }
    
    private void CompleteQTE()
    {
        qteActive = false;
        StartCoroutine(CloseQTEAfterDelay(0.5f, true));
    }
    
    private void FailQTE(string reason)
    {
        qteActive = false;
        StartCoroutine(CloseQTEAfterDelay(1f, false));
    }
    
    private void ResetProgress()
    {
        pressedKeys.Clear();
        
        // Reset all key visuals and positions
        for (int i = 0; i < keyButtons.Count; i++)
        {
            // Find KeycapBG child specifically for color
            Transform keycapBG = keyButtons[i].transform.Find("KeycapBG");
            Image keyImage = keycapBG != null ? keycapBG.GetComponent<Image>() : null;
            
            RectTransform rectTransform = keyButtons[i].GetComponent<RectTransform>();
            Vector2 pos = rectTransform.anchoredPosition;
            
            if (i == 0)
            {
                // First key is current - white and up
                if (keyImage != null) keyImage.color = currentColor;
                pos.y = originalYPositions[i] + currentKeyYOffset;
            }
            else if (i == 1)
            {
                // Second key is next - gray and slightly up
                if (keyImage != null) keyImage.color = pendingColor;
                pos.y = originalYPositions[i] + nextKeyYOffset;
            }
            else
            {
                // Rest are pending - gray at original position
                if (keyImage != null) keyImage.color = pendingColor;
                pos.y = originalYPositions[i];
            }
            
            rectTransform.anchoredPosition = pos;
        }
        
        UpdateProgress();
    }
    
    private void CreateKeyButtons()
    {
        originalYPositions.Clear();

        // Instantiate all buttons and set text and colors
        for (int i = 0; i < qteKeys.Count; i++)
        {
            GameObject keyBtn = Instantiate(keyButtonPrefab, keyContainer);

            // Find KeycapBG child specifically for color
            Transform keycapBG = keyBtn.transform.Find("KeycapBG");
            Image keyImage = keycapBG != null ? keycapBG.GetComponent<Image>() : null;

            TextMeshProUGUI keyText = keyBtn.GetComponentInChildren<TextMeshProUGUI>();

            // Set the key text
            if (keyText != null)
            {
                keyText.text = qteKeys[i].ToString();
            }

            // Set color
            if (i == 0)
            {
                // Current key - WHITE
                if (keyImage != null)
                {
                    keyImage.color = currentColor;
                }
            }
            else if (i == 1)
            {
                // Next key - GRAY
                if (keyImage != null)
                {
                    keyImage.color = pendingColor;
                }
            }
            else
            {
                // Pending keys - GRAY
                if (keyImage != null)
                {
                    keyImage.color = pendingColor;
                }
            }

            keyButtons.Add(keyBtn);
        }

        // Force layout rebuild to apply HorizontalLayoutGroup positioning
        LayoutRebuilder.ForceRebuildLayoutImmediate(keyContainer.GetComponent<RectTransform>());

        // Now set Y positions after layout has positioned X
        for (int i = 0; i < keyButtons.Count; i++)
        {
            RectTransform rectTransform = keyButtons[i].GetComponent<RectTransform>();
            Vector2 pos = rectTransform.anchoredPosition;

            // Store original Y position AFTER layout
            float originalY = pos.y;
            originalYPositions.Add(originalY);

            if (i == 0)
            {
                pos.y = currentKeyYOffset;
            }
            else if (i == 1)
            {
                pos.y = nextKeyYOffset;
            }
            else
            {
                pos.y = 0;
            }

            rectTransform.anchoredPosition = pos;
        }
    }
    private void UpdateKeyButton(int index, bool success)
    {
        if (index < 0 || index >= keyButtons.Count) return;
        
        // Find KeycapBG child specifically for color
        Transform keycapBG = keyButtons[index].transform.Find("KeycapBG");
        Image keyImage = keycapBG != null ? keycapBG.GetComponent<Image>() : null;
        
        RectTransform rectTransform = keyButtons[index].GetComponent<RectTransform>();
        Vector2 pos = rectTransform.anchoredPosition;
        
        if (keyImage != null && success)
        {
            // Completed key - GREEN and back to original Y (NO OFFSET)
            keyImage.color = completedColor;
            pos.y = 0;
            rectTransform.anchoredPosition = pos;
            
            // Update next key
            if (index + 1 < keyButtons.Count)
            {
                Transform nextKeycapBG = keyButtons[index + 1].transform.Find("KeycapBG");
                Image nextImage = nextKeycapBG != null ? nextKeycapBG.GetComponent<Image>() : null;
                
                RectTransform nextRect = keyButtons[index + 1].GetComponent<RectTransform>();
                Vector2 nextPos = nextRect.anchoredPosition;
                
                if (nextImage != null)
                {
                    // Next key becomes current - WHITE and move up (70 offset)
                    nextImage.color = currentColor;
                    nextPos.y = currentKeyYOffset;
                    nextRect.anchoredPosition = nextPos;
                }
                
                // Update the key after next (if exists) - becomes next with 10 offset
                if (index + 2 < keyButtons.Count)
                {
                    RectTransform afterNextRect = keyButtons[index + 2].GetComponent<RectTransform>();
                    Vector2 afterNextPos = afterNextRect.anchoredPosition;
                    afterNextPos.y = nextKeyYOffset;
                    afterNextRect.anchoredPosition = afterNextPos;
                }
                
                // Reset all other pending keys to NO OFFSET
                for (int i = index + 3; i < keyButtons.Count; i++)
                {
                    RectTransform pendingRect = keyButtons[i].GetComponent<RectTransform>();
                    Vector2 pendingPos = pendingRect.anchoredPosition;
                    pendingPos.y = 0; // NO OFFSET
                    pendingRect.anchoredPosition = pendingPos;
                }
            }
        }
    }
    
    private void ClearKeyButtons()
    {
        foreach (GameObject btn in keyButtons)
        {
            Destroy(btn);
        }
        keyButtons.Clear();
        originalYPositions.Clear();
    }
    
    private void UpdateProgress()
    {
        if (progressText != null)
        {
            progressText.text = $"{pressedKeys.Count}/{qteKeys.Count}";
        }
    }
    
    private IEnumerator CloseQTEAfterDelay(float delay, bool success)
    {
        // Invoke the event IMMEDIATELY so PlayerKillingMechanics can kill the enemy right away
        OnQTEComplete?.Invoke(success);
        
        // Then do the UI animation/cooldown
        yield return new WaitForSeconds(delay);
        qtePanel.SetActive(false);
        ClearKeyButtons();
        
        if (success)
        {
            yield return new WaitForSeconds(cooldownAfterSuccess);
        }
    }
}

public enum QTEDifficulty
{
    Easy,
    Medium,
    Hard
}