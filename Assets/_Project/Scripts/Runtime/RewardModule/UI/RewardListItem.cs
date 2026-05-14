using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.RewardModule
{
    public class RewardListItem : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text uiTextRewardLabelValue;

        [SerializeField]
        private Image uiImageRewardIcon;

        [SerializeField]
        private AspectRatioFitter sliceIconFitter;

        private const float APPEAR_SCALE_START = 0f;
        private const float APPEAR_SCALE_END = 1f;
        
        private const string LABEL_FORMAT = "{0}  <color={1}>x{2}</color>";
        private const string REWARD_AMOUNT_COLOR = "#FFD900";

        public void Initialize(RewardDTO dto)
        {
            uiTextRewardLabelValue.text = string.Format(LABEL_FORMAT, dto.Label, REWARD_AMOUNT_COLOR, dto.Amount);

            if (dto.Icon != null)
            {
                uiImageRewardIcon.sprite = dto.Icon;
                sliceIconFitter.aspectRatio = dto.Icon.rect.width / dto.Icon.rect.height;
            }

            PlayAppearAnimation();
        }

        private void PlayAppearAnimation()
        {
            transform.localScale = Vector3.one * APPEAR_SCALE_START;
            transform.DOScale(APPEAR_SCALE_END, UIConfig.PANEL_SCALE_DURATION).SetEase(Ease.OutBack);
        }
    }
}
