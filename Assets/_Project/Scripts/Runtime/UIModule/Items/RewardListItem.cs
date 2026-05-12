using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.UIModule
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

        public void Initialize(RewardDTO dto)
        {
            uiTextRewardLabelValue.text = $"{dto.Label}\n<color=#FFD900>x{dto.Amount}</color>";

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
