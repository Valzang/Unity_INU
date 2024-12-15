using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _0.Scripts.Dodge.UI
{
    public class DodgeGameResultView : MonoBehaviour
    {
        [Header("다시 시작하기")] [SerializeField] private Button _restartButton;
        [Header("게임 종료하기")] [SerializeField] private Button _exitButton;
        [Header("총 플레이 시간")] [SerializeField] private TMP_Text _playTime;

        private void Awake()
        {
            Subscribe();
        }

        private void Subscribe()
        {
            _exitButton.onClick.AddListener(Exit);
            _restartButton.onClick.AddListener(Restart);
        }

        private void Restart()
        {
            gameObject.SetActive(false);
            DodgeGameManager.Instance.Initialize();
        }

        private void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void ShowGameOver(float gamePlayTime)
        {
            _playTime.text = $"{gamePlayTime:F2}초";
        }
    }
}