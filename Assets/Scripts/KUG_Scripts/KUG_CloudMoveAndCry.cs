using UnityEngine;
using DG.Tweening;

namespace KucukGunes
{
    public class KUG_CloudMoveAndCry : MonoBehaviour
    {
        [Header("Referanslar")]
        public Transform cloudBody;
        public Transform eyeTear;
        public Transform tearStartPoint;
        public Transform tearEndPoint;
        public Transform targetTransform;

        [Header("Animasyon Ayarlari")]
        public float moveDuration = 1.5f;
        public float floatAmplitude = 0.1f;
        public float floatDuration = 1.4f;
        public float tearDropDuration = 1.2f;
        public float tearInterval = 0.4f;

        private Vector3 startPos;
        private Vector3 tearStartPos;
        private bool hasMoved = false;

        private Tween moveTween;
        private Tween floatTween;
        private Tween tearTween;

        private void Start()
        {
            // Baslangic konumu kaydet
            if (cloudBody != null)
                startPos = cloudBody.localPosition;
            else
                startPos = transform.localPosition;

            if (eyeTear != null && tearStartPoint != null)
            {
                tearStartPos = tearStartPoint.localPosition;
                eyeTear.localPosition = tearStartPos;
            }
        }

        private void OnMouseDown()
        {
            if (Input.GetMouseButtonDown(0) && !hasMoved)
            {
                hasMoved = true;
                StopAllTweens();

                if (targetTransform == null) return;

                // Bulutu hedefe yumusakca gotur
                moveTween = cloudBody.DOLocalMove(targetTransform.localPosition, moveDuration)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        StartFloatMotion();
                        StartTearLoop();
                    });
            }
        }

        private void StartFloatMotion()
        {
            if (cloudBody == null) return;

            floatTween = cloudBody.DOLocalMoveY(startPos.y + floatAmplitude, floatDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void StartTearLoop()
        {
            if (eyeTear == null || tearEndPoint == null || tearStartPoint == null)
                return;

            tearTween = eyeTear.DOLocalMove(tearEndPoint.localPosition, tearDropDuration)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    eyeTear.localPosition = tearStartPoint.localPosition;
                    DOVirtual.DelayedCall(tearInterval, StartTearLoop, false);
                });
        }

        private void StopAllTweens()
        {
            // Tum tweenleri temizle
            DOTween.Kill(transform);
            if (cloudBody != null) DOTween.Kill(cloudBody);
            if (eyeTear != null) DOTween.Kill(eyeTear);

            if (moveTween != null && moveTween.IsActive()) moveTween.Kill();
            if (floatTween != null && floatTween.IsActive()) floatTween.Kill();
            if (tearTween != null && tearTween.IsActive()) tearTween.Kill();
        }

        private void OnDisable()
        {
            // Tetiklenme bayragi sifirla
            hasMoved = false;

            // Tum animasyonlari durdur
            StopAllTweens();

            // Konumlari manuel sifirla
            if (cloudBody != null)
                cloudBody.localPosition = startPos;
            else
                transform.localPosition = startPos;

            if (eyeTear != null && tearStartPoint != null)
                eyeTear.localPosition = tearStartPos;
        }
    }
}
