using UnityEngine;
using DG.Tweening;

namespace KucukGunes
{
    [System.Serializable]
    public class SunItem
    {
        public Transform sunTransform;
        public SpriteRenderer sunRenderer;
        public Transform targetPosition;

        [HideInInspector] public Vector3 startPos;
        [HideInInspector] public Quaternion startRot;
        [HideInInspector] public Vector3 startScale;
    }

    public class KUG_SunGroupEnter : MonoBehaviour
    {
        [Header("Gunes Listesi")]
        public SunItem[] suns;

        [Header("Animasyon Ayarlari")]
        public float moveDuration = 1.2f;
        public float fadeDuration = 0.6f;
        public float floatBaseAmplitude = 0.15f;
        public float floatBaseDuration = 1.5f;

        [Header("Fade Scale Efekti")]
        public float appearScaleAmount = 1.2f;
        public float appearScaleDuration = 0.4f;

        private bool hasEntered = false;

        private void Start()
        {
            foreach (var sun in suns)
            {
                if (sun.sunTransform != null)
                {
                    sun.startPos = sun.sunTransform.localPosition;
                    sun.startRot = sun.sunTransform.localRotation;
                    sun.startScale = sun.sunTransform.localScale;
                }

                if (sun.sunRenderer != null)
                {
                    Color c = sun.sunRenderer.color;
                    c.a = 0f;
                    sun.sunRenderer.color = c;
                }
            }
        }

        private void OnMouseDown()
        {
            if (Input.GetMouseButtonDown(0) && !hasEntered)
            {
                hasEntered = true;

                foreach (var sun in suns)
                {
                    if (sun.sunTransform == null || sun.targetPosition == null || sun.sunRenderer == null)
                        continue;

                    DOTween.Kill(sun.sunTransform);

                    // Ilk olarak scale buyume efekti
                    sun.sunTransform.localScale = sun.startScale * 0.8f; // biraz kucuk basla
                    sun.sunTransform.DOScale(sun.startScale * appearScaleAmount, appearScaleDuration * 0.5f)
                        .SetEase(Ease.OutBack)
                        .OnComplete(() =>
                        {
                            sun.sunTransform.DOScale(sun.startScale, appearScaleDuration * 0.5f)
                                .SetEase(Ease.InOutSine);
                        });

                    // Fade-in
                    Color currentColor = sun.sunRenderer.color;
                    currentColor.a = 0f;
                    sun.sunRenderer.color = currentColor;
                    sun.sunRenderer.DOFade(1f, fadeDuration)
                        .SetEase(Ease.InOutSine);

                    // Hareketli gelis
                    sun.sunTransform.DOLocalMove(sun.targetPosition.localPosition, moveDuration)
                        .SetEase(Ease.OutBack)
                        .OnComplete(() =>
                        {
                            // Random float animasyonlar
                            float randomOffset = Random.Range(0.1f, floatBaseAmplitude);
                            float randomDuration = Random.Range(floatBaseDuration * 0.8f, floatBaseDuration * 1.2f);
                            float randomDelay = Random.Range(0f, 0.3f);
                            float direction = Random.value > 0.5f ? 1f : -1f;

                            sun.sunTransform.DOLocalMoveY(
                                sun.targetPosition.localPosition.y + randomOffset * direction,
                                randomDuration)
                            .SetEase(Ease.InOutSine)
                            .SetLoops(-1, LoopType.Yoyo)
                            .SetDelay(randomDelay);
                        });
                }
            }
        }

        private void OnDisable()
        {
            hasEntered = false;

            foreach (var sun in suns)
            {
                if (sun.sunTransform == null || sun.sunRenderer == null)
                    continue;

                DOTween.Kill(sun.sunTransform);

                sun.sunTransform.localPosition = sun.startPos;
                sun.sunTransform.localRotation = sun.startRot;
                sun.sunTransform.localScale = sun.startScale;

                Color c = sun.sunRenderer.color;
                c.a = 0f;
                sun.sunRenderer.color = c;
            }
        }
    }
}
