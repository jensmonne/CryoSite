using UnityEngine;
using UnityEngine.UI;

namespace VRIF_Mirror_Package.Scripts.UI {
    public class LocalPlayerHealthUI : MonoBehaviour
    {
        public Image UIImage;

        public float lerpSpeed = 5f;
        
        private void Update() {
            float currentHealth = 0;
            float maxHealth = 0;
            if (UIImage) {
                UIImage.fillAmount = Mathf.Lerp(UIImage.fillAmount, currentHealth / maxHealth, lerpSpeed * Time.deltaTime);
            }
        }
    }
}

