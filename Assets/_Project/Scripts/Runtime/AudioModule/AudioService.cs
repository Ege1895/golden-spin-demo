using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using DG.Tweening;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.AudioModule
{
    public class AudioService : MonoBehaviour, IAudioService, IInitializable
    {
        [Inject]
        private readonly ISaveService saveService;

        private readonly List<AudioSource> sfxSources = new List<AudioSource>();
        private readonly Dictionary<string, AudioClip> clipCache = new Dictionary<string, AudioClip>();

        private AudioSource loopSource;
        private string activeLoopKey;
        private Tween loopFadeTween;

        public bool IsSfxEnabled { get; private set; } = true;
        public bool IsMusicEnabled { get; private set; } = true;

        public void Initialize()
        {
            LoadSettings();
            BuildSourcePool();
        }

        public void PlaySfx(string addressableKey)
        {
            if (!IsSfxEnabled || string.IsNullOrEmpty(addressableKey))
            {
                return;
            }

            AudioClip clip = ResolveClip(addressableKey);

            if (clip == null)
            {
                return;
            }

            AudioSource source = GetIdleSource();
            source.volume = AudioConfig.DEFAULT_SFX_VOLUME;
            source.PlayOneShot(clip);
        }

        public void PlayLoop(string addressableKey)
        {
            if (string.IsNullOrEmpty(addressableKey))
            {
                return;
            }

            if (activeLoopKey == addressableKey && loopSource.isPlaying)
            {
                return;
            }

            AudioClip clip = ResolveClip(addressableKey);

            if (clip == null)
            {
                return;
            }

            activeLoopKey = addressableKey;
            loopSource.clip = clip;
            loopSource.loop = true;
            loopSource.volume = 0f;
            loopSource.Play();

            FadeLoopTo(IsMusicEnabled ? AudioConfig.DEFAULT_MUSIC_VOLUME : 0f);
        }

        public void StopLoop()
        {
            FadeLoopTo(0f, OnLoopFadeOutComplete);
        }

        public void SetSfxEnabled(bool enabled)
        {
            IsSfxEnabled = enabled;
            saveService.SaveInt(AudioConfig.PREFS_KEY_SFX_ENABLED, enabled ? AudioConfig.PREFS_VALUE_TRUE : AudioConfig.PREFS_VALUE_FALSE);
        }

        public void SetMusicEnabled(bool enabled)
        {
            IsMusicEnabled = enabled;
            saveService.SaveInt(AudioConfig.PREFS_KEY_MUSIC_ENABLED, enabled ? AudioConfig.PREFS_VALUE_TRUE : AudioConfig.PREFS_VALUE_FALSE);

            if (loopSource == null)
            {
                return;
            }

            FadeLoopTo(enabled ? AudioConfig.DEFAULT_MUSIC_VOLUME : 0f);
        }

        private void LoadSettings()
        {
            IsSfxEnabled = saveService.LoadInt(AudioConfig.PREFS_KEY_SFX_ENABLED, AudioConfig.PREFS_VALUE_TRUE) == AudioConfig.PREFS_VALUE_TRUE;
            IsMusicEnabled = saveService.LoadInt(AudioConfig.PREFS_KEY_MUSIC_ENABLED, AudioConfig.PREFS_VALUE_TRUE) == AudioConfig.PREFS_VALUE_TRUE;
        }

        private void BuildSourcePool()
        {
            for (int i = 0; i < AudioConfig.SFX_SOURCE_POOL_SIZE; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.loop = false;
                sfxSources.Add(source);
            }

            loopSource = gameObject.AddComponent<AudioSource>();
            loopSource.playOnAwake = false;
            loopSource.loop = true;
            loopSource.volume = 0f;
        }

        private AudioSource GetIdleSource()
        {
            foreach (AudioSource source in sfxSources)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }

            return sfxSources[0];
        }

        private AudioClip ResolveClip(string addressableKey)
        {
            if (clipCache.TryGetValue(addressableKey, out AudioClip cached))
            {
                return cached;
            }

            AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(addressableKey);
            AudioClip clip = handle.WaitForCompletion();

            if (clip != null)
            {
                clipCache[addressableKey] = clip;
            }

            return clip;
        }

        private void FadeLoopTo(float targetVolume)
        {
            FadeLoopTo(targetVolume, null);
        }

        private void FadeLoopTo(float targetVolume, TweenCallback onComplete)
        {
            if (loopSource == null)
            {
                onComplete?.Invoke();
                return;
            }

            loopFadeTween?.Kill();
            loopFadeTween = DOTween.To(
                () => loopSource.volume,
                v => loopSource.volume = v,
                targetVolume,
                AudioConfig.LOOP_FADE_DURATION
            );

            if (onComplete != null)
            {
                loopFadeTween.OnComplete(onComplete);
            }
        }

        private void OnLoopFadeOutComplete()
        {
            if (loopSource == null)
            {
                return;
            }

            loopSource.Stop();
            loopSource.clip = null;
            activeLoopKey = null;
        }

        private void OnDestroy()
        {
            loopFadeTween?.Kill();
        }
    }
}
