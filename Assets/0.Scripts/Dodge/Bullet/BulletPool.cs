using _0.Scripts.Utility;

namespace _0.Scripts.Dodge
{
    public class BulletPool : ObjectPooler<Bullet>
    {
        public void SetLimitCount(int bulletMaxCount)
        {
            _limitCount = (uint)bulletMaxCount;
        }
    }
}