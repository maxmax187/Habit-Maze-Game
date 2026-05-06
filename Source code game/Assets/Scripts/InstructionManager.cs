using UnityEngine;
using TMPro;
using System;

public class InstructionManager : MonoBehaviour
{
    public static InstructionManager Instance { get; private set; }

    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private TMP_Text instructionText;

    private Action onDismissCallback;
    private bool isShowing = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        instructionPanel.SetActive(false);
    }

    private void Update()
    {
        if (isShowing && Input.GetKeyDown(KeyCode.Space))
        {
            DismissInstruction();
        }
    }
    public void ShowInstruction(string text, Action onDismiss = null)
    {
        isShowing = true;
        instructionText.text = text;
        instructionPanel.SetActive(true);
        Time.timeScale = 0f;
        GameManager.Instance.PauseTimer();
        onDismissCallback = onDismiss;
    }

    public void DismissInstruction()
    {
        isShowing = false;
        instructionPanel.SetActive(false);
        Time.timeScale = 1f;
        GameManager.Instance.ResumeTimer();
        onDismissCallback?.Invoke();
        onDismissCallback = null;
    }
}