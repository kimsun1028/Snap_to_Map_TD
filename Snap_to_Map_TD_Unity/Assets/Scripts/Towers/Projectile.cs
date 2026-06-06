using System.Collections;
using UnityEngine;
using SnapToMapTD.Enemies;

namespace SnapToMapTD.Towers
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float hitAnimDuration = 0.3f;

        private static readonly int AnimHit = Animator.StringToHash("Hit");

        private Animator animator;
        private Enemy target;
        private int damage;
        private float speed;
        private float aoeRadius;
        private LayerMask enemyLayer;
        private bool isHitting;

        public void Init(Enemy target, int damage, float speed, float aoeRadius, LayerMask enemyLayer)
        {
            this.target = target;
            this.damage = damage;
            this.speed = speed;
            this.aoeRadius = aoeRadius;
            this.enemyLayer = enemyLayer;

            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (isHitting) return;

            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (Vector2.Distance(transform.position, target.transform.position) < 0.15f)
                StartCoroutine(HitCoroutine());
        }

        private IEnumerator HitCoroutine()
        {
            isHitting = true;
            Hit();

            if (animator != null)
                animator.SetTrigger(AnimHit);

            yield return new WaitForSeconds(hitAnimDuration);
            Destroy(gameObject);
        }

        private void Hit()
        {
            if (aoeRadius > 0f)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, aoeRadius, enemyLayer);
                foreach (Collider2D hit in hits)
                {
                    Enemy e = hit.GetComponent<Enemy>();
                    if (e != null) e.TakeDamage(damage);
                }
            }
            else
            {
                if (target != null) target.TakeDamage(damage);
            }
        }
    }
}
