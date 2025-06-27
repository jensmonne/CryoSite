using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace VRIF_Mirror_Package.Scripts.UI.Utils
{
    public class ReworkedScreenFader : MonoBehaviour
    {
        [Header("Fader Settings")]
        [SerializeField] private bool fadeOutOnSceneLoaded = true;
        [SerializeField] private float sceneFadeInDelay = 0.2f;
        [SerializeField] private Color fadeColor = Color.black;
        [SerializeField] private float fadeOutSpeed = 6f;
        [SerializeField] private float fadeInSpeed = 6f;
        
        private GameObject _fadeObject;
        private RectTransform _fadeObjectRect;
        private Canvas _fadeCanvas;
        private CanvasGroup _canvasGroup;
        private Image _fadeImage;
        private IEnumerator _fadeRoutine;
        
        private const string FaderName = "ReworkedScreenFader";

        private void Awake()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            if (_fadeObject != null) return;
            
            _fadeObject = new GameObject(FaderName);
            _fadeObject.transform.SetParent(Camera.main.transform);
            _fadeObject.transform.localPosition = new Vector3(0, 0, 0.03f);
            _fadeObject.transform.localRotation = Quaternion.identity;

            _fadeCanvas = _fadeObject.AddComponent<Canvas>();
            _fadeCanvas.renderMode = RenderMode.WorldSpace;
            _fadeCanvas.sortingOrder = 999;

            _canvasGroup = _fadeObject.AddComponent<CanvasGroup>();
            _canvasGroup.interactable = false;

            _fadeImage = _fadeObject.AddComponent<Image>();
            Color opaqueColor = fadeColor;
            opaqueColor.a = 1f;
            _fadeImage.color = opaqueColor;
            _fadeImage.raycastTarget = false;

            _fadeObjectRect = _fadeObject.GetComponent<RectTransform>();
            _fadeObjectRect.anchorMin = new Vector2(1, 0);
            _fadeObjectRect.anchorMax = new Vector2(0, 1);
            _fadeObjectRect.pivot = new Vector2(0.5f, 0.5f);
            _fadeObjectRect.sizeDelta = new Vector2(0.2f, 0.2f);
            _fadeObjectRect.localScale = new Vector2(2f, 2f);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (fadeOutOnSceneLoaded  && _fadeObject != null)
            {
                UpdateImageAlpha(1f);
                StartCoroutine(FadeOutWithDelay(sceneFadeInDelay));
            }
        }

        private IEnumerator FadeOutWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            DoFadeOut();
        }
        
        public virtual void DoFadeIn(Action onFadeComplete = null, Color? overrideColor = null, float? fadeTime = null)
        {
            if (_fadeRoutine != null)
                StopCoroutine(_fadeRoutine);
            
            _fadeImage.color = overrideColor ?? fadeColor;
            
            float speed = fadeTime ?? fadeInSpeed;

            _fadeRoutine = DoFade(_canvasGroup.alpha, 1f, speed, onFadeComplete);
            StartCoroutine(_fadeRoutine);
        }

        public virtual void DoFadeOut(Action onFadeComplete = null, float? fadeTime = null)
        {
            if (_fadeRoutine != null)
                StopCoroutine(_fadeRoutine);

            float speed = fadeTime ?? fadeOutSpeed;

            _fadeRoutine = DoFade(_canvasGroup.alpha, 0f, speed, onFadeComplete);
            StartCoroutine(_fadeRoutine);
        }
        
        public virtual void SetFadeLevel(float fadeLevel, float? fadeTime = null)
        {
            if (_fadeRoutine != null)
                StopCoroutine(_fadeRoutine);
            
            float speed = fadeTime ?? fadeInSpeed;

            _fadeRoutine = DoFade(_canvasGroup.alpha, fadeLevel, speed);
            StartCoroutine(_fadeRoutine);
        }

        IEnumerator DoFade(float alphaFrom, float alphaTo, float speed, Action onComplete = null)
        {
            float alpha = alphaFrom;
            UpdateImageAlpha(alpha);

            while (!Mathf.Approximately(alpha, alphaTo))
            {
                alpha = Mathf.MoveTowards(alpha, alphaTo, Time.deltaTime * speed);
                UpdateImageAlpha(alpha);
                yield return null;
            }

            UpdateImageAlpha(alphaTo);
            onComplete?.Invoke();
        }
        
        protected virtual void UpdateImageAlpha(float alpha)
        {
            if (_canvasGroup == null) return;

            if (!_canvasGroup.gameObject.activeSelf)
                _canvasGroup.gameObject.SetActive(true);

            _canvasGroup.alpha = alpha;

            if (Mathf.Approximately(alpha, 0f))
                _canvasGroup.gameObject.SetActive(false);
        }
    }
}

