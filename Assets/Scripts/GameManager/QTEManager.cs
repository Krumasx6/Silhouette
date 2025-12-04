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
    [SerializeField] private Image qteBackground; // Optional background
    
    [Header("Colors")]
    [SerializeField] private Color pendingColor = Color.gray;
    [SerializeField] private Color currentColor = Color.yellow;
    [SerializeField] private Color completedColor = Color.green;
    [SerializeField] private Color failedColor = Color.red;
    
    // QTE State
    private bool qteActive = false;
    private List<KeyCode> qteKeys = new List<KeyCode>();
    private List<KeyCode> pressedKeys = new List<KeyCode>();
    private List<GameObject> keyButtons = new List<GameObject>();
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
                numKeys = Random.Range(3, 6); // 3-5 keys
                break;
            case QTEDifficulty.Medium:
                numKeys = Random.Range(5, 8); // 5-7 keys
                break;
            case QTEDifficulty.Hard:
                numKeys = Random.Range(7, 11); // 7-10 keys
                break;
        }
        
        StartQTE(numKeys);
    }
    
    // Button-friendly versions (use these for UI buttons)
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
        
        // Reset all key visuals
        for (int i = 0; i < keyButtons.Count; i++)
        {
            UpdateKeyButton(i, false);
        }
        
        UpdateProgress();
    }
    
    private void CreateKeyButtons()
    {
        for (int i = 0; i < qteKeys.Count; i++)
        {
            GameObject keyBtn = Instantiate(keyButtonPrefab, keyContainer);
            TextMeshProUGUI keyText = keyBtn.GetComponentInChildren<TextMeshProUGUI>();
            Image keyImage = keyBtn.GetComponent<Image>();
            
            if (keyText != null)
            {
                keyText.text = qteKeys[i].ToString();
            }
            
            if (keyImage != null)
            {
                keyImage.color = i == 0 ? currentColor : pendingColor;
            }
            
            keyButtons.Add(keyBtn);
        }
    }
    
    private void UpdateKeyButton(int index, bool success)
    {
        if (index < 0 || index >= keyButtons.Count) return;
        
        Image keyImage = keyButtons[index].GetComponent<Image>();
        if (keyImage != null)
        {
            keyImage.color = success ? completedColor : pendingColor;
            
            if (success)
            {
                // Scale animation for completed key
                StartCoroutine(ScaleAnimation(keyButtons[index].transform));
                
                // Highlight next key
                if (index + 1 < keyButtons.Count)
                {
                    Image nextImage = keyButtons[index + 1].GetComponent<Image>();
                    if (nextImage != null)
                    {
                        nextImage.color = currentColor;
                        StartCoroutine(BounceAnimation(keyButtons[index + 1].transform));
                    }
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
        yield return new WaitForSeconds(delay);
        qtePanel.SetActive(false);
        ClearKeyButtons();
        
        // If successful, reset guard awareness after cooldown
        if (success)
        {
            yield return new WaitForSeconds(cooldownAfterSuccess);
            // Guards become unaware again after successful stealth kill
        }
        
        OnQTEComplete?.Invoke(success);
    }
    
    private IEnumerator ScaleAnimation(Transform target)
    {
        Vector3 originalScale = target.localScale;
        Vector3 targetScale = originalScale * 1.2f;
        
        float elapsed = 0f;
        float duration = 0.2f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            target.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            target.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }
        
        target.localScale = originalScale;
    }
    
    private IEnumerator BounceAnimation(Transform target)
    {
        float elapsed = 0f;
        float duration = 0.5f;
        Vector3 originalPos = target.localPosition;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float bounce = Mathf.Sin(t * Mathf.PI * 2) * 10f;
            target.localPosition = originalPos + Vector3.up * bounce;
            yield return null;
        }
        
        target.localPosition = originalPos;
    }
}

public enum QTEDifficulty
{
    Easy,
    Medium,
    Hard
}

// Example usage in another script:
/*
public class GameController : MonoBehaviour
{
    [SerializeField] private QTEManager qteManager;
    private string guardAwareness = "unaware"; // Track guard state
    
    void Start()
    {
        qteManager.OnQTEComplete += HandleQTEResult;
    }
    
    public void StartKillAttempt()
    {
        // Determine difficulty based on current guard awareness
        QTEDifficulty difficulty = guardAwareness == "unaware" ? QTEDifficulty.Easy :
                                   guardAwareness == "suspicious" ? QTEDifficulty.Medium :
                                   QTEDifficulty.Hard;
        
        qteManager.StartQTEByDifficulty(difficulty);
    }
    
    private void HandleQTEResult(bool success)
    {
        if (success)
        {
            Debug.Log("Kill successful!");
            guardAwareness = "unaware"; // Reset to unaware after successful kill
            // Add kill count, etc.
        }
        else
        {
            Debug.Log("Kill failed!");
            guardAwareness = "alert"; // Set to alert on failure
            StartCoroutine(ResetAwarenessAfterTime(10f)); // Reset after 10 seconds
        }
    }
    
    private IEnumerator ResetAwarenessAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        guardAwareness = "unaware"; // Guards calm down after time
        Debug.Log("Guards are unaware again");
    }
}
*/