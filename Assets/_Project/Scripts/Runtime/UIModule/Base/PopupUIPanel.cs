using UnityEngine;
using DG.Tweening;

namespace GoldenSpin.UIModule
{
    public abstract class PopupUIPanel : BasePanel
    {
        [SerializeField]
        private Transform popupContainer;

        private const float POPUP_DURATION   = 0.35f;
        private const float POPUP_OVERSHOOT  = 1.15f;

        protected override void OnShow()
        {
            PlayPopupAnimation();
        }

        private void PlayPopupAnimation()
        {
            if (popupContainer == null) return;

            popupContainer.localScale = Vector3.zero;
            popupContainer.DOScale(Vector3.one, POPUP_DURATION)
                .SetEase(Ease.OutBack, POPUP_OVERSHOOT);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (popupContainer == null && transform.childCount > 0)
                popupContainer = transform.GetChild(0);
        }
#endif
    }
}
