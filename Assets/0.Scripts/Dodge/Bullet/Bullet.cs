using _0.Scripts.Dodge.UI;
using UnityEngine;

namespace _0.Scripts.Dodge
{
    public class Bullet : MonoBehaviour
    {
        private Vector2 _endPosition;
        private float _speed;
        
        /// <summary>
        /// 총알 초기화해주는 함수
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="speed"></param>
        public void Init(Vector2 startPos, Vector2 endPos, float speed)
        {
            transform.position = startPos;
            _endPosition = endPos;
            _speed = speed;
            gameObject.SetActive(true);
        }

        private void FixedUpdate()
        {
            var prevPos = (Vector2)transform.position;
            if (Vector2.SqrMagnitude(_endPosition-prevPos) <= 0.01f)
            {
                gameObject.SetActive(false);
                DodgeGameManager.Instance.Recycle(this);
                return;
            }
            transform.position = Vector2.MoveTowards(prevPos, _endPosition, _speed * Time.fixedDeltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<DodgePlayer>(out var player)) return;
            player.GetDamage(1);
        }
    }
}