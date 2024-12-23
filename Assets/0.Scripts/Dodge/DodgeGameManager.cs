using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [Header("총알 시작 위치")] [SerializeField] private Transform[] _bulletArea;
        [Header("총알 오브젝트 풀러")] [SerializeField] private NormalBulletPool _normalBulletPooler;
        [Header("유도탄 오브젝트 풀러")] [SerializeField] private GuidedBulletPool _guidedBulletPooler;
        [Header("시작 총알 수")] [SerializeField] [Range(10, 5000)] private int _bulletStartCount = 10;
        [Header("최대 총알 수")] [SerializeField] [Range(10, 5000)] private int _bulletMaxCount = 300;
        [Header("총알 증가하는 간격")] [SerializeField] [Range(1f,100f)] private float _addBulletCoolTime;
        [Header("N번째 탄마다 유도탄 생성")] [SerializeField] [Range(0,100)] private int _guidedBulletTime;

        public float GamePlayTime { get; private set; } = 0f;
        private bool _isStartGame = false;
        
        private bool _isGameStopped = false;
        private int _bulletCurrentCount = 0;

        private float _screenHalfWidth = 0f;
        private float _screenHalfHeight = 0f;

        protected override void Awake()
        {
            base.Awake();
            Initialize();

            _screenHalfWidth = (_bulletArea.Max(x => x.position.x) - _bulletArea.Min(x => x.position.x)) * 0.5f;
            _screenHalfHeight = (_bulletArea.Max(x => x.position.y) - _bulletArea.Min(x => x.position.y)) * 0.5f;
        }

        public void Initialize()
        {
            Time.timeScale = 1f;
            SoundManager.Instance.PlayBGM("MainBgm");
            StopAllCoroutines();
            if (DodgePlayer.Instance != null)
            {
                DodgePlayer.Instance.transform.position = default;
            }
                
            _isStartGame = false;
            _isGameStopped = false;
            GamePlayTime = 0f;
            _bulletCurrentCount = 0;
            
            if(_guidedBulletTime == 0)
                _normalBulletPooler.SetLimitCount(_bulletMaxCount);
            else if(_guidedBulletTime == 1)
            {
                _guidedBulletPooler.SetLimitCount(_bulletMaxCount);
            }
            else
            {
                var guidedCount = _bulletMaxCount / _guidedBulletTime;
                _normalBulletPooler.SetLimitCount(_bulletMaxCount - guidedCount);
                _guidedBulletPooler.SetLimitCount(guidedCount);
            }
            _bulletCount.text = "0개";
            _aliveTime.text = "0초";
            Invoke(nameof(InitializeBullets), 1f);
        }

        /// <summary>
        /// 총알 초기화
        /// </summary>
        private void InitializeBullets()
        {
            List<Bullet> bullets = null;
            if(_guidedBulletTime == 1)
            {
                if (!_guidedBulletPooler.TryGetMultipleItem(_bulletStartCount, out var list)) return;
                bullets = new List<Bullet>(list.Count);
                bullets.AddRange(list);
            }
            else
            {
                if (!_normalBulletPooler.TryGetMultipleItem(_bulletStartCount, out var list)) return;
                bullets = new List<Bullet>(list.Count);
                bullets.AddRange(list);
            }
            foreach (var bullet in bullets)
            {
                Recycle(bullet);
            }
            _isStartGame = true;
            _bulletCurrentCount = bullets.Count;
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
            _normalBulletPooler.DisposeAll();
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
            
            if (index % 2 == 0)
            {
                nextStartPosition.x += Random.Range(-_screenHalfWidth, _screenHalfWidth);
            }
            else
            {
                nextStartPosition.y += Random.Range(-_screenHalfHeight, _screenHalfHeight);
            }

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
            
            if (startIndex % 2 == 0)
            {
                nextEndPosition.x += Random.Range(-_screenHalfWidth, _screenHalfWidth);
            }
            else
            {
                nextEndPosition.y += Random.Range(-_screenHalfHeight, _screenHalfHeight);
            }
            
            return nextEndPosition;
        }

        private WaitForSeconds _addCoolTime;
        private IEnumerator CreateBulletCoroutine()
        {
            _addCoolTime ??= new(_addBulletCoolTime);
            while (!_isGameStopped)
            {
                yield return _addCoolTime;

                Bullet targetBullet = null;
                ++_bulletCurrentCount;
                if (_guidedBulletTime > 0 && _bulletCurrentCount % _guidedBulletTime == 0)
                {
                    if (!_guidedBulletPooler.TryGetItem(out var bullet)) continue;
                    targetBullet = bullet;
                }
                else
                {
                    if (!_normalBulletPooler.TryGetItem(out var bullet)) continue;
                    targetBullet = bullet;
                }
                
                Recycle(targetBullet);
                _bulletCount.text = $"{_bulletCurrentCount.ToString()}개";
            }
        }
    }
}