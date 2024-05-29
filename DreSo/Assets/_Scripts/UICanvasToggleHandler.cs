using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// Handles the activation and deactivation of a UI canvas based on user input.
/// It uses the new Input System and supports mouse, touchscreen, and controller inputs.
/// </summary>
public class UICanvasToggleHandler : MonoBehaviour
{
    #region Fields

    [Tooltip("The Canvas object to be activated/deactivated")]
    public GameObject canvas; // The Canvas object to be activated/deactivated
    
    [Tooltip("Reference to the click action from the Input Action Asset")]
    public InputActionReference clickActionReference; // Reference to the click action from the Input Action Asset

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        // Subscribe to the click action
        clickActionReference.action.performed += OnClickPerformed;
        clickActionReference.action.Enable();
    }

    private void OnDisable()
    {
        // Unsubscribe from the click action
        clickActionReference.action.performed -= OnClickPerformed;
        clickActionReference.action.Disable();
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles the click performed event. Activates or deactivates the canvas based on click location.
    /// </summary>
    /// <param name="context">The context of the input action.</param>
    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        Vector2 pointerPosition = Pointer.current.position.ReadValue();

        // Check if the pointer is over a UI element
        if (IsPointerOverUI(pointerPosition))
        {
            return;
        }

        // Raycast to check if an interactable object was clicked
        Ray ray = Camera.main.ScreenPointToRay(pointerPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                // Activate the canvas
                canvas.SetActive(true);
            }
            else if (canvas.activeSelf)
            {
                // Deactivate the canvas if active and clicked outside the object
                canvas.SetActive(false);
            }
        }
        else if (canvas.activeSelf)
        {
            // Deactivate the canvas if active and clicked outside any interactable area
            canvas.SetActive(false);
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Checks if the pointer is currently over a UI element.
    /// </summary>
    /// <param name="pointerPosition">The position of the pointer.</param>
    /// <returns>True if the pointer is over a UI element, otherwise false.</returns>
    private bool IsPointerOverUI(Vector2 pointerPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = pointerPosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    #endregion
}
