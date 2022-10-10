using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Add the script to your Dropdown Menu Template Object via (Your Dropdown Button > Template)

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectAutoScroll : MonoBehaviour
{
    // Sets the speed to move the scrollbar
    public float scrollSpeed = 10f;

    // Set as Template Object via (Your Dropdown Button > Template)
    public ScrollRect m_templateScrollRect;

    // Set as Template Viewport Object via (Your Dropdown Button > Template > Viewport)
    public RectTransform m_templateViewportTransform;

    // Set as Template Content Object via (Your Dropdown Button > Template > Viewport > Content)
    public RectTransform m_ContentRectTransform;

    private RectTransform m_SelectedRectTransform;

    private void Update()
    {
        UpdateScrollToSelected(m_templateScrollRect, m_ContentRectTransform, m_templateViewportTransform);
    }

    private void UpdateScrollToSelected(ScrollRect scrollRect, RectTransform contentRectTransform,
        RectTransform viewportRectTransform)
    {
        // Get the current selected option from the eventsystem.
        var selected = EventSystem.current.currentSelectedGameObject;

        if (selected == null) return;
        if (selected.transform.parent != contentRectTransform.transform) return;

        m_SelectedRectTransform = selected.GetComponent<RectTransform>();

        // Math stuff
        var selectedDifference = viewportRectTransform.localPosition - m_SelectedRectTransform.localPosition;
        var contentHeightDifference = contentRectTransform.rect.height - viewportRectTransform.rect.height;

        var selectedPosition = contentRectTransform.rect.height - selectedDifference.y;
        var currentScrollRectPosition = scrollRect.normalizedPosition.y * contentHeightDifference;
        var above = currentScrollRectPosition - m_SelectedRectTransform.rect.height / 2 +
                    viewportRectTransform.rect.height;
        var below = currentScrollRectPosition + m_SelectedRectTransform.rect.height / 2;

        // Check if selected option is out of bounds.
        if (selectedPosition > above)
        {
            var step = selectedPosition - above;
            var newY = currentScrollRectPosition + step;
            var newNormalizedY = newY / contentHeightDifference;
            scrollRect.normalizedPosition = Vector2.Lerp(scrollRect.normalizedPosition, new Vector2(0, newNormalizedY),
                scrollSpeed * Time.deltaTime);
        }
        else if (selectedPosition < below)
        {
            var step = selectedPosition - below;
            var newY = currentScrollRectPosition + step;
            var newNormalizedY = newY / contentHeightDifference;
            scrollRect.normalizedPosition = Vector2.Lerp(scrollRect.normalizedPosition, new Vector2(0, newNormalizedY),
                scrollSpeed * Time.deltaTime);
        }
    }
}