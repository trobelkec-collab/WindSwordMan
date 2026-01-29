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
        public float MaxHealth => maxHealth;
        public bool IsDead => _isDead;

        public event System.Action<float, float> OnHealthChanged; // current, max

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

            OnHealthChanged?.Invoke(_currentHealth, maxHealth);

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            _isDead = true;
            Debug.Log($"[DamageableObject] {name} has died.");
        }

        public void Revive()
        {
            _currentHealth = maxHealth;
            _isDead = false;
            OnHealthChanged?.Invoke(_currentHealth, maxHealth);
            Debug.Log($"[DamageableObject] {name} Revived!");
        }
    }
}
