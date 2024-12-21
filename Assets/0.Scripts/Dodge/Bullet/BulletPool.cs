using _0.Scripts.Utility;

namespace _0.Scripts.Dodge
{
    public class BulletPool : ObjectPooler<Bullet>
    {
        public void SetLimitCount(int bulletMaxCount)
        {
            _limitCount = (uint)bulletMaxCount;
        }
        
        /// <summary>
        /// 등록된 풀에 아이템을 반환합니다.
        /// </summary>
        /// <param name="item"></param>
        public override void ReleaseItem(Bullet item)
        {
            if (item == null) return;
            _activePools.Remove(item);
            item.gameObject.SetActive(false);
            _poolQueue.Enqueue(item);
        }
    }
}