using System.Collections.Generic;
using UnityEngine;

namespace _0.Scripts.Utility
{
    public class SoundManager : Singleton<SoundManager>
    {
        [Header("배경음 재생기")] [SerializeField] private AudioSource _bgmPlayer;
        [Header("효과음 재생기")] [SerializeField] private AudioSource _effectPlayer;

        [Header("================")] 
        [Header("배경음 목록")] [SerializeField] private List<AudioClip> _bgmList;
        [Header("효과음 목록")] [SerializeField] private List<AudioClip> _effectList;

        private Dictionary<string, AudioClip> _bgmDict;
        private Dictionary<string, AudioClip> _effectDict;

        protected override void Awake()
        {
            base.Awake();
            if (_bgmList is { Count: > 0 })
            {
                _bgmDict = new(_bgmList.Count);
                foreach (var bgmClip in _bgmList)
                {
                    _bgmDict.TryAdd(bgmClip.name, bgmClip);
                }
            }

            if (_effectList is { Count: > 0 })
            {
                _effectDict = new(_effectList.Count);
                foreach (var effectClip in _effectList)
                {
                    _effectDict.TryAdd(effectClip.name, effectClip);
                }
            }
        }

        /// <summary>
        /// 특정 파일 이름을 가진 BGM 실행
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="startTime"></param>
        public void PlayBGM(string fileName, float startTime = 0f)
        {
            if (_bgmDict is not { Count: > 0 }) return;
            if (_bgmDict.TryGetValue(fileName, out var clip))
            {
                if (_bgmPlayer.clip == clip && _bgmPlayer.isPlaying)
                {
                    _bgmPlayer.time = 0f;
                    _bgmPlayer.Play();
                    return;
                }

                _bgmPlayer.clip = clip;
                _bgmPlayer.loop = true;
                _bgmPlayer.Play();
                _bgmPlayer.time = startTime;
            }
            else
            {
                Debug.LogWarning($"{fileName}이라는 이름의 BGM이 없습니다.");
            }
        }

        public void StopBGM()
        {
            _bgmPlayer.Stop();
        }

        public float GetCurrentBgmTime()
        {
            return _bgmPlayer.clip != null ? _bgmPlayer.time : 0f;
        }

        /// <summary>
        /// 특정 파일 이름을 가진 효과음 실행
        /// </summary>
        /// <param name="fileName"></param>
        public void PlayEffect(string fileName)
        {
            if (_effectDict is not { Count: > 0 }) return;
            if (_effectDict.TryGetValue(fileName, out var clip))
            {
                _effectPlayer.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning($"{fileName}이라는 이름의 BGM이 없습니다.");
            }
        }

        public void SetMuteBgm(bool isActive)
        {
            _bgmPlayer.mute = isActive;
        }
    }
}