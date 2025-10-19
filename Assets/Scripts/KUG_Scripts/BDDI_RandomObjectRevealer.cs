using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BirDenizDI
{
    /// <summary>
    /// Bir grup GameObject'i (ve cocuklarini), tiklandiginda rastgele sirayla, tek tek, 
    /// estetik bir animasyonla ortaya cikarir ve ardindan surekli sallanmalarini saglar.
    /// CALISMASI ICIN LISTEDEKI HER GAMEOBJECT'TE BIR CANVASGROUP COMPONENT'I OLMALIDIR.
    /// </summary>
    public class BDDI_RandomObjectRevealer : MonoBehaviour
    {
        [Header("Kontrol Edilecek Obje Gruplari")]
        [Tooltip("Animasyonla ortaya cikacak olan tum ana GameObject'leri bu listeye ekleyin.")]
        [SerializeField] private List<GameObject> objectsToReveal;

        [Header("Animasyon Ayarlari")]
        [Tooltip("Tek bir objenin belirme (dusme ve gorunur olma) animasyonunun suresi.")]
        [SerializeField] private float appearDuration = 0.6f;
        [Tooltip("Objelerin ne kadar yukaridan dusecegi.")]
        [SerializeField] private float dropDistance = 0.5f;
        [Tooltip("Objelerin ortaya cikmasi arasindaki gecikme suresi.")]
        [SerializeField] private float delayBetweenObjects = 0.2f;
        [Tooltip("Surekli sallanma animasyonunun donme acisi.")]
        [SerializeField] private float swingAngle = 8f;
        [Tooltip("Bir sallanma dongusunun ne kadar surecegi.")]
        [SerializeField] private float swingDuration = 1.8f;

        [Header("Ses Ayarlari")]
        [Tooltip("(Opsiyonel) Her bir obje ortaya cikarken calacak ses.")]
        [SerializeField] private AudioSource popSound;

        // Objelerin baslangic durumlarini ve CanvasGroup'larini saklamak icin
        private Dictionary<GameObject, Vector3> initialPositions = new Dictionary<GameObject, Vector3>();
        private Dictionary<GameObject, Quaternion> initialRotations = new Dictionary<GameObject, Quaternion>();
        private Dictionary<GameObject, CanvasGroup> canvasGroups = new Dictionary<GameObject, CanvasGroup>();

        // Bu script'in eklendigi objenin baslangic durumu
        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private Vector3 initialScale;

        private bool hasBeenClicked = false;

        private void Awake()
        {
            initialPosition = transform.localPosition;
            initialRotation = transform.localRotation;
            initialScale = transform.localScale;

            if (objectsToReveal != null)
            {
                foreach (var obj in objectsToReveal)
                {
                    if (obj == null) continue;
                    initialPositions[obj] = obj.transform.localPosition;
                    initialRotations[obj] = obj.transform.localRotation;

                    // CanvasGroup component'ini bul ve sakla
                    CanvasGroup cg = obj.GetComponent<CanvasGroup>();
                    if (cg == null)
                    {
                        Debug.LogError("'" + obj.name + "' objesinde CanvasGroup component'i eksik! Lutfen ekleyin.", obj);
                        continue; // Hata varsa bu objeyi atla
                    }
                    canvasGroups[obj] = cg;
                }
            }

            ResetState();
        }

        private void OnDisable()
        {
            ResetState();
        }

        private void OnMouseDown()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (hasBeenClicked) return;

            PlayRevealAnimation();
        }

        private void PlayRevealAnimation()
        {
            hasBeenClicked = true;
            StartCoroutine(RevealObjectsInRandomOrder());
        }

        private IEnumerator RevealObjectsInRandomOrder()
        {
            if (objectsToReveal == null || objectsToReveal.Count == 0) yield break;

            List<GameObject> shuffledList = objectsToReveal.OrderBy(a => Random.value).ToList();

            foreach (var obj in shuffledList)
            {
                if (obj != null && canvasGroups.ContainsKey(obj))
                {
                    AnimateSingleObject(obj);
                    if (popSound != null)
                    {
                        popSound.Play();
                    }
                }
                yield return new WaitForSeconds(delayBetweenObjects);
            }
        }

        private void AnimateSingleObject(GameObject targetObject)
        {
            Vector3 startPos = initialPositions[targetObject];
            Vector3 dropStartPos = startPos + new Vector3(0, dropDistance, 0);
            CanvasGroup cg = canvasGroups[targetObject];

            Sequence sequence = DOTween.Sequence();

            sequence.AppendCallback(() =>
            {
                targetObject.SetActive(true);
                targetObject.transform.localPosition = dropStartPos;
                targetObject.transform.localScale = Vector3.zero;
                cg.alpha = 0f;
            });

            sequence.Append(targetObject.transform.DOLocalMove(startPos, appearDuration).SetEase(Ease.OutBounce));
            sequence.Join(targetObject.transform.DOScale(1f, appearDuration).SetEase(Ease.OutBack));
            sequence.Join(cg.DOFade(1f, appearDuration * 0.8f));

            sequence.OnComplete(() =>
            {
                targetObject.transform.DOLocalRotate(new Vector3(0, 0, swingAngle), swingDuration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            });
        }

        private void ResetState()
        {
            StopAllCoroutines();
            DOTween.Kill(transform);
            transform.localPosition = initialPosition;
            transform.localRotation = initialRotation;
            transform.localScale = initialScale;

            if (objectsToReveal != null)
            {
                foreach (var obj in objectsToReveal)
                {
                    if (obj == null) continue;
                    DOTween.Kill(obj.transform, true);
                    if (canvasGroups.ContainsKey(obj))
                    {
                        DOTween.Kill(canvasGroups[obj], true);
                    }

                    obj.transform.localPosition = initialPositions[obj];
                    obj.transform.localRotation = initialRotations[obj];
                    obj.transform.localScale = Vector3.one;
                    obj.SetActive(false);
                }
            }

            hasBeenClicked = false;
        }
    }
}