﻿using _0.Scripts.SuperMario.Blocks;
using _0.Scripts.Utility;
using UnityEngine;

namespace _0.Scripts.SuperMario
{
    public class SuperMarioGameManager : Singleton<SuperMarioGameManager>
    {
        [Header("첫 시작 HP")] [SerializeField] private int _marioHp = 3;
        [Header("시작 위치")] [SerializeField] private Transform _startPoint;
        [Header("마리오")] [SerializeField] private Mario _mario;
        [Header("적들")] [SerializeField] private Transform _enemyParent;
        [Header("블럭들")] [SerializeField] private Transform _blockParent;

        private Camera _mainCamera;
        private Vector3 _startCamPos;

        protected override void Awake()
        {
            base.Awake();
            _mainCamera = Camera.main;
            _startCamPos = _mainCamera.transform.position;
        }

        public void RespawnMario()
        {
            if (--_marioHp <= 0)
            {
                SoundManager.Instance.PlayEffect("SuperMario_GameOver");
                //TODO 대충 여기서 결과 보여주기?

                return;
            }
            _mainCamera.transform.position = _startCamPos;
            _mario.transform.position = _startPoint.position;

            var enemies = _enemyParent.GetComponentsInChildren<Enemy>();
            var blocks = _blockParent.GetComponentsInChildren<Block>();

            foreach (var enemy in enemies)
            {
                enemy.ResetEnemy();
            }
            
            foreach (var block in blocks)
            {
                block.ResetBlock();
            }

        }
    }
}