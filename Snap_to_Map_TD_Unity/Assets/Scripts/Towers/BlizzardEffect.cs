using System.Collections;
using UnityEngine;
using SnapToMapTD.Enemies;

namespace SnapToMapTD.Towers
{
    public class BlizzardEffect : MonoBehaviour
    {
        [SerializeField] private float despawnAnimDuration = 0.5f;

        private static readonly int AnimDespawn = Animator.StringToHash("Despawn");

        private Animator animator;
        private int damage;
        private float aoeRadius;
        private float slowMultiplier;
        private float lifetime;
        private LayerMask enemyLayer;

        private const float SlowTickInterval = 0.2f;
        private const float SlowLingerDuration = 0.35f;

        public void Init(int damage, float aoeRadius, float slowMultiplier, float lifetime, LayerMask enemyLayer)
        {
            this.damage = damage;
            this.aoeRadius = aoeRadius;
            this.slowMultiplier = slowMultiplier;
            this.lifetime = lifetime;
            this.enemyLayer = enemyLayer;

            animator = GetComponent<Animator>();
            StartCoroutine(Execute());
        }

        private IEnumerator Execute()
        {
            ApplyDamage();

            float elapsed = 0f;
            while (elapsed < lifetime)
            {
                ApplySlow();
                yield return new WaitForSeconds(SlowTickInterval);
                elapsed += SlowTickInterval;
            }

            // Despawn 애니메이션 재생 후 Destroy
            if (animator != null)
                animator.SetTrigger(AnimDespawn);
            yield return new WaitForSeconds(despawnAnimDuration);
            Destroy(gameObject);
        }

        private void ApplyDamage()
        {
            foreach (Enemy e in GetEnemiesInRange())
                e.TakeDamage(damage);
        }

        private void ApplySlow()
        {
            foreach (Enemy e in GetEnemiesInRange())
                e.ApplySlow(slowMultiplier, SlowLingerDuration);
        }

        private Enemy[] GetEnemiesInRange()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, aoeRadius, enemyLayer);
            var result = new System.Collections.Generic.List<Enemy>();
            foreach (Collider2D hit in hits)
            {
                Enemy e = hit.GetComponent<Enemy>();
                if (e != null) result.Add(e);
            }
            return result.ToArray();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, aoeRadius);
        }
    }
}
