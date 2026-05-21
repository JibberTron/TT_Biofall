using UnityEngine;
using TMPro;

public class InteractionPromptDetector : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDist = 5f;
    [SerializeField] private LayerMask interactMask;

    [Header("UI")]
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private string promptMessage = "Press [E] to interact";

    private void Satrt()
    {
        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        CheckForInteractable();
    }

    private void CheckForInteractable()
    {
        if(promptText == null || playerCamera == null)
        {
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position + playerCamera.transform.forward * 0.5f, playerCamera.transform.forward);

        bool isLookingAtInteractable = Physics.Raycast(ray, out RaycastHit hit, interactDist, interactMask);

        promptText.gameObject.SetActive(isLookingAtInteractable);

        if(isLookingAtInteractable)
        {
            promptText.text = promptMessage;
        }
    }
}