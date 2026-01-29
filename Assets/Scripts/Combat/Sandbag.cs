using UnityEngine;

namespace Combat
{
    /// <summary>
    /// 전투 테스트용 샌드백입니다. 피격 시 시각적 피드백을 제공합니다.
    /// </summary>
    public class Sandbag : DamageableObject
    {
        private Renderer _renderer;
        private Color _originalColor;

        protected override void Awake()
        {
            base.Awake();
            _renderer = GetComponent<Renderer>();
            if (_renderer != null)
            {
                _originalColor = _renderer.material.color;
            }
        }

        public override void TakeDamage(float amount)
        {
            base.TakeDamage(amount);
            
            // 피격 피드백: 잠시 빨간색으로 변경
            if (_renderer != null)
            {
                StartCoroutine(FlashColor(Color.red, 0.2f));
            }
        }

        protected override void Die()
        {
            base.Die();
            Debug.Log("Sandbag Defeated! Resetting HP in 2 seconds...");
            StartCoroutine(RestoreRoutine());
        }

        private System.Collections.IEnumerator RestoreRoutine()
        {
            yield return new WaitForSeconds(2f);
            Revive();
        }

        private System.Collections.IEnumerator FlashColor(Color targetColor, float duration)
        {
            _renderer.material.color = targetColor;
            yield return new WaitForSeconds(duration);
            _renderer.material.color = _originalColor;
        }
    }
}
