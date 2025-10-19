using UnityEngine;
using DG.Tweening;

namespace KucukGunes
{
    public class KUG_SunRevealEffect : MonoBehaviour
    {
        [Header("Ana Obje ve Kollar")]
        public SpriteRenderer bodyRenderer;
        public Transform[] arms;

        [Header("Efekt Ayarlari")]
        public float fadeDuration = 1f;
        public float scaleAmount = 1.1f;
        public float scaleDuration = 0.4f;
        public float armMoveAngle = 20f;
        public float armAnimDuration = 0.5f;

        [Header("Gorundukten Sonra Efekt")]
        public float loopScaleFactor = 1.05f;
        public float loopScaleDuration = 1.2f;
        public float swingAngle = 4f;
        public float swingDuration = 1.5f;

        private Vector3 startPos;
        private Quaternion[] initialArmRotations;
        private Vector3 initialScale;
        private float initialAlpha;

        private bool hasRevealed = false;

        private void Start()
        {
            // Baslangic pozisyonlarini ve rotasyonlarini kaydet
            startPos = transform.localPosition;
            initialScale = transform.localScale;
            initialAlpha = bodyRenderer != null ? bodyRenderer.color.a : 1f;

            if (bodyRenderer != null)
            {
                Color c = bodyRenderer.color;
                c.a = 0f;
                bodyRenderer.color = c;
            }

            if (arms != null && arms.Length > 0)
            {
                initialArmRotations = new Quaternion[arms.Length];
                for (int i = 0; i < arms.Length; i++)
                {
                    if (arms[i] != null)
                        initialArmRotations[i] = arms[i].localRotation;
                }
            }
        }

        private void OnMouseDown()
        {
            if (Input.GetMouseButtonDown(0) && !hasRevealed)
            {
                hasRevealed = true;
                DOTween.Kill(transform);

                if (bodyRenderer != null)
                {
                    Color c = bodyRenderer.color;
                    c.a = 1f;
                    bodyRenderer.DOFade(1f, fadeDuration).SetEase(Ease.InOutSine);
                }

                // Efektli buyut-kucult
                transform.DOLocalMove(startPos, 0f); // pozisyonu sabitle
                transform.DOScale(initialScale * scaleAmount, scaleDuration)
                         .SetEase(Ease.OutBack)
                         .OnComplete(() =>
                         {
                             transform.DOScale(initialScale, scaleDuration).SetEase(Ease.InOutSine)
                             .OnComplete(() =>
                             {
                                 // Sonsuz efektlere basla
                                 StartLoopingEffects();
                             });
                         });

                // Kollari oynat
                if (arms != null && arms.Length > 0)
                {
                    foreach (Transform arm in arms)
                    {
                        if (arm != null)
                        {
                            arm.DOLocalRotate(new Vector3(0, 0, armMoveAngle), armAnimDuration)
                                .SetEase(Ease.OutQuad)
                                .SetLoops(2, LoopType.Yoyo);
                        }
                    }
                }
            }
        }

        private void StartLoopingEffects()
        {
            // Sonsuz scale loop
            transform.DOScale(initialScale * loopScaleFactor, loopScaleDuration)
                     .SetEase(Ease.InOutSine)
                     .SetLoops(-1, LoopType.Yoyo);

            // Sonsuz salinma loop
            transform.DOLocalRotate(new Vector3(0, 0, swingAngle), swingDuration)
                     .SetEase(Ease.InOutSine)
                     .SetLoops(-1, LoopType.Yoyo);
        }

        private void OnDisable()
        {
            // Tweenleri iptal et
            DOTween.Kill(transform);

            // Pozisyon, olcek, rotasyon sifirla
            transform.localPosition = startPos;
            transform.localScale = initialScale;
            transform.localRotation = Quaternion.identity;

            // Gunes alpha sifirla
            if (bodyRenderer != null)
            {
                Color c = bodyRenderer.color;
                c.a = 0f;
                bodyRenderer.color = c;
            }

            // Kollarin rotasyonunu sifirla
            if (arms != null && arms.Length > 0 && initialArmRotations != null)
            {
                for (int i = 0; i < arms.Length; i++)
                {
                    if (arms[i] != null)
                        arms[i].localRotation = initialArmRotations[i];
                }
            }

            hasRevealed = false;
        }
    }
}
