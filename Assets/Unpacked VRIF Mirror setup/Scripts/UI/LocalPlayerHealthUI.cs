using UnityEngine;
using UnityEngine.UI;

namespace BNG {
    public class LocalPlayerHealthUI : MonoBehaviour
    {
        public Image UIImage;

        public float LerpSpeed = 5f;
        
        private void Update() {
            float CurrentHealth = 0;
            float MaxHealth = 0;
            if (UIImage) {
                UIImage.fillAmount = Mathf.Lerp(UIImage.fillAmount, CurrentHealth / MaxHealth, LerpSpeed * Time.deltaTime);
            }
        }
    }
}

