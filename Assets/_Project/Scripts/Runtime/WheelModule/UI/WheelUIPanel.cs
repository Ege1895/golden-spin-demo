using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;
using GoldenSpin.UIModule;

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
        private Image uiImageSpinFrame;
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
        private const string ZONE_LABEL_PREFIX = "Zone ";
        private const float SPIN_BACKGROUND_ALPHA = 0.1f;

        private static readonly Color SPIN_FRAME_ACTIVE = new Color(0.165f, 0.498f, 0.831f, 1f); // #2A80D4
        private static readonly Color SPIN_FRAME_DISABLED = new Color(0.165f, 0.165f, 0.243f, 1f); // #2A2A3E
        private static readonly Color COLLECT_FRAME_DISABLED = new Color(0.145f, 0.145f, 0.145f, 1f); // #252525

        private int targetSliceIndex;

#if UNITY_EDITOR
        private const string PATH_BUTTON_SPIN = "ui_action_row/ui_button_spin";
        private const string PATH_BUTTON_COLLECT = "ui_action_row/ui_button_collect";
        private const string PATH_IMAGE_SPIN_FRAME = "ui_action_row/ui_button_spin/ui_image_frame";
        private const string PATH_IMAGE_COLLECT_FRAME = "ui_action_row/ui_button_collect/ui_image_frame";
#endif

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

            if (state == GameState.Restarting)
            {
                ShowImmediate();
            }
        }

        private void HandleZoneChanged(ZoneDTO zoneDTO)
        {
            RefreshSpinBackground(zoneDTO.SpinType);
            RefreshZoneLevelBadge(zoneDTO);
            RefreshTierColors(zoneDTO.SpinType);
            UpdateButtonStates(gameController.CurrentState);
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
            float totalRotation = WheelConfig.SPIN_MIN_ROTATIONS * WheelConfig.DEGREES_PER_ROTATION + targetAngle;
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
            uiImageSpinBackground.color = new Color(tier.r, tier.g, tier.b, SPIN_BACKGROUND_ALPHA);
        }

        private void RefreshTierColors(SpinType spinType)
        {
            Color tier = TierColors.For(spinType);

            if (uiButtonCollect != null)
            {
                ColorBlock colorBlock = uiButtonCollect.colors;
                colorBlock.normalColor = tier;
                uiButtonCollect.colors = colorBlock;
            }
        }

        private void RefreshZoneLevelBadge(ZoneDTO zone)
        {
            if (uiTextZoneCountValue != null)
            {
                uiTextZoneCountValue.text = $"{ZONE_LABEL_PREFIX}{zone.ZoneNumber}";
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
            bool canCollect = isIdle && zoneController.IsCurrentZoneSafeToLeave();

            uiButtonSpin.interactable = isIdle;
            uiButtonCollect.interactable = canCollect;

            RefreshButtonFrameStates(isIdle, canCollect, zoneController.CurrentZoneDTO.SpinType);
        }

        private void RefreshButtonFrameStates(bool spinActive, bool collectActive, SpinType spinType)
        {
            if (uiImageSpinFrame != null)
            {
                uiImageSpinFrame.color = spinActive ? SPIN_FRAME_ACTIVE : SPIN_FRAME_DISABLED;
            }

            if (uiImageCollectFrame != null)
            {
                uiImageCollectFrame.color = collectActive ? TierColors.For(spinType) : COLLECT_FRAME_DISABLED;
            }
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
            {
                uiButtonSpin = transform.Find(PATH_BUTTON_SPIN)?.GetComponent<Button>();
            }

            if (uiButtonCollect == null)
            {
                uiButtonCollect = transform.Find(PATH_BUTTON_COLLECT)?.GetComponent<Button>();
            }

            if (uiImageSpinFrame == null)
            {
                uiImageSpinFrame = transform.Find(PATH_IMAGE_SPIN_FRAME)?.GetComponent<Image>();
            }

            if (uiImageCollectFrame == null)
            {
                uiImageCollectFrame = transform.Find(PATH_IMAGE_COLLECT_FRAME)?.GetComponent<Image>();
            }
        }
#endif
    }
}
