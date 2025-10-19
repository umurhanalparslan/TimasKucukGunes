using UnityEngine;
using DG.Tweening;

namespace BirDenizDI // Namespace "BirDenizDI" olarak guncellendi
{
    // Orijinal sinif adi "SunGirlReveal" korundu, sadece prefix eklendi
    public class BDDI_SunGirlReveal : MonoBehaviour
    {
        [Header("Gorsel Referanslar")]
        [Tooltip("Karakterin ana govdesi")]
        [SerializeField] private Transform sunBody;
        [Tooltip("Sol kol")]
        [SerializeField] private Transform leftArm;
        [Tooltip("Sag kol")]
        [SerializeField] private Transform rightArm;
        [Tooltip("Goz kirpma icin kullanilacak 'goz kapagi' gorseli")]
        [SerializeField] private SpriteRenderer eyelidSprite;

        [Header("Animasyon Ayarlari")]
        [Tooltip("Belirme animasyonunun baslayacagi ilk olcek (varsayilan: 0.7)")]
        [SerializeField] private float appearStartScale = 0.7f;
        [Tooltip("Govdenin belirme animasyonunun suresi")]
        [SerializeField] private float appearDuration = 0.5f;
        [Tooltip("Kollarin donme acisi")]
        [SerializeField] private float armRotationAngle = 25f;
        [Tooltip("Kollarin sallanma suresi")]
        [SerializeField] private float armWaveDuration = 0.8f;
        [Tooltip("Goz kapaginin kapanip acilma suresi")]
        [SerializeField] private float blinkDuration = 0.1f;
        [Tooltip("Iki goz kirpma arasindaki bekleme suresi")]
        [SerializeField] private float blinkLoopInterval = 1.5f;
        [Tooltip("Govdenin havada süzülme mesafesi")]
        [SerializeField] private float floatAmplitude = 0.1f;
        [Tooltip("Bir süzülme dongusunun suresi")]
        [SerializeField] private float floatDuration = 1.2f;

        // Baslangic durumlarini saklamak icin
        private Vector3 initialScale;
        private Vector3 initialPosition;
        private Quaternion leftArmInitialRotation;
        private Quaternion rightArmInitialRotation;

        private bool hasAppeared = false;

        private void Awake()
        {
            // Objelerin animasyon bittigindeki son hallerini kaydet
            if (sunBody != null)
            {
                initialScale = sunBody.localScale;
                initialPosition = sunBody.localPosition;
            }
            if (leftArm != null) leftArmInitialRotation = leftArm.localRotation;
            if (rightArm != null) rightArmInitialRotation = rightArm.localRotation;

            // Her seyi baslangic (tiklanmaya hazir) durumuna getir
            ResetState();
        }

        private void OnDisable()
        {
            ResetState();
        }

        private void OnMouseDown()
        {
            if (Input.GetMouseButtonDown(0) && !hasAppeared)
            {
                // Belirme animasyonunu baslat
                PlayRevealAnimation();
            }
        }

        /// <summary>
        /// Karakterin ilk belirme animasyonunu oynatir.
        /// </summary>
        private void PlayRevealAnimation()
        {
            hasAppeared = true;

            if (sunBody != null)
            {
                // Kucuk olcekten baslayarak normal boyutuna ulas
                sunBody.DOScale(initialScale, appearDuration)
                    .SetEase(Ease.OutBack)
                    .OnComplete(StartLoopingAnimations); // Bitince dongusel animasyonlari baslat
            }
            else
            {
                // Eger govde atanmamissa, dogrudan donguleri baslat
                StartLoopingAnimations();
            }
        }

        /// <summary>
        /// Belirme animasyonu bittikten sonra baslayan surekli animasyonlari yonetir.
        /// </summary>
        private void StartLoopingAnimations()
        {
            // Goz kirpma dongusu
            if (eyelidSprite != null)
            {
                Sequence blinkSequence = DOTween.Sequence();
                // Acik bekle -> HIZLICA KAPAN -> HIZLICA ACIL
                blinkSequence.AppendInterval(blinkLoopInterval);
                blinkSequence.Append(eyelidSprite.DOFade(0f, blinkDuration));
                blinkSequence.Append(eyelidSprite.DOFade(1f, blinkDuration));
                blinkSequence.SetLoops(-1); // Bu sekansi sonsuz donguye al
            }

            // Kol sallama dongusu
            if (leftArm != null)
            {
                leftArm.DOLocalRotate(new Vector3(0, 0, armRotationAngle), armWaveDuration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }
            if (rightArm != null)
            {
                rightArm.DOLocalRotate(new Vector3(0, 0, -armRotationAngle), armWaveDuration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }

            // Govde süzülme dongusu
            if (sunBody != null)
            {
                sunBody.DOLocalMoveY(initialPosition.y + floatAmplitude, floatDuration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }
        }

        /// <summary>
        /// Tum objeleri ve animasyonlari baslangic (tiklanmaya hazir) durumuna sifirlar.
        /// </summary>
        private void ResetState()
        {
            // Tum animasyonlari oldur
            DOTween.Kill(transform);
            if (sunBody != null) DOTween.Kill(sunBody);
            if (leftArm != null) DOTween.Kill(leftArm);
            if (rightArm != null) DOTween.Kill(rightArm);
            if (eyelidSprite != null) DOTween.Kill(eyelidSprite);

            // Govdeyi sifirla: Baslangic pozisyonuna ve KUCUK olcege getir
            if (sunBody != null)
            {
                sunBody.localPosition = initialPosition;
                sunBody.localScale = initialScale * appearStartScale;
            }

            // Kollari sifirla
            if (leftArm != null) leftArm.localRotation = leftArmInitialRotation;
            if (rightArm != null) rightArm.localRotation = rightArmInitialRotation;

            // Gozleri sifirla: Goz kapagi gorunmez (alpha=0) yani gozler ACIK
            if (eyelidSprite != null)
            {
                eyelidSprite.color = new Color(eyelidSprite.color.r, eyelidSprite.color.g, eyelidSprite.color.b, 1f);
            }

            hasAppeared = false;
        }
    }
}