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
        private WheelSliceItemFactory sliceItemFactory;

        [SerializeField]
        private Transform wheelTransform;
        [SerializeField]
        private Transform slicesContainer;
        [SerializeField]
        private Transform indicatorTransform;

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
        private TMP_Text uiTextWheelTitle;

        [SerializeField]
        private Image uiImageSpinBackground;

        private readonly List<WheelSliceItem> activeSliceItems = new List<WheelSliceItem>();

        private const string TITLE_NORMAL = "NORMAL SPIN";
        private const string TITLE_SILVER = "SILVER SPIN";
        private const string TITLE_GOLDEN = "GOLDEN SPIN";

        private int targetSliceIndex;

        private void Start()
        {
            sliceItemFactory.SetParent(slicesContainer);
            ListenEvents();
            ShowImmediate();

            if (wheelController.CurrentSlices.Count > 0)
            {
                BuildWheelSlices();
                RefreshWheelBase(zoneController.CurrentZoneDTO.SpinType);
            }
        }

        private void ListenEvents()
        {
            uiButtonSpin.onClick.AddListener(OnSpinButtonClicked);

            gameController.OnStateChanged += HandleStateChanged;
            zoneController.OnZoneChanged += HandleZoneChanged;
            wheelController.OnWheelPrepared += HandleWheelPrepared;
            wheelController.OnSpinStarted += HandleSpinStarted;
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
                _ => TITLE_NORMAL,
            };
        }

        private void RefreshSpinBackground(SpinType spinType)
        {
            if (uiImageSpinBackground == null)
            {
                return;
            }

            uiImageSpinBackground.color = spinType switch
            {
                SpinType.Golden => new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.1f),
                SpinType.Silver => new Color(Color.gray.r, Color.gray.g, Color.gray.b, 0.1f),
                _ => new Color(Color.white.r, Color.white.g, Color.white.b, 0.1f),
            };
        }

        private void UpdateButtonStates(GameState state)
        {
            bool isIdle = state == GameState.Idle;
            uiButtonSpin.interactable = isIdle;
        }

        private void OnSpinButtonClicked()
        {
            gameController.RequestSpin();
        }

        private void UnsubscribeFromEvents()
        {
            uiButtonSpin.onClick.RemoveListener(OnSpinButtonClicked);

            gameController.OnStateChanged -= HandleStateChanged;
            zoneController.OnZoneChanged -= HandleZoneChanged;
            wheelController.OnWheelPrepared -= HandleWheelPrepared;
            wheelController.OnSpinStarted -= HandleSpinStarted;
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
                uiButtonSpin = transform.Find("ui_button_spin")?.GetComponent<Button>();
            }
        }
#endif
    }
}
