using UnityEngine;
using DG.Tweening;
using System; // Action kullanimi icin gerekli

namespace BirDenizDI
{
    /// <summary>
    /// Bir objeye tiklandiginda, tavsan gibi ileri ve geri sekmesini saglayan script.
    /// Her tiklama, objeyi baslangic ve hedef pozisyonlari arasinda hareket ettirir.
    /// </summary>
    public class KUG_DuckBounceMove : MonoBehaviour
    {
        [Header("Ziplama Ayarlari")]
        [Tooltip("Tek bir ziplamada kat edilecek ileri mesafe.")]
        [SerializeField] private float hopDistance = 2f;
        [Tooltip("Ziplamanin ulasacagi maksimum yukseklik.")]
        [SerializeField] private float hopHeight = 0.5f;
        [Tooltip("Tek bir ziplama animasyonunun toplam suresi.")]
        [SerializeField] private float hopDuration = 0.6f;

        // Baslangic durumlarini saklamak icin
        private Vector3 initialPosition;
        private Vector3 initialScale;
        private Quaternion initialRotation;

        // Animasyonun hedef noktasini saklamak icin
        private Vector3 targetPosition;

        // Durum kontrolu
        private bool isAtStartPosition = true;
        private bool isAnimating = false;

        private void Awake()
        {
            // Baslangic degerlerini kaydet
            initialPosition = transform.localPosition;
            initialScale = transform.localScale;
            initialRotation = transform.localRotation;

            // Hedef pozisyonu baslangicta bir kere hesapla
            targetPosition = initialPosition + new Vector3(hopDistance, 0, 0);
        }

        private void OnDisable()
        {
            // Tum animasyonlari aninda oldur
            DOTween.Kill(transform);

            // Objenin durumunu baslangic haline sifirla
            transform.localPosition = initialPosition;
            transform.localScale = initialScale;
            transform.localRotation = initialRotation;

            // Durum bayraklarini sifirla
            isAtStartPosition = true;
            isAnimating = false;
        }

        private void OnMouseDown()
        {
            // Kural: Sol mouse tiklamasini ve animasyon durumunu kontrol et
            if (Input.GetMouseButtonDown(0) && !isAnimating)
            {
                // Gidilecek hedefi belirle: Eger baslangictaysan hedefe, degilsen baslangica git.
                Vector3 destination = isAtStartPosition ? targetPosition : initialPosition;

                // Ziplama animasyonunu baslat
                HopTo(destination, () =>
                {
                    // Animasyon tamamlandiginda calisacak kod:
                    isAtStartPosition = !isAtStartPosition; // Konum durumunu tersine cevir
                    isAnimating = false; // Artik yeni tiklamalara acik
                });
            }
        }

        /// <summary>
        /// Belirtilen hedefe, ezilip uzayarak ziplama animasyonu uygular.
        /// </summary>
        /// <param name="destination">Gidilecek hedef pozisyon</param>
        /// <param name="onComplete">Animasyon bitince tetiklenecek eylem</param>
        private void HopTo(Vector3 destination, Action onComplete)
        {
            isAnimating = true;

            // Ziplama kavisinin tepe noktasi
            Vector3 midPoint = (transform.localPosition + destination) / 2f;
            midPoint.y += hopHeight;

            Sequence sequence = DOTween.Sequence();

            // 1. Ezilme (Ziplamaya hazirlik)
            sequence.Append(transform.DOScale(new Vector3(1.1f, 0.9f, 1f), hopDuration * 0.15f));

            // 2. Ziplama (Kavisli hareket ve havada uzama)
            sequence.Append(transform.DOLocalMove(midPoint, hopDuration * 0.35f).SetEase(Ease.OutQuad));
            sequence.Join(transform.DOScale(new Vector3(0.9f, 1.1f, 1f), hopDuration * 0.35f));

            // 3. Inis
            sequence.Append(transform.DOLocalMove(destination, hopDuration * 0.35f).SetEase(Ease.InQuad));

            // 4. Yere Vurma ve Normale Donme
            sequence.Append(transform.DOScale(new Vector3(1.1f, 0.9f, 1f), hopDuration * 0.15f));
            sequence.Append(transform.DOScale(initialScale, hopDuration * 0.1f));

            // Tum animasyon bittiginde, parametre olarak gelen onComplete eylemini cagir.
            sequence.OnComplete(() => onComplete?.Invoke());
        }
    }
}