using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using DG.Tweening;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;
using Zenject;

namespace GoldenSpin.UIModule
{
    public class RewardListItem : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text uiTextRewardLabelValue;
        [SerializeField]
        private TMP_Text uiTextRewardAmountValue;
        [SerializeField]
        private Image uiImageRewardIcon;

        private AsyncOperationHandle<Sprite> spriteHandle;

        private const float APPEAR_SCALE_START = 0f;
        private const float APPEAR_SCALE_END   = 1f;

        public void Initialize(RewardDTO dto)
        {
            uiTextRewardLabelValue.text  = dto.Label;
            uiTextRewardAmountValue.text = $"x{dto.Amount}";

            LoadSprite(dto.SpriteKey);
            PlayAppearAnimation();
        }

        private void LoadSprite(string spriteKey)
        {
            if (string.IsNullOrEmpty(spriteKey))
            {
                return;
            }

            spriteHandle = Addressables.LoadAssetAsync<Sprite>(spriteKey);
            spriteHandle.Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    uiImageRewardIcon.sprite = handle.Result;
                }
            };
        }

        private void PlayAppearAnimation()
        {
            transform.localScale = Vector3.one * APPEAR_SCALE_START;
            transform.DOScale(APPEAR_SCALE_END, UIConfig.PANEL_SCALE_DURATION).SetEase(Ease.OutBack);
        }

        private void OnDestroy()
        {
            if (spriteHandle.IsValid())
            {
                Addressables.Release(spriteHandle);
            }
        }

        public class Factory : PlaceholderFactory<RewardDTO, RewardListItem> { }
    }
}
