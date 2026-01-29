using UnityEngine;

namespace Combat
{
    /// <summary>
    /// 데미지를 받을 수 있는 모든 객체가 구현해야 하는 인터페이스입니다.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// 데미지를 입릅니다.
        /// </summary>
        /// <param name="amount">데미지 양</param>
        void TakeDamage(float amount);

        /// <summary>
        /// 현재 체력을 반환합니다.
        /// </summary>
        float CurrentHealth { get; }

        /// <summary>
        /// 객체가 사망(파괴) 상태인지 확인합니다.
        /// </summary>
        bool IsDead { get; }
    }
}
