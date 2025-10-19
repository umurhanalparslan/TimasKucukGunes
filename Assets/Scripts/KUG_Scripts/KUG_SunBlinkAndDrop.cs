using UnityEngine;
using DG.Tweening;

namespace KucukGunes
{
    public class KUG_SunBlinkAndDrop : MonoBehaviour
    {
        [Header("Referanslar")]
        public SpriteRenderer eyeRenderer;
        public Transform eyeDrop;
        public Transform eyeDropStartPoint;
        public Transform eyeDropEndPoint;
        public Transform leftArm;
        public Transform rightArm;

        [Header("Blink Ayarlari")]
        public float blinkFadeDuration = 0.4f;
        public float blinkDelay = 1.2f;

        [Header("Kol Ayarlari")]
        public float armRotateAngle = 18f;
        public float armDuration = 0.8f;

        [Header("Damla Ayarlari")]
        public float dropDuration = 1.3f;
        public float dropInterval = 0.4f;

        private Quaternion leftArmStartRot;
        private Quaternion rightArmStartRot;
        private Vector3 dropStartPos;
        private float eyeStartAlpha;
        private bool hasTriggered = false;

        private Tween dropTween;
        private Sequence blinkSequence;

        private void Start()
        {
            // Baslangic degerlerini kaydet
            if (leftArm != null) leftArmStartRot = leftArm.localRotation;
            if (rightArm != null) rightArmStartRot = rightArm.localRotation;

            if (eyeRenderer != null)
            {
                eyeStartAlpha = eyeRenderer.color.a;
                Color c = eyeRenderer.color;
                c.a = 0f;
                eyeRenderer.color = c;
            }

            if (eyeDrop != null && eyeDropStartPoint != null)
            {
                dropStartPos = eyeDropStartPoint.localPosition;
                eyeDrop.localPosition = dropStartPos;
            }
        }

        private void OnMouseDown()
        {
            if (Input.GetMouseButtonDown(0) && !hasTriggered)
            {
                hasTriggered = true;
                DOTween.Kill(transform);

                // Goz blink loop
                if (eyeRenderer != null)
                {
                    blinkSequence = DOTween.Sequence().SetLoops(-1).SetDelay(blinkDelay);
                    blinkSequence.Append(eyeRenderer.DOFade(eyeStartAlpha, blinkFadeDuration).SetEase(Ease.InOutSine));
                    blinkSequence.Append(eyeRenderer.DOFade(0f, blinkFadeDuration).SetEase(Ease.InOutSine));
                }

                // Kollari hareket ettir
                if (leftArm != null)
                {
                    leftArm.DOLocalRotate(new Vector3(0, 0, armRotateAngle), armDuration)
                        .SetEase(Ease.InOutSine)
                        .SetLoops(-1, LoopType.Yoyo);
                }

                if (rightArm != null)
                {
                    rightArm.DOLocalRotate(new Vector3(0, 0, -armRotateAngle), armDuration)
                        .SetEase(Ease.InOutSine)
                        .SetLoops(-1, LoopType.Yoyo);
                }

                // Damla loop
                if (eyeDrop != null && eyeDropEndPoint != null)
                {
                    StartDropLoop();
                }
            }
        }

        private void StartDropLoop()
        {
            if (eyeDrop == null || eyeDropEndPoint == null || eyeDropStartPoint == null)
                return;

            dropTween = eyeDrop.DOLocalMove(eyeDropEndPoint.localPosition, dropDuration)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    eyeDrop.localPosition = eyeDropStartPoint.localPosition;
                    DOVirtual.DelayedCall(dropInterval, StartDropLoop, false);
                });
        }

        private void OnDisable()
        {
            hasTriggered = false;

            // Tüm tweenleri tek tek öldür
            DOTween.Kill(transform);
            if (leftArm != null) DOTween.Kill(leftArm);
            if (rightArm != null) DOTween.Kill(rightArm);
            if (eyeRenderer != null) DOTween.Kill(eyeRenderer);
            if (eyeDrop != null) DOTween.Kill(eyeDrop);

            if (dropTween != null && dropTween.IsActive()) dropTween.Kill();
            if (blinkSequence != null && blinkSequence.IsActive()) blinkSequence.Kill();

            // Kollari sifirla
            if (leftArm != null) leftArm.localRotation = leftArmStartRot;
            if (rightArm != null) rightArm.localRotation = rightArmStartRot;

            // Gozu sifirla
            if (eyeRenderer != null)
            {
                Color c = eyeRenderer.color;
                c.a = 0f;
                eyeRenderer.color = c;
            }

            // Damlayi sifirla
            if (eyeDrop != null && eyeDropStartPoint != null)
                eyeDrop.localPosition = eyeDropStartPoint.localPosition;

            // Objeyi sifirla
            if (transform != null)
                transform.localPosition = Vector3.zero;
        }

    }
}
