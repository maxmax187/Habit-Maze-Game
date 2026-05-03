using UnityEngine;
using TMPro;
using System;

public class InstructionManager : MonoBehaviour
{
    public static InstructionManager Instance { get; private set; }

    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private TMP_Text instructionText;

    private Action onDismissCallback;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowInstruction(string text, Action onDismiss = null)
    {
        instructionText.text = text;
        instructionPanel.SetActive(true);
        Time.timeScale = 0f;
        GameManager.Instance.PauseTimer();
        onDismissCallback = onDismiss;
    }

    public void DismissInstruction()
    {
        instructionPanel.SetActive(false);
        Time.timeScale = 1f;
        GameManager.Instance.ResumeTimer();
        onDismissCallback?.Invoke();
        onDismissCallback = null;
    }
}