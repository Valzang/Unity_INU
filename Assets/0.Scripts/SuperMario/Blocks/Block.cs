using System;
using UnityEngine;

namespace _0.Scripts.SuperMario.Blocks
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Block : MonoBehaviour
    {
        protected virtual void Awake()
        {
            
        }

        public virtual void ResetBlock()
        {
            Awake();
        }
    }
}