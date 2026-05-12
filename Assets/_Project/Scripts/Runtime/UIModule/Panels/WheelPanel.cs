using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.UIModule
{
    public class WheelPanel : BasePanel
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
        private Button uiButtonSpin;
        [SerializeField]
        private Button uiButtonLeave;
        
        [SerializeField]
        private TMP_Text uiTextZoneTypeValue;
        [SerializeField]
        private TMP_Text uiTextZoneNumberValue;
        
        [SerializeField]
        private Image uiImageSpinBackground;

        private readonly List<WheelSliceItem> activeSliceItems = new List<WheelSliceItem>();

        private int targetSliceIndex;

        protected override void Awake()
        {
            base.Awake();
            sliceItemFactory.SetParent(slicesContainer);
        }

        private void Start()
        {
            ListenEvents();
        }

        private void ListenEvents()
        {
            uiButtonSpin.onClick.AddListener(OnSpinButtonClicked);
            uiButtonLeave.onClick.AddListener(OnLeaveButtonClicked);

            gameController.OnStateChanged += HandleStateChanged;
            zoneController.OnZoneChanged  += HandleZoneChanged;
            wheelController.OnSpinStarted += HandleSpinStarted;
        }

        protected override void OnShow()
        {
            RefreshZoneUI(zoneController.CurrentZoneDTO);
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
        }

        private void HandleZoneChanged(ZoneDTO zoneDTO)
        {
            RefreshZoneUI(zoneDTO);
        }

        private void HandleSpinStarted(int resultIndex)
        {
            targetSliceIndex = resultIndex;
            BuildWheelSlices();
            StartSpinAnimation();
        }

        private void StartSpinAnimation()
        {
            float targetAngle   = wheelController.GetTargetAngleForSlice(targetSliceIndex);
            float totalRotation = WheelConfig.SPIN_MIN_ROTATIONS * 360f + targetAngle;
            float duration      = Random.Range(WheelConfig.SPIN_MIN_DURATION, WheelConfig.SPIN_MAX_DURATION);

            wheelTransform.DORotate(
                new Vector3(0f, 0f, -totalRotation),
                duration,
                RotateMode.FastBeyond360
            ).SetEase(Ease.OutQuart).OnComplete(() =>
            {
                WheelSliceDTO resultSlice = wheelController.CurrentSlices[targetSliceIndex];
                activeSliceItems[targetSliceIndex].SetHighlight(true);
                gameController.OnSpinAnimationCompleted(resultSlice);
            });
        }

        private void RefreshZoneUI(ZoneDTO zoneDTO)
        {
            uiTextZoneNumberValue.text = zoneDTO.ZoneNumber.ToString();
            uiTextZoneTypeValue.text   = zoneDTO.Type.ToString().ToUpper();

            switch (zoneDTO.SpinType)
            {
                case SpinType.Golden:
                    uiImageSpinBackground.color = Color.yellow;
                    break;
                case SpinType.Silver:
                    uiImageSpinBackground.color = Color.gray;
                    break;
                default:
                    uiImageSpinBackground.color = Color.white;
                    break;
            }
        }

        private void UpdateButtonStates(GameState state)
        {
            bool isIdle   = state == GameState.Idle;
            bool canLeave = isIdle && zoneController.IsCurrentZoneSafeToLeave();

            uiButtonSpin.interactable  = isIdle;
            uiButtonLeave.interactable = canLeave;
            uiButtonLeave.gameObject.SetActive(canLeave);
        }

        private void OnSpinButtonClicked()
        {
            gameController.RequestSpin();
        }

        private void OnLeaveButtonClicked()
        {
            gameController.RequestCollect();
        }

        private void UnsubscribeFromEvents()
        {
            uiButtonSpin.onClick.RemoveListener(OnSpinButtonClicked);
            uiButtonLeave.onClick.RemoveListener(OnLeaveButtonClicked);

            gameController.OnStateChanged -= HandleStateChanged;
            zoneController.OnZoneChanged  -= HandleZoneChanged;
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

            if (uiButtonLeave == null)
            {
                uiButtonLeave = transform.Find("ui_button_leave")?.GetComponent<Button>();
            }
        }
#endif
    }
}
