using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;
using GoldenSpin.UIModule;
using UnityEngine.Serialization;

namespace GoldenSpin.WheelModule
{
    public class WheelUIPanel : BasePanel
    {
        [Inject]
        private IGameController gameController;

        [Inject]
        private IZoneController zoneController;

        [Inject]
        private IWheelController wheelController;

        [Inject]
        private ICurrencyController currencyController;

        [Inject]
        private WheelSliceItemFactory sliceItemFactory;

        [SerializeField]
        private Transform wheelTransform;
        [SerializeField]
        private Transform slicesContainer;

        [SerializeField]
        private GameObject wheelBaseBronzeObject;
        [SerializeField]
        private GameObject wheelBaseSilverObject;
        [SerializeField]
        private GameObject wheelBaseGoldObject;
        [SerializeField]
        private GameObject wheelBaseBronzeIndicatorObject;
        [SerializeField]
        private GameObject wheelBaseSilverIndicatorObject;
        [SerializeField]
        private GameObject wheelBaseGoldIndicatorObject;

        [SerializeField]
        private Button uiButtonSpin;
        [SerializeField]
        private Button uiButtonCollect;
        
        [SerializeField]
        private Image uiImageCollectFrame;
        [SerializeField]
        private Image uiImageSpinBackground;
        
        [SerializeField]
        private TMP_Text uiTextWheelTitle;
        [SerializeField]
        private TMP_Text uiTextZoneCountValue;
        [SerializeField]
        private TMP_Text uiTextGoldBadgeValue;

        private readonly List<WheelSliceItem> activeSliceItems = new List<WheelSliceItem>();

        private const string TITLE_BRONZE = "BRONZE SPIN";
        private const string TITLE_SILVER = "SILVER SPIN";
        private const string TITLE_GOLDEN = "GOLDEN SPIN";

        private int targetSliceIndex;

        private void Start()
        {
            sliceItemFactory.SetParent(slicesContainer);
            ListenEvents();
            ShowImmediate();

            RefreshZoneLevelBadge(zoneController.CurrentZoneDTO);
            RefreshGoldBadge(currencyController.CurrentAmount);

            if (wheelController.CurrentSlices.Count > 0)
            {
                BuildWheelSlices();
                RefreshWheelBase(zoneController.CurrentZoneDTO.SpinType);
            }
        }

        private void ListenEvents()
        {
            uiButtonSpin.onClick.AddListener(OnSpinButtonClicked);
            uiButtonCollect.onClick.AddListener(OnCollectButtonClicked);

            gameController.OnStateChanged += HandleStateChanged;
            zoneController.OnZoneChanged += HandleZoneChanged;
            wheelController.OnWheelPrepared += HandleWheelPrepared;
            wheelController.OnSpinStarted += HandleSpinStarted;
            currencyController.OnCurrencyChanged += RefreshGoldBadge;
        }

        protected override void OnShow()
        {
            RefreshSpinBackground(zoneController.CurrentZoneDTO.SpinType);
            UpdateButtonStates(gameController.CurrentState);
        }

        private void BuildWheelSlices()
        {
            ClearSlices();

            for (int i = 0; i < wheelController.CurrentSlices.Count; i++)
            {
                WheelSliceItem item = sliceItemFactory.Create(wheelController.CurrentSlices[i]);
                item.transform.localRotation = Quaternion.Euler(0f, 0f, i * WheelConfig.SLICE_ANGLE);
                activeSliceItems.Add(item);
            }
        }

        private void ClearSlices()
        {
            foreach (WheelSliceItem item in activeSliceItems)
            {
                sliceItemFactory.DestroyItem(item);
            }

            activeSliceItems.Clear();
        }

        private void HandleStateChanged(GameState state)
        {
            UpdateButtonStates(state);

            if (state == GameState.Collecting)
            {
                Hide();
            }
            else if (state == GameState.Restarting)
            {
                ShowImmediate();
            }
        }

        private void HandleZoneChanged(ZoneDTO zoneDTO)
        {
            RefreshSpinBackground(zoneDTO.SpinType);
            RefreshZoneLevelBadge(zoneDTO);
            RefreshTierColors(zoneDTO.SpinType);
        }

        private void HandleWheelPrepared(SpinType spinType)
        {
            wheelTransform.localRotation = Quaternion.identity;
            BuildWheelSlices();
            RefreshWheelBase(spinType);
            Show();
        }

        private void HandleSpinStarted(int resultIndex)
        {
            targetSliceIndex = resultIndex;
            StartSpinAnimation();
        }

        private void StartSpinAnimation()
        {
            float targetAngle = wheelController.GetTargetAngleForSlice(targetSliceIndex);
            float totalRotation = WheelConfig.SPIN_MIN_ROTATIONS * 360f + targetAngle;
            float duration = Random.Range(WheelConfig.SPIN_MIN_DURATION, WheelConfig.SPIN_MAX_DURATION);

            wheelTransform.DORotate(
                new Vector3(0f, 0f, -totalRotation),
                duration,
                RotateMode.FastBeyond360
            ).SetEase(Ease.OutQuart).OnComplete(() =>
            {
                WheelSliceDTO resultSlice = wheelController.CurrentSlices[targetSliceIndex];
                gameController.OnSpinAnimationCompleted(resultSlice);
            });
        }

        private void RefreshWheelBase(SpinType spinType)
        {
            wheelBaseBronzeObject?.SetActive(spinType == SpinType.Normal);
            wheelBaseSilverObject?.SetActive(spinType == SpinType.Silver);
            wheelBaseGoldObject?.SetActive(spinType == SpinType.Golden);

            wheelBaseBronzeIndicatorObject?.SetActive(spinType == SpinType.Normal);
            wheelBaseSilverIndicatorObject?.SetActive(spinType == SpinType.Silver);
            wheelBaseGoldIndicatorObject?.SetActive(spinType == SpinType.Golden);

            RefreshWheelTitle(spinType);
            RefreshSpinBackground(spinType);
            RefreshTierColors(spinType);
        }

        private void RefreshWheelTitle(SpinType spinType)
        {
            if (uiTextWheelTitle == null)
            {
                return;
            }

            uiTextWheelTitle.text = spinType switch
            {
                SpinType.Golden => TITLE_GOLDEN,
                SpinType.Silver => TITLE_SILVER,
                _ => TITLE_BRONZE,
            };

            uiTextWheelTitle.color = TierColors.For(spinType);
        }

        private void RefreshSpinBackground(SpinType spinType)
        {
            if (uiImageSpinBackground == null)
            {
                return;
            }

            Color tier = TierColors.For(spinType);
            uiImageSpinBackground.color = new Color(tier.r, tier.g, tier.b, 0.1f);
        }

        private void RefreshTierColors(SpinType spinType)
        {
            Color tier = TierColors.For(spinType);

            if (uiButtonCollect != null)
            {
                ColorBlock colorBlock = uiButtonCollect.colors;

                colorBlock.normalColor = tier;
                colorBlock.highlightedColor = colorBlock.highlightedColor;
                colorBlock.selectedColor = colorBlock.selectedColor;
                colorBlock.pressedColor = colorBlock.pressedColor;
                colorBlock.disabledColor = colorBlock.disabledColor;

                uiButtonCollect.colors = colorBlock;
            }

            if (uiImageCollectFrame != null && spinType != SpinType.Normal)
            {
                uiImageCollectFrame.color = tier;
            }
        }

        private void RefreshZoneLevelBadge(ZoneDTO zone)
        {
            if (uiTextZoneCountValue != null)
            {
                uiTextZoneCountValue.text = $"Zone {zone.ZoneNumber}";
            }
        }

        private void RefreshGoldBadge(int amount)
        {
            if (uiTextGoldBadgeValue != null)
            {
                uiTextGoldBadgeValue.text = amount.ToString();
            }
        }

        private void UpdateButtonStates(GameState state)
        {
            bool isIdle = state == GameState.Idle;
            uiButtonSpin.interactable    = isIdle;
            uiButtonCollect.interactable = isIdle && zoneController.IsCurrentZoneSafeToLeave();
            RefreshCollectButtonVisual(zoneController.CurrentZoneDTO);
        }

        private void RefreshCollectButtonVisual(ZoneDTO zone)
        {
            bool canCollect = zoneController.IsCurrentZoneSafeToLeave();
            uiButtonCollect.interactable = canCollect && gameController.CurrentState == GameState.Idle;
        }

        private void OnSpinButtonClicked()
        {
            gameController.RequestSpin();
        }

        private void OnCollectButtonClicked()
        {
            gameController.RequestCollect();
        }

        private void UnsubscribeFromEvents()
        {
            uiButtonSpin.onClick.RemoveListener(OnSpinButtonClicked);
            uiButtonCollect.onClick.RemoveListener(OnCollectButtonClicked);

            gameController.OnStateChanged -= HandleStateChanged;
            zoneController.OnZoneChanged -= HandleZoneChanged;
            wheelController.OnWheelPrepared -= HandleWheelPrepared;
            wheelController.OnSpinStarted -= HandleSpinStarted;
            currencyController.OnCurrencyChanged -= RefreshGoldBadge;
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (uiButtonSpin == null)
                uiButtonSpin = transform.Find("ui_action_row/ui_button_spin")?.GetComponent<Button>();

            if (uiButtonCollect == null)
                uiButtonCollect = transform.Find("ui_action_row/ui_button_collect")?.GetComponent<Button>();
        }
#endif
    }
}
