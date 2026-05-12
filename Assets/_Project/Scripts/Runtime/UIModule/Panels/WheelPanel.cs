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
        private Button uiButtonLeave;
        
        [SerializeField]
        private TMP_Text uiTextZoneTypeValue;
        [SerializeField]
        private TMP_Text uiTextZoneNumberValue;
        
        [SerializeField]
        private Image uiImageSpinBackground;

        private readonly List<WheelSliceItem> activeSliceItems = new List<WheelSliceItem>();

        private int targetSliceIndex;

        private void Start()
        {
            sliceItemFactory.SetParent(slicesContainer);
            ListenEvents();
            ShowImmediate();
        }

        private void ListenEvents()
        {
            uiButtonSpin.onClick.AddListener(OnSpinButtonClicked);
            uiButtonLeave.onClick.AddListener(OnLeaveButtonClicked);

            gameController.OnStateChanged    += HandleStateChanged;
            zoneController.OnZoneChanged     += HandleZoneChanged;
            wheelController.OnWheelPrepared  += HandleWheelPrepared;
            wheelController.OnSpinStarted    += HandleSpinStarted;
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

            if (state == GameState.GameOver || state == GameState.Collecting || state == GameState.SpinResult)
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
            RefreshZoneUI(zoneDTO);
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

        private void RefreshWheelBase(SpinType spinType)
        {
            wheelBaseBronzeObject?.SetActive(spinType == SpinType.Normal);
            wheelBaseSilverObject?.SetActive(spinType == SpinType.Silver);
            wheelBaseGoldObject?.SetActive(spinType == SpinType.Golden);    
            
            wheelBaseBronzeIndicatorObject?.SetActive(spinType == SpinType.Normal);
            wheelBaseSilverIndicatorObject?.SetActive(spinType == SpinType.Silver);
            wheelBaseGoldIndicatorObject?.SetActive(spinType == SpinType.Golden);
        }

        private void RefreshZoneUI(ZoneDTO zoneDTO)
        {
            uiTextZoneNumberValue.text = zoneDTO.ZoneNumber.ToString();
            uiTextZoneTypeValue.text   = zoneDTO.Type.ToString().ToUpper();

            switch (zoneDTO.SpinType)
            {
                case SpinType.Golden:
                    uiImageSpinBackground.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.1f);
                    break;
                case SpinType.Silver:
                    uiImageSpinBackground.color = new Color(Color.gray.r, Color.gray.g, Color.gray.b, 0.1f);
                    break;
                default:
                    uiImageSpinBackground.color = new Color(Color.white.r, Color.white.g, Color.white.b, 0.1f);
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
            Debug.Log("OnSpinButtonClicked");
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

            gameController.OnStateChanged   -= HandleStateChanged;
            zoneController.OnZoneChanged    -= HandleZoneChanged;
            wheelController.OnWheelPrepared -= HandleWheelPrepared;
            wheelController.OnSpinStarted   -= HandleSpinStarted;
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
