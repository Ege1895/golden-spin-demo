using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using GoldenSpin.CoreModule;

namespace GoldenSpin.UIModule
{
    public class WheelSliceItem : MonoBehaviour
    {
        [SerializeField]
        private Image uiImageSliceIcon;
        [SerializeField]
        private TMP_Text uiTextSliceLabelValue;
        [SerializeField]
        private GameObject bombIndicator;

        public void Initialize(WheelSliceDTO dto)
        {
            bool isBomb = dto.ContentType == SliceContentType.Bomb;

            bombIndicator.SetActive(isBomb);
            uiImageSliceIcon.gameObject.SetActive(!isBomb);
            uiTextSliceLabelValue.gameObject.SetActive(!isBomb);

            if (isBomb || dto.Reward == null)
            {
                return;
            }

            uiTextSliceLabelValue.text = $"x{dto.Reward.Amount}";

            if (dto.Reward.Icon != null)
            {
                SetSliceIcon(dto.Reward.Icon);
            }
        }

        private void SetSliceIcon(Sprite sprite)
        {
            uiImageSliceIcon.sprite = sprite;
        }

        public class Factory : PlaceholderFactory<WheelSliceItem> { }
    }
}
