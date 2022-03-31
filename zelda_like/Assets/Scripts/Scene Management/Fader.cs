using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ZL.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        // Cached Reference
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = this.GetComponent<CanvasGroup>();
        }

        public IEnumerator FadeOut(float time)
        {
            //Debug.Log("Fade Out");
            _canvasGroup.alpha = 0;
            while (_canvasGroup.alpha < 1)
            {
                _canvasGroup.alpha += Time.deltaTime / time;
                yield return null;
            }

            _canvasGroup.alpha = 1;
        }

        public void FadeOutImmediate()
        {
            //Debug.Log("Fade Out Immediate");
            _canvasGroup.alpha = 1;
        }

        public void FadeInImmediate()
        {
            //Debug.Log("Fade Out Immediate");
            _canvasGroup.alpha = 0;
        }

        public IEnumerator FadeIn(float time)
        {
            //Debug.Log("Fade In");
            _canvasGroup.alpha = 1;
            while (_canvasGroup.alpha > 0)
            {
                _canvasGroup.alpha -= Time.deltaTime / time;
                yield return null;
            }
            _canvasGroup.alpha = 0;
        }

        private IEnumerator Flash(float oneFlashTime, float flashScreenDuration, Color color)
        {
            if (oneFlashTime >= flashScreenDuration) yield break;

            ChangeColor(color);
            float flashDuration = oneFlashTime / 2;
            for (float t = 0; t <= flashScreenDuration; t += Time.deltaTime)
            {
                //Debug.Log("Time t: " + t);
                yield return FadeOut(flashDuration);
                t += flashDuration;
                yield return FadeIn(flashDuration);
                t += flashDuration;
            }

            FadeInImmediate();
            ChangeColor(color);
        }

        public void FlashScreen(float oneFlashTime, float flashScreenDuration, Color color)
        {
            //Debug.Log("Flash Screen");
            StartCoroutine(Flash(oneFlashTime, flashScreenDuration, color));
        }

        public void ChangeColor(Color newColor)
        {
            this.gameObject.GetComponent<Image>().color = newColor;
        }
    }
}
