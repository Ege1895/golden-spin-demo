using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;
using Zenject;

namespace GoldenSpin.UIModule
{
    public class RewardListItem : MonoBehaviour
    {
        private const float APPEAR_SCALE_START = 0f;
        private const float APPEAR_SCALE_END   = 1f;

        [SerializeField] private TMP_Text ui_text_reward_label_value;
        [SerializeField] private TMP_Text ui_text_reward_amount_value;
        [SerializeField] private Image    ui_image_reward_icon;

        public void Initialize(RewardDTO dto)
        {
            ui_text_reward_label_value.text  = dto.Label;
            ui_text_reward_amount_value.text = $"x{dto.Amount}";

            PlayAppearAnimation();
        }

        private void PlayAppearAnimation()
        {
            transform.localScale = Vector3.one * APPEAR_SCALE_START;
            transform.DOScale(APPEAR_SCALE_END, UIConfig.PANEL_SCALE_DURATION).SetEase(Ease.OutBack);
        }

        public class Factory : PlaceholderFactory<RewardDTO, RewardListItem> { }
    }
}
