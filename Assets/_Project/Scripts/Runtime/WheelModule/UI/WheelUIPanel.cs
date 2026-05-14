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
        private Image uiImageCollectFrame;

        [SerializeField]
        private TMP_Text uiTextWheelTitle;
        [SerializeField]
        private TMP_Text uiTextWheelSubtitleValue;
        [SerializeField]
        private TMP_Text uiTextZoneCountValue;
        [SerializeField]
        private TMP_Text uiTextGoldBadgeValue;

        [SerializeField]
        private Image uiImageStateIcon;

        private readonly List<WheelSliceItem> activeSliceItems = new List<WheelSliceItem>();

        private const string TITLE_BRONZE = "BRONZE SPIN";
        private const string TITLE_SILVER = "SILVER SPIN";
        private const string TITLE_GOLDEN = "GOLDEN SPIN";
        private const string ZONE_LABEL_PREFIX = "Zone ";
        private const string SUBTITLE_FORMAT = "Up To x{0} Rewards";
        private const string AMOUNT_PREFIX = "x";

#if UNITY_EDITOR
        private const string PATH_BUTTON_SPIN = "ui_action_row/ui_button_spin";
        private const string PATH_BUTTON_COLLECT = "ui_action_row/ui_button_collect";
        private const string PATH_IMAGE_COLLECT_FRAME = "ui_action_row/ui_button_collect/ui_image_frame";
        private const string PATH_IMAGE_STATE_ICON = "ui_container_zone_level_badge/ui_image_state_icon";
        private const string PATH_TEXT_WHEEL_SUBTITLE = "ui_text_wheel_subtitle_value";
#endif

        private static readonly Color COLLECT_FRAME_DISABLED = new Color(0.145f, 0.145f, 0.145f, 1f);

        private int targetSliceIndex;
        private int lastTickedSliceIndex;
        private Tween spinTween;

        protected override void Awake()
        {
            base.Awake();
            ListenEvents();
        }

        private void Start()
        {
            ShowImmediate();

            RefreshZoneLevelBadge(zoneController.CurrentZoneDTO);
            RefreshGoldBadge(currencyController.CurrentAmount);
            RefreshWheelSubtitle(zoneController.CurrentZoneDTO.SpinType, zoneController.GetCurrentZoneMultiplier());

            if (wheelController.CurrentSlices.Count > 0)
            {
                BuildWheelSlices();
                RefreshWheelBase(zoneController.CurrentZoneDTO.SpinType);
            }

            UpdateButtonStates(gameController.CurrentState);
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
            UpdateButtonStates(gameController.CurrentState);
        }

        private void BuildWheelSlices()
        {
            ClearSlices();

            for (int i = 0; i < wheelController.CurrentSlices.Count; i++)
            {
                WheelSliceItem item = sliceItemFactory.Create(wheelController.CurrentSlices[i], slicesContainer);
                item.transform.localRotation = Quaternion.Euler(0f, 0f, i * WheelConfig.SLICE_ANGLE);
                activeSliceItems.Add(item);
            }
        }

        private void ClearSlices()
        {
            foreach (WheelSliceItem item in activeSliceItems)
            {
                sliceItemFactory.Destroy(item);
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
            RefreshZoneLevelBadge(zoneDTO);
            RefreshTierColors(zoneDTO.SpinType);
            RefreshWheelSubtitle(zoneDTO.SpinType, zoneController.GetCurrentZoneMultiplier());
            UpdateButtonStates(gameController.CurrentState);
        }

        private void HandleWheelPrepared(SpinType spinType)
        {
            wheelTransform.localRotation = Quaternion.identity;
            BuildWheelSlices();
            RefreshWheelBase(spinType);
            RefreshWheelSubtitle(spinType, zoneController.GetCurrentZoneMultiplier());
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

            lastTickedSliceIndex = -1;
            spinTween?.Kill();

            spinTween = wheelTransform
                .DORotate(new Vector3(0f, 0f, -totalRotation), duration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuart)
                .OnUpdate(HandleSpinTick)
                .OnComplete(HandleSpinAnimationComplete);
        }

        private void HandleSpinTick()
        {
            float currentAngle = Mathf.Abs(wheelTransform.localEulerAngles.z);
            int currentSliceIndex = Mathf.FloorToInt(currentAngle / WheelConfig.SLICE_ANGLE);

            if (currentSliceIndex == lastTickedSliceIndex)
            {
                return;
            }

            lastTickedSliceIndex = currentSliceIndex;
            PlayIndicatorRatchet();
        }

        private void PlayIndicatorRatchet()
        {
            GameObject indicator = GetActiveIndicatorObject();

            if (indicator == null)
            {
                return;
            }

            indicator.transform.DOComplete();
            indicator.transform.DOPunchRotation(
                new Vector3(0f, 0f, WheelConfig.RATCHET_PUNCH_DEGREES),
                WheelConfig.RATCHET_PUNCH_DURATION,
                WheelConfig.RATCHET_PUNCH_VIBRATO,
                WheelConfig.RATCHET_PUNCH_ELASTICITY
            );
        }

        private GameObject GetActiveIndicatorObject()
        {
            if (wheelBaseGoldIndicatorObject != null && wheelBaseGoldIndicatorObject.activeSelf)
            {
                return wheelBaseGoldIndicatorObject;
            }

            if (wheelBaseSilverIndicatorObject != null && wheelBaseSilverIndicatorObject.activeSelf)
            {
                return wheelBaseSilverIndicatorObject;
            }

            return wheelBaseBronzeIndicatorObject;
        }

        private void HandleSpinAnimationComplete()
        {
            WheelSliceDTO resultSlice = wheelController.CurrentSlices[targetSliceIndex];
            gameController.OnSpinAnimationCompleted(resultSlice);
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
            if (zone == null)
            {
                return;
            }

            if (uiTextZoneCountValue != null)
            {
                uiTextZoneCountValue.text = $"{ZONE_LABEL_PREFIX}{zone.ZoneNumber}";
            }

            if (uiImageStateIcon != null)
            {
                uiImageStateIcon.color = TierColors.For(zone.SpinType);
            }
        }

        private void RefreshGoldBadge(int amount)
        {
            if (uiTextGoldBadgeValue != null)
            {
                uiTextGoldBadgeValue.text = amount.ToString();
            }
        }

        private void RefreshWheelSubtitle(SpinType spinType, int multiplier)
        {
            if (uiTextWheelSubtitleValue == null)
            {
                return;
            }

            uiTextWheelSubtitleValue.text = string.Format(SUBTITLE_FORMAT, multiplier);
            uiTextWheelSubtitleValue.color = TierColors.For(spinType);
        }

        private void UpdateButtonStates(GameState state)
        {
            bool isIdle = state == GameState.Idle;
            bool canCollect = isIdle && zoneController.IsCurrentZoneSafeToLeave();

            uiButtonSpin.interactable = isIdle;
            uiButtonCollect.interactable = canCollect;

            RefreshButtonFrameStates(canCollect, zoneController.CurrentZoneDTO.SpinType);
        }

        private void RefreshButtonFrameStates(bool collectActive, SpinType spinType)
        {
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

            spinTween?.Kill();
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

            if (uiImageCollectFrame == null)
            {
                uiImageCollectFrame = transform.Find(PATH_IMAGE_COLLECT_FRAME)?.GetComponent<Image>();
            }

            if (uiImageStateIcon == null)
            {
                uiImageStateIcon = transform.Find(PATH_IMAGE_STATE_ICON)?.GetComponent<Image>();
            }

            if (uiTextWheelSubtitleValue == null)
            {
                uiTextWheelSubtitleValue = transform.Find(PATH_TEXT_WHEEL_SUBTITLE)?.GetComponent<TMP_Text>();
            }
        }
#endif
    }
}
