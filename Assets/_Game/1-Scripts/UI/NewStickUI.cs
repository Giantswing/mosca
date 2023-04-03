using UnityEngine;

namespace UI
{
    public class NewStickUI : MonoBehaviour
    {
        [SerializeField] private RectTransform startingPosRectImage;
        [SerializeField] private RectTransform currentPosRectImage;

        private void OnEnable()
        {
            //PlayerMovement.onTouchInput += UpdateStickUI;

            startingPosRectImage.gameObject.SetActive(false);
            currentPosRectImage.gameObject.SetActive(false);
        }

        private void UpdateStickUI(Vector2 touchStartPos, Vector2 touchCurrentPos)
        {
            if (touchStartPos != Vector2.zero)
            {
                startingPosRectImage.gameObject.SetActive(true);
                currentPosRectImage.gameObject.SetActive(true);
            }
            else
            {
                startingPosRectImage.gameObject.SetActive(false);
                currentPosRectImage.gameObject.SetActive(false);
            }

            startingPosRectImage.position = touchStartPos;
            currentPosRectImage.position = touchCurrentPos;
        }

        private void OnDisable()
        {
            //PlayerMovement.onTouchInput -= UpdateStickUI;
        }
    }
}