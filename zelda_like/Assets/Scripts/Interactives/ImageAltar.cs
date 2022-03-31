using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ZL.Interactives
{
    public class ImageAltar : MonoBehaviour, IInteractable
    {
        [SerializeField] GameObject _popup;
        [SerializeField] Image _image;
        [SerializeField] Sprite _imageSource;

        [Space(10)]
        [Header("-------- Fader Time Variables --------")]
        [SerializeField] private float _fadeOutTime = 1f;
        [SerializeField] private float _fadeInTime = 0.5f;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _image.sprite = _imageSource;
            _popup.SetActive(false);
            _canvasGroup = _popup.GetComponentInChildren<CanvasGroup>();
        }


        public void Close()
        {
            //Debug.Log("Image Altar Close");
            StartCoroutine("FadeIn");

        }

        public void CloseInteractionWithAltar()
        {
            if (_popup.activeInHierarchy)
                Close();
        }

        public void Interact()
        {
            //Debug.Log("Image Altar Interact");
            StartCoroutine("FadeOut");
        }


        public void InteractWithAltar()
        {
            if (Input.GetButtonDown("Submit") && !_popup.activeInHierarchy)
                Interact();
            if (Input.GetButtonDown("Cancel") && _popup.activeInHierarchy)
                Close();
        }

        public IEnumerator FadeIn()
        {
            //Debug.Log("Fade In");
            _canvasGroup.alpha = 1;
            while (_canvasGroup.alpha > 0)
            {
                _canvasGroup.alpha -= Time.deltaTime / _fadeInTime;
                yield return null;
            }
            _canvasGroup.alpha = 0;
            _popup.SetActive(false);
        }

        public IEnumerator FadeOut()
        {
            //Debug.Log("Fade Out");
            _canvasGroup.alpha = 0;
            _popup.SetActive(true);
            while (_canvasGroup.alpha < 1)
            {
                _canvasGroup.alpha += Time.deltaTime / _fadeOutTime;
                yield return null;
            }
            _canvasGroup.alpha = 1;
        }
    }
}