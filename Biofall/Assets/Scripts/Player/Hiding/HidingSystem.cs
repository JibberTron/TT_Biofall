using UnityEngine;
using TMPro;

public class HidingSystem : MonoBehaviour
{
    [Header("Player Control Scripts")]
    [SerializeField] private MonoBehaviour[] scriptsToDisableWhileHiding;
    [SerializeField] private KeyCode exitHideKey = KeyCode.F;
    [SerializeField] private GameObject hidingPromptUI;

    [Header("Player Visual")]
    [SerializeField] private GameObject playerVisual;

    private bool isHiding;
    private Camera originalCamera;
    private Camera activeHidingCamera;
    private Transform currentExitPoint;
    private bool waitingForKeyRelease;

    private void Update()
    {
        if (!isHiding)
        {
            return;
        }

        if (waitingForKeyRelease)
        {
            if (!Input.GetKey(exitHideKey))
            {
                waitingForKeyRelease = false;
            }

            return;
        }

        if (Input.GetKeyDown(exitHideKey))
        {
            ExitHidingSpot();
        }
    }

    public void EnterHidingSpot(Transform hidePoint, Transform exitPoint, Camera hidingCamera)
    {
        if (isHiding)
        {
            return;
        }

        if (hidePoint == null)
        {
            Debug.LogWarning("Hide point is missing.");
            return;
        }

        isHiding = true;
        waitingForKeyRelease = true;
        currentExitPoint = exitPoint;

        foreach (MonoBehaviour script in scriptsToDisableWhileHiding)
        {
            if (script != null && script != this)
            {
                script.enabled = false;
            }
        }

        CharacterController controller = GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.enabled = false;
        }

        transform.SetPositionAndRotation(hidePoint.position, hidePoint.rotation);

        if (controller != null)
        {
            controller.enabled = true;
        }

        originalCamera = Camera.main;

        if (originalCamera != null)
        {
            originalCamera.gameObject.SetActive(false);
        }

        activeHidingCamera = hidingCamera;

        if (activeHidingCamera != null)
        {
            activeHidingCamera.gameObject.SetActive(true);
        }

        if (playerVisual != null)
        {
            playerVisual.SetActive(false);
        }

        if (hidingPromptUI != null)
        {
            hidingPromptUI.SetActive(true);
        }

        Debug.Log("Player entered hiding.");
    }

    public void ExitHidingSpot()
    {
        if (!isHiding)
        {
            return;
        }

        isHiding = false;

        CharacterController controller = GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.enabled = false;
        }

        if (currentExitPoint != null)
        {
            transform.SetPositionAndRotation(currentExitPoint.position, currentExitPoint.rotation);
        }

        if (controller != null)
        {
            controller.enabled = true;
        }

        if (activeHidingCamera != null)
        {
            activeHidingCamera.gameObject.SetActive(false);
        }

        if (originalCamera != null)
        {
            originalCamera.gameObject.SetActive(true);
        }

        foreach (MonoBehaviour script in scriptsToDisableWhileHiding)
        {
            if (script != null && script != this)
            {
                script.enabled = true;
            }
        }

        if (playerVisual != null)
        {
            playerVisual.SetActive(true);
        }

        if (hidingPromptUI != null)
        {
            hidingPromptUI.SetActive(false);
        }

        currentExitPoint = null;
        activeHidingCamera = null;
        waitingForKeyRelease = false;

        Debug.Log("Player exited hiding.");
    }

    public void ForceExitHiding()
    {
        ExitHidingSpot();
    }

    public bool IsHiding()
    {
        return isHiding;
    }

    public float GetDetectionModifier()
    {
        return isHiding ? 0f : 1f;
    }
}