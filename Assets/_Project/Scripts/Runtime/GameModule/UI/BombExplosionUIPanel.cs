using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using GoldenSpin.CoreModule;
using GoldenSpin.UIModule;

namespace GoldenSpin.GameModule
{
    public class BombExplosionUIPanel : MonoBehaviour
    {
        [Inject]
        private IGameController gameController;

        [SerializeField]
        private Image uiImageScreenFlash;

        [SerializeField]
        private ParticleSystem bombParticleSystem;

        [SerializeField]
        private Camera shakeCamera;

        private static readonly Color FLASH_PEAK_COLOR = new Color(1f, 0.85f, 0.4f, 1f);
        private static readonly Color FLASH_HIDDEN_COLOR = new Color(1f, 0.85f, 0.4f, 0f);

        private const float FLASH_FADE_IN_DURATION = 0.05f;
        private const float FLASH_FADE_OUT_DURATION = 0.4f;
        private const float CAMERA_SHAKE_DURATION = 0.5f;
        private const float CAMERA_SHAKE_STRENGTH = 0.35f;
        private const int CAMERA_SHAKE_VIBRATO = 18;
        private const float CAMERA_SHAKE_RANDOMNESS = 90f;

#if UNITY_EDITOR
        private const string PATH_IMAGE_SCREEN_FLASH = "ui_image_screen_flash";
        private const string PATH_PARTICLE_SYSTEM = "ui_vfx_bomb_particles";
#endif

        private void Awake()
        {
            ListenEvents();
            HideFlash();
        }

        private void ListenEvents()
        {
            gameController.OnStateChanged += HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.GameOver)
            {
                PlayExplosionSequence();
            }
        }

        private void PlayExplosionSequence()
        {
            PlayFlash();
            PlayCameraShake();
            PlayParticles();
        }

        private void PlayFlash()
        {
            if (uiImageScreenFlash == null)
            {
                return;
            }

            uiImageScreenFlash.gameObject.SetActive(true);
            uiImageScreenFlash.color = FLASH_HIDDEN_COLOR;

            Sequence flashSequence = DOTween.Sequence();
            flashSequence.Append(TweenFlashColor(FLASH_PEAK_COLOR, FLASH_FADE_IN_DURATION));
            flashSequence.Append(TweenFlashColor(FLASH_HIDDEN_COLOR, FLASH_FADE_OUT_DURATION));
            flashSequence.OnComplete(HideFlash);
        }

        private Tween TweenFlashColor(Color target, float duration)
        {
            return DOTween.To(
                () => uiImageScreenFlash.color,
                value => uiImageScreenFlash.color = value,
                target,
                duration
            );
        }

        private void HideFlash()
        {
            if (uiImageScreenFlash == null)
            {
                return;
            }

            uiImageScreenFlash.color = FLASH_HIDDEN_COLOR;
            uiImageScreenFlash.gameObject.SetActive(false);
        }

        private void PlayCameraShake()
        {
            Camera target = shakeCamera != null ? shakeCamera : Camera.main;

            if (target == null)
            {
                return;
            }

            target.transform.DOShakePosition(
                CAMERA_SHAKE_DURATION,
                CAMERA_SHAKE_STRENGTH,
                CAMERA_SHAKE_VIBRATO,
                CAMERA_SHAKE_RANDOMNESS,
                snapping: false,
                fadeOut: true
            );
        }

        private void PlayParticles()
        {
            if (bombParticleSystem == null)
            {
                return;
            }

            bombParticleSystem.gameObject.SetActive(true);
            bombParticleSystem.Stop();
            bombParticleSystem.Clear();
            bombParticleSystem.Play();
        }

        private void UnsubscribeFromEvents()
        {
            gameController.OnStateChanged -= HandleStateChanged;
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (uiImageScreenFlash == null)
            {
                uiImageScreenFlash = transform.Find(PATH_IMAGE_SCREEN_FLASH)?.GetComponent<Image>();
            }

            if (bombParticleSystem == null)
            {
                bombParticleSystem = transform.Find(PATH_PARTICLE_SYSTEM)?.GetComponent<ParticleSystem>();
            }
        }
#endif
    }
}
