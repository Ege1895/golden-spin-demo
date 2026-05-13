using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

        private const string AMOUNT_PREFIX = "x";

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

            uiTextSliceLabelValue.text = $"{AMOUNT_PREFIX}{dto.Reward.Amount}";

            if (dto.Reward.Icon != null)
            {
                SetSliceIcon(dto.Reward.Icon);
            }
        }

        private void SetSliceIcon(Sprite sprite)
        {
            uiImageSliceIcon.sprite = sprite;
        }
    }
}
