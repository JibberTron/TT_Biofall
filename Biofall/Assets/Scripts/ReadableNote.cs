using UnityEngine;
using TMPro;

public class ReadableNote : MonoBehaviour, IInteractable
{
    [Header("UI")]
    [SerializeField] private GameObject notePanel;
    [SerializeField] private TMP_Text noteTextUI;

    [Header("Note Text")]
    [TextArea(5, 15)]
    [SerializeField] private string noteText;

    [Header("Input")]
    [SerializeField] private KeyCode closeKey = KeyCode.E;

    private bool isOpen;
    private bool waitingForKeyRelease;
    public static bool IsNoteOpen { get; private set; }

    private void Start()
    {
        if (notePanel != null)
        {
            notePanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isOpen)
        {
            return;
        }

        if (waitingForKeyRelease)
        {
            if (!Input.GetKey(closeKey))
            {
                waitingForKeyRelease = false;
            }

            return;
        }

        if (Input.GetKeyDown(closeKey))
        {
            CloseNote();
        }
    }

    public void Interact()
    {
        if (isOpen)
        {
            CloseNote();
        }
        else
        {
            OpenNote();
        }
    }

    private void OpenNote()
    {
        isOpen = true;
        waitingForKeyRelease = true;

        if (notePanel != null)
        {
            notePanel.SetActive(true);
        }

        if (noteTextUI != null)
        {
            noteTextUI.text = noteText;
        }

        IsNoteOpen = true;
        Time.timeScale = 0f;
    }

    private void CloseNote()
    {
        isOpen = false;

        if (notePanel != null)
        {
            notePanel.SetActive(false);
        }

        IsNoteOpen = false;
        Time.timeScale = 1f;
    }
}