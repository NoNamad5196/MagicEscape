using System;
using UnityEngine;

namespace MagicEscape
{
    /// <summary>효과음 종류. AudioManager Inspector의 clips 배열에 id별로 클립을 연결한다.</summary>
    public enum SfxId
    {
        Interact,
        Pickup,
        RunePress,
        RuneWrong,
        RuneSolved,
        CauldronAccept,
        CauldronWrong,
        PotionSolved,
        SocketPlace,
        SocketReject,
        SealReveal,
        SealRelease,
        DoorOpen,
        Clear,
        UiClick
    }

    /// <summary>
    /// BGM 1곡 + 2D 효과음 재생. 클립이 비어 있어도 오류 없이 무시(그레이박스 단계 대응).
    /// </summary>
    [DisallowMultipleComponent]
    public class AudioManager : MonoBehaviour
    {
        [Serializable]
        public struct SfxEntry
        {
            public SfxId id;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume;
        }

        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioClip bgmClip;
        [SerializeField] private SfxEntry[] clips;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (sfxSource == null) Debug.LogWarning("[AudioManager] sfxSource 미지정 — 효과음이 재생되지 않습니다.", this);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        /// <summary>어디서든 null 안전하게 호출.</summary>
        public static void Play(SfxId id)
        {
            if (Instance != null) Instance.PlaySfx(id);
        }

        public static void PlayBgm()
        {
            if (Instance == null || Instance.bgmSource == null || Instance.bgmClip == null) return;
            if (Instance.bgmSource.isPlaying) return;
            Instance.bgmSource.clip = Instance.bgmClip;
            Instance.bgmSource.loop = true;
            Instance.bgmSource.Play();
        }

        public void PlaySfx(SfxId id)
        {
            if (sfxSource == null || clips == null) return;
            foreach (SfxEntry entry in clips)
            {
                if (entry.id != id || entry.clip == null) continue;
                sfxSource.PlayOneShot(entry.clip, entry.volume <= 0f ? 1f : entry.volume);
                return;
            }
        }

        /// <summary>UI Button OnClick 연결용.</summary>
        public void PlayUiClick() => PlaySfx(SfxId.UiClick);
    }
}
