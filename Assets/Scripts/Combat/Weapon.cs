using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// 무기 스크립트입니다. 플레이어의 공격 애니메이션과 연동하여 충돌 판정을 제어합니다.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Weapon : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float damage = 10f;

        private Collider _collider;
        private bool _isAttacking = false;
        private HashSet<IDamageable> _hitTargets; // 한 번의 공격으로 중복 타격을 방지하기 위함

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true; // 무기는 트리거로 작동
            _collider.enabled = false; // 기본적으로 꺼둠
            _hitTargets = new HashSet<IDamageable>();
        }

        /// <summary>
        /// 공격 판정을 활성화합니다. (애니메이션 이벤트 등에서 호출)
        /// </summary>
        public void EnableHitbox()
        {
            _isAttacking = true;
            _collider.enabled = true;
            _hitTargets.Clear();
            Debug.Log("[Weapon] Hitbox Enabled");
        }

        /// <summary>
        /// 공격 판정을 비활성화합니다.
        /// </summary>
        public void DisableHitbox()
        {
            _isAttacking = false;
            _collider.enabled = false;
            Debug.Log("[Weapon] Hitbox Disabled");
        }

        public void Swing(float duration)
        {
            StopAllCoroutines();
            StartCoroutine(SwingRoutine(duration));
        }

        private System.Collections.IEnumerator SwingRoutine(float duration)
        {
            EnableHitbox();

            Quaternion startRot = Quaternion.Euler(0, 0, 0);
            Quaternion endRot = Quaternion.Euler(0, 90, 0); // Swing 90 degrees
            
            // Adjust based on player's forward? 
            // Assuming Weapon is child of Player/WeaponHolder. 
            // Let's do a local rotation swing.
            
            // Forward swing: Start at -45, End at +45
            startRot = Quaternion.Euler(0, -45, 0);
            endRot = Quaternion.Euler(0, 45, 0);
            
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.localRotation = Quaternion.Lerp(startRot, endRot, t);
                yield return null;
            }
            
            transform.localRotation = Quaternion.identity; // Reset
            DisableHitbox();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isAttacking) return;

            // 자기 자신(플레이어)은 무시
            if (other.CompareTag("Player")) return;

            IDamageable target = other.GetComponent<IDamageable>();
            if (target != null && !_hitTargets.Contains(target))
            {
                target.TakeDamage(damage);
                _hitTargets.Add(target);
                
                // 타격 이펙트 재생 로직 등이 이곳에 추가될 수 있습니다.
                Debug.Log($"[Weapon] Hit {other.name}!");
            }
        }
    }
}
