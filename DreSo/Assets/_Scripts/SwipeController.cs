using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SwipeController : MonoBehaviour
{
    public GameObject scrollbar;
    public InputActionReference clickActionReference; // Reference to the Click action in the Default Input Actions
    public Button nextButton;
    public Button previousButton;

    private Scrollbar scrollbarComponent; // Cached Scrollbar component
    private float scroll_pos = 0;
    private float[] pos;
    private float distance;
    private int currentIndex = 0;

    private const float LerpSpeed = 0.1f; // Speed of the lerp animation
    private const float SelectedScale = 1f; // Scale of the selected element
    private const float UnselectedScale = 0.8f; // Scale of the unselected elements

    void Start()
    {
        // Cache the Scrollbar component
        scrollbarComponent = scrollbar.GetComponent<Scrollbar>();

        // Initialize positions array and distance between elements
        InitializePositions();

        // Enable the click action
        clickActionReference.action.Enable();

        // Add listeners to buttons
        nextButton.onClick.AddListener(Next);
        previousButton.onClick.AddListener(Previous);
    }

    void OnEnable()
    {
        // Register the click event
        clickActionReference.action.performed += OnClickPerformed;
    }

    void OnDisable()
    {
        // Unregister the click event
        clickActionReference.action.performed -= OnClickPerformed;

        // Remove listeners from buttons
        nextButton.onClick.RemoveListener(Next);
        previousButton.onClick.RemoveListener(Previous);
    }

    void Update()
    {
        if (Pointer.current.press.isPressed)
        {
            scroll_pos = scrollbarComponent.value;
        }
        else
        {
            for (int i = 0; i < pos.Length; i++)
            {
                if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
                {
                    scrollbarComponent.value = Mathf.Lerp(scrollbarComponent.value, pos[i], LerpSpeed);
                }
            }
        }
        AdjustScales();
    }

    /// <summary>
    /// Initializes the positions array and distance between elements.
    /// </summary>
    private void InitializePositions()
    {
        pos = new float[transform.childCount];
        distance = 1f / (pos.Length - 1f);
        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }
    }

    /// <summary>
    /// Handles the click input to update the scroll position.
    /// </summary>
    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        if (context.control.device is Gamepad)
        {
            var delta = context.ReadValue<Vector2>();
            scroll_pos = Mathf.Clamp(scroll_pos + delta.x * 0.01f, 0f, 1f); // Adjust sensitivity as needed
            scrollbarComponent.value = scroll_pos;
        }
    }

    /// <summary>
    /// Scrolls to the next element.
    /// </summary>
    public void Next()
    {
        if (currentIndex < pos.Length - 1)
        {
            currentIndex++;
            scroll_pos = pos[currentIndex];
            scrollbarComponent.value = scroll_pos;
        }
    }

    /// <summary>
    /// Scrolls to the previous element.
    /// </summary>
    public void Previous()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            scroll_pos = pos[currentIndex];
            scrollbarComponent.value = scroll_pos;
        }
    }

    /// <summary>
    /// Adjusts the scales of the child elements to create a zoom effect.
    /// </summary>
    private void AdjustScales()
    {
        for (int i = 0; i < pos.Length; i++)
        {
            float targetScale = (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2)) ? SelectedScale : UnselectedScale;
            transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(targetScale, targetScale), LerpSpeed);
        }
    }
}
