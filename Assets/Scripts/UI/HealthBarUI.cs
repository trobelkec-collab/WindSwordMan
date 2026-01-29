using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image healthFill; // Green bar
        [SerializeField] private Image damageFill; // Red bar (behind green)
        [SerializeField] private Combat.DamageableObject target;

        [Header("Settings")]
        [SerializeField] private float damageAnimDelay = 0.5f;
        [SerializeField] private float damageAnimDuration = 0.5f;
        [SerializeField] private Vector3 offset = new Vector3(0, 2.2f, 0);

        private Coroutine _damageRoutine;
        private Camera _cam;

        private void Start()
        {
            _cam = Camera.main;
            
            if (target != null)
            {
                target.OnHealthChanged += UpdateHealth;
                // Init with correct max values
                UpdateHealth(target.CurrentHealth, target.MaxHealth); 
            }
        }

        private void LateUpdate()
        {
            // Billboard effect: Face camera
            if (_cam != null)
            {
                transform.rotation = _cam.transform.rotation;
            }
        }

        private void OnDestroy()
        {
            if (target != null)
            {
                target.OnHealthChanged -= UpdateHealth;
            }
        }

        private void UpdateHealth(float current, float max)
        {
            float pct = Mathf.Clamp01(current / max);
            Debug.Log($"[HealthBarUI] Update for {target.name}: {current}/{max} = {pct*100}%");

            // 1. Instant update Green bar
            if (healthFill != null)
            {
                healthFill.fillAmount = pct;
            }

            // 2. Animate Red bar (Damage effect)
            if (damageFill != null)
            {
                // Flash effect? 
                // The red bar is BEHIND the green bar. 
                // When green shrinks instantly, the red is revealed (showing the damage taken).
                // Then we wait, and shrink the red bar to match green.
                
                if (_damageRoutine != null) StopCoroutine(_damageRoutine);
                _damageRoutine = StartCoroutine(AnimateDamageBar(pct));
            }
        }

        private IEnumerator AnimateDamageBar(float targetPct)
        {
            // 1. Flash Effect: White/Bright Red
            // Make sure damageFill is visible
            if (damageFill != null)
            {
                damageFill.color = Color.white; 
                yield return new WaitForSeconds(0.05f);
                damageFill.color = Color.red;
            }

            // 2. Hang time (Delay)
            yield return new WaitForSeconds(damageAnimDelay);

            // 3. Shrink Animation
            if (damageFill != null)
            {
                float startPct = damageFill.fillAmount;
                float elapsed = 0f;

                while (elapsed < damageAnimDuration)
                {
                    elapsed += Time.deltaTime;
                    damageFill.fillAmount = Mathf.Lerp(startPct, targetPct, elapsed / damageAnimDuration);
                    yield return null;
                }
                
                damageFill.fillAmount = targetPct; // Ensure consistent final value
            }
        }
    }
}
