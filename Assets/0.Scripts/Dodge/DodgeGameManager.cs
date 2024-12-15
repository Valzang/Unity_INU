﻿using System.Collections;
using _0.Scripts.Dodge.UI;
using _0.Scripts.Utility;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _0.Scripts.Dodge
{
    public class DodgeGameManager : Singleton<DodgeGameManager>
    {
        [Header("현재 총알 개수")] [SerializeField] private TMP_Text _bulletCount;
        [Header("생존 시간")] [SerializeField] private TMP_Text _aliveTime;
        [Header("게임 결과")] [SerializeField] private DodgeGameResultView _resultView;

        [Header("총알 관련 ==============================")]
        [Header("총알 시작 위치")] [SerializeField] private RectTransform[] _bulletArea;
        [Header("총알 오브젝트 풀러")] [SerializeField] private BulletPool _bulletPooler;
        [Header("시작 총알 수")] [SerializeField] [Range(10, 5000)] private int _bulletStartCount = 10;
        [Header("최대 총알 수")] [SerializeField] [Range(10, 5000)] private int _bulletMaxCount = 300;
        [Header("총알 증가하는 간격")] [SerializeField] [Range(1f,100f)] private float _addBulletCoolTime;

        public float GamePlayTime { get; private set; } = 0f;
        private bool _isStartGame = false;
        
        private bool _isGameStopped = false;
        private int _bulletCurrentCount = 0;

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        public void Initialize()
        {
            Time.timeScale = 1f;
            StopAllCoroutines();
            _isStartGame = false;
            _isGameStopped = false;
            GamePlayTime = 0f;
            _bulletCurrentCount = 0;
            
            _bulletPooler.SetLimitCount(_bulletMaxCount);
            _bulletCount.text = "0개";
            _aliveTime.text = "0초";
            Invoke(nameof(InitializeBullets), 1f);
        }

        /// <summary>
        /// 총알 초기화
        /// </summary>
        private void InitializeBullets()
        {
            if (!_bulletPooler.TryGetMultipleItem(_bulletStartCount, out var list)) return;
            foreach (var bullet in list)
            {
                Recycle(bullet);
            }
            _isStartGame = true;
            _bulletCurrentCount = list.Count;
            _bulletCount.text = $"{_bulletCurrentCount.ToString()}개";
            StartCoroutine(nameof(CreateBulletCoroutine));
        }

        

        private void Update()
        {
            if (!_isStartGame) return;
            GamePlayTime += Time.deltaTime;
            _aliveTime.text = $"{GamePlayTime:F2}초";
        }

        public void GameOver()
        {
            _isStartGame = false;
            StopAllCoroutines();
            _bulletPooler.DisposeAll();
            Time.timeScale = 0f;
            _resultView.gameObject.SetActive(true);
            _resultView.ShowGameOver(GamePlayTime);
        }

        public void Recycle(Bullet bullet)
        {
            var startPos = GetStartPos(out var startIndex);
            var endPos = GetEndPos(startIndex);
            bullet.Init(startPos, endPos, 3f);
        }

        private Vector2 GetStartPos(out int index)
        {
            index = Random.Range(0, 4);
            var targetArea = _bulletArea[index];
            var nextStartPosition = (Vector2)targetArea.position;

            if (!targetArea.TryGetComponent<BoxCollider2D>(out var boxCollider2D))
            {
                return default;
            }

            var sizeDelta = boxCollider2D.bounds.size;
            var halfWidth = sizeDelta.x * 0.5f;
            var halfHeight = sizeDelta.y * 0.5f;

            nextStartPosition.x += Random.Range(-halfWidth, halfWidth);
            nextStartPosition.y += Random.Range(-halfHeight, halfHeight);

            return nextStartPosition;
        }
        
        private Vector2 GetEndPos(int startIndex)
        {
            startIndex += 2;
            if (startIndex >= _bulletArea.Length)
            {
                startIndex -= _bulletArea.Length;
            }
            var targetArea = _bulletArea[startIndex];
            var nextEndPosition = (Vector2)(targetArea.position);
            
            if (!targetArea.TryGetComponent<BoxCollider2D>(out var boxCollider2D))
            {
                return default;
            }

            var sizeDelta = boxCollider2D.bounds.size;
            var halfWidth = sizeDelta.x * 0.5f;
            var halfHeight = sizeDelta.y * 0.5f;

            nextEndPosition.x += Random.Range(-halfWidth, halfWidth);
            nextEndPosition.y += Random.Range(-halfHeight, halfHeight);

            return nextEndPosition;
        }

        private WaitForSeconds _addCoolTime;
        private IEnumerator CreateBulletCoroutine()
        {
            _addCoolTime ??= new(_addBulletCoolTime);
            while (!_isGameStopped)
            {
                yield return _addCoolTime;
                if (!_bulletPooler.TryGetItem(out var bullet)) continue;
                Recycle(bullet);
                ++_bulletCurrentCount;
                _bulletCount.text = $"{_bulletCurrentCount.ToString()}개";
            }
        }
    }
}