using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GoldenSpin.CoreModule;

namespace GoldenSpin.WheelModule
{
    public class WheelSliceItem : MonoBehaviour
    {
        [SerializeField]
        private Image uiImageSliceIcon;

        [SerializeField]
        private TMP_Text uiTextSliceLabelValue;

        [SerializeField]
        private GameObject bombIndicator;

        private const string AMOUNT_PREFIX = "x";
        private const float WIN_ICON_SCALE_TARGET = 1.2f;
        private const float WIN_ICON_SCALE_DURATION = 0.3f;

        private bool isBomb;
        private Tween winTween;

        public void Initialize(WheelSliceDTO dto)
        {
            isBomb = dto.ContentType == SliceContentType.Bomb;

            bombIndicator.SetActive(isBomb);
            uiImageSliceIcon.gameObject.SetActive(!isBomb);
            uiTextSliceLabelValue.gameObject.SetActive(!isBomb);

            if (isBomb || dto.Reward == null)
            {
                return;
            }

            uiTextSliceLabelValue.text = $"{AMOUNT_PREFIX}{dto.Reward.Amount}";

            if (dto.Reward.Icon != null)
            {
                SetSliceIcon(dto.Reward.Icon);
            }
        }

        public void PlayWinAnimation()
        {
            if (isBomb)
            {
                return;
            }

            winTween?.Kill();
            uiImageSliceIcon.transform.localScale = Vector3.one;

            winTween = uiImageSliceIcon.transform
                .DOScale(WIN_ICON_SCALE_TARGET, WIN_ICON_SCALE_DURATION)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void StopWinAnimation()
        {
            winTween?.Kill();
            winTween = null;
            uiImageSliceIcon.transform.localScale = Vector3.one;
        }

        private void SetSliceIcon(Sprite sprite)
        {
            uiImageSliceIcon.sprite = sprite;
        }

        private void OnDestroy()
        {
            winTween?.Kill();
        }
    }
}
