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
        private Image ui_image_slice_icon;
        [SerializeField]
        private TMP_Text ui_text_slice_label_value;
        [SerializeField]
        private Image ui_image_slice_background;
        [SerializeField]
        private GameObject bombIndicator;

        public void Initialize(WheelSliceDTO dto)
        {
            bool isBomb = dto.ContentType == SliceContentType.Bomb;

            bombIndicator.SetActive(isBomb);
            ui_image_slice_icon.gameObject.SetActive(!isBomb);
            ui_text_slice_label_value.gameObject.SetActive(!isBomb);

            if (!isBomb && dto.Reward != null)
            {
                ui_text_slice_label_value.text = $"x{dto.Reward.Amount}";
            }
        }

        public void SetHighlight(bool highlighted)
        {
            ui_image_slice_background.color = highlighted ? Color.yellow : Color.white;
        }

        public class Factory : PlaceholderFactory<WheelSliceDTO, WheelSliceItem> { }
    }
}
