using UnityEngine;
using DG.Tweening;

namespace KucukGunes
{
    public class KUG_KingJumpLaugh : MonoBehaviour
    {
        [Header("Kral Parcalari")]
        public Transform body; // tum karakter
        public Transform head; // kafasini sallamak icin
        public Transform[] arms; // varsa kollari da oynasin

        [Header("Ziplama ve Efekt Ayarlari")]
        public float jumpHeight = 0.5f;
        public float jumpDuration = 0.4f;
        public float headShakeAngle = 15f;
        public float headShakeDuration = 0.25f;
        public float laughScale = 1.1f;
        public float laughDuration = 0.3f;

        private Vector3 startPos;
        private Vector3 initialScale;
        private Quaternion headStartRotation;
        private Quaternion[] armStartRotations;

        private bool isAnimating = false;

        private void Start()
        {
            startPos = transform.localPosition;
            initialScale = transform.localScale;

            if (head != null)
                headStartRotation = head.localRotation;

            if (arms != null && arms.Length > 0)
            {
                armStartRotations = new Quaternion[arms.Length];
                for (int i = 0; i < arms.Length; i++)
                {
                    if (arms[i] != null)
                        armStartRotations[i] = arms[i].localRotation;
                }
            }
        }

        private void OnMouseDown()
        {
            if (Input.GetMouseButtonDown(0) && !isAnimating)
            {
                isAnimating = true;
                DOTween.Kill(transform);

                // Ziplama animasyonu
                body.DOLocalMoveY(startPos.y + jumpHeight, jumpDuration / 2f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        body.DOLocalMoveY(startPos.y, jumpDuration / 2f)
                            .SetEase(Ease.InQuad);
                    });

                // Kafa sallama (kahkaha gibi)
                if (head != null)
                {
                    head.DOLocalRotate(new Vector3(0, 0, headShakeAngle), headShakeDuration)
                        .SetEase(Ease.InOutSine)
                        .SetLoops(4, LoopType.Yoyo)
                        .OnComplete(() =>
                        {
                            head.localRotation = headStartRotation;
                        });
                }

                // Kollar titrese olur (minik loop)
                if (arms != null && arms.Length > 0)
                {
                    foreach (Transform arm in arms)
                    {
                        if (arm != null)
                        {
                            arm.DOLocalRotate(new Vector3(0, 0, 10f), 0.2f)
                                .SetEase(Ease.InOutSine)
                                .SetLoops(2, LoopType.Yoyo);
                        }
                    }
                }

                // Hohoho efekti gibi buyume-kuculme
                transform.DOScale(initialScale * laughScale, laughDuration)
                         .SetEase(Ease.OutBack)
                         .SetLoops(2, LoopType.Yoyo)
                         .OnComplete(() => isAnimating = false);
            }
        }

        private void OnDisable()
        {
            DOTween.Kill(transform);

            // Pozisyon, olcek, rotasyon reset
            transform.localPosition = startPos;
            transform.localScale = initialScale;

            if (head != null)
                head.localRotation = headStartRotation;

            if (arms != null && arms.Length > 0 && armStartRotations != null)
            {
                for (int i = 0; i < arms.Length; i++)
                {
                    if (arms[i] != null)
                        arms[i].localRotation = armStartRotations[i];
                }
            }

            isAnimating = false;
        }
    }
}
