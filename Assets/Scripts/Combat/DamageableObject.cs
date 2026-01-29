using UnityEngine;

namespace Combat
{
    /// <summary>
    /// 체력을 가지고 데미지를 입을 수 있는 기본 객체입니다.
    /// </summary>
    public class DamageableObject : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        private float _currentHealth;
        private bool _isDead;

        public float CurrentHealth => _currentHealth;
        public bool IsDead => _isDead;

        protected virtual void Awake()
        {
            _currentHealth = maxHealth;
            _isDead = false;
        }

        public virtual void TakeDamage(float amount)
        {
            if (_isDead) return;

            _currentHealth -= amount;
            Debug.Log($"[DamageableObject] {name} took {amount} damage. Current Health: {_currentHealth}");

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            _isDead = true;
            Debug.Log($"[DamageableObject] {name} has died.");
            // 기본 동작은 없음, 상속받은 클래스에서 구현 (예: 애니메이션 재생, 오브젝트 제거 등)
        }
    }
}
