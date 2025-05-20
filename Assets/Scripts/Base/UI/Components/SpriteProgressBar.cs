using UnityEngine;

namespace Base.UI
{
    [ExecuteInEditMode]
    public class SpriteProgressBar : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer backgroundSprite;
        [SerializeField] private SpriteRenderer fillSprite;

        [Range(0f, 1f)]
        [SerializeField] private float fillAmount = 1f;
        
        // Manually specify the total width and height of your progress bar
        [SerializeField] private float totalWidth = 1f;
        [SerializeField] private float totalHeight = 0.2f;
    
        // Origin point of the progress bar (0 = left, 0.5 = center, 1 = right)
        [SerializeField] private float originPoint = 0f;

        // If you want to customize colors
        [SerializeField] private Color maxValueColor = Color.green;
        [SerializeField] private Color minValueColor = Color.red;
        [SerializeField] private bool useColorGradient = false;

        private void Start()
        {
            UpdateProgressBar();
        }

        public void SetFillAmount(float amount)
        {
            fillAmount = Mathf.Clamp01(amount);
            UpdateProgressBar();
        }

        private void UpdateProgressBar()
        {
            if (fillSprite == null) return;
        
            // For standard sprites
            if (fillSprite.drawMode == SpriteDrawMode.Simple)
            {
                // Scale the sprite accordingly
                Vector3 scale = fillSprite.transform.localScale;
                scale.x = totalWidth * fillAmount;
                scale.y = totalHeight;
                fillSprite.transform.localScale = scale;
            
                // Position adjustment based on origin point
                Vector3 position = fillSprite.transform.localPosition;
                position.x = totalWidth * (fillAmount - 1) * (1 - originPoint);
                fillSprite.transform.localPosition = position;
            }
            // For 9-sliced sprites
            else if (fillSprite.drawMode == SpriteDrawMode.Sliced || fillSprite.drawMode == SpriteDrawMode.Tiled)
            {
                // Use size property for 9-sliced sprites
                fillSprite.size = new Vector2(totalWidth * fillAmount, totalHeight);
            
                // Position adjustment based on origin point
                Vector3 position = fillSprite.transform.localPosition;
                position.x = totalWidth * (fillAmount - 1) * (1 - originPoint) / 2;
                fillSprite.transform.localPosition = position;
            }
        
            // Update color if using gradient
            if (useColorGradient)
            {
                fillSprite.color = Color.Lerp(minValueColor, maxValueColor, fillAmount);
            }
        }

        private void Update()
        {
            if (Application.isEditor)
            {
                UpdateProgressBar();
            }
        }
    }
}