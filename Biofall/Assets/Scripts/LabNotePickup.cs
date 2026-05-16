using UnityEngine;
using UnityEngine.UI;

public class LabNotePickup : MonoBehaviour, IInteractable
{
    [SerializeField] string noteTitle;

    [TextArea]
    [SerializeField] string noteText;

    [SerializeField] bool destroyAfterPickup = true;

    [SerializeField] GameObject notePanel;
    [SerializeField] Image noteImage;
    [SerializeField] Sprite noteSprite;

    private bool playerNearby;
  void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
        
        if(notePanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            notePanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }
    public void Interact()
    {
        Debug.Log("Lab Note Found: " + noteTitle);

        Debug.Log(noteText);

        noteImage.sprite = noteSprite;

        notePanel.SetActive(true);

        Time.timeScale = 0f;

        if (destroyAfterPickup)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerNearby = true;
            Debug.Log("Press E to read lab note");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}
