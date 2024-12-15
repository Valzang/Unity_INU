using _0.Scripts.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _0.Scripts.Dodge
{
    public class DodgeTitleScene : MonoBehaviour
    {
        private bool _isLoading = false;

        private AsyncOperation _sceneLoadAsync = null;
        private void Start()
        {
            _sceneLoadAsync = SceneManager.LoadSceneAsync("2.MainScene");
            _sceneLoadAsync.allowSceneActivation = false;
            SoundManager.Instance.PlayBGM("TitleBgm");
        }

        public void Update()
        {
            if (_isLoading) return;
            if (!Input.anyKeyDown) return;

            if (!(_sceneLoadAsync.progress >= 0.9f)) return;
            _isLoading = true;
            _sceneLoadAsync.allowSceneActivation = true;
        }
    }
}