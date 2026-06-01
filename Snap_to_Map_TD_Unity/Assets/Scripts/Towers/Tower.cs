using UnityEngine;
using SnapToMapTD.Enemies;

namespace SnapToMapTD.Towers
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
    public class Tower : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float range = 0.5f;
        [SerializeField] private int damage = 20;
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private int cost = 50;

        [Header("Targeting")]
        [SerializeField] private LayerMask enemyLayer;

        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private float attackTimer;

        private static readonly int AnimAttack = Animator.StringToHash("Attack");

        public int Cost => cost;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer > 0f) return;

            Enemy target = FindNearestEnemy();
            if (target == null) return;

            FaceTarget(target.transform.position);
            Attack(target);
            attackTimer = attackCooldown;
        }

        private Enemy FindNearestEnemy()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
            Debug.Log($"[Tower] 탐색 hits: {hits.Length}, layer: {enemyLayer.value}, range: {range}");
            Enemy nearest = null;
            float nearestDist = float.MaxValue;

            foreach (Collider2D hit in hits)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy == null) continue;

                float dist = (enemy.transform.position - transform.position).sqrMagnitude;
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = enemy;
                }
            }
            return nearest;
        }

        private void FaceTarget(Vector3 targetPosition)
        {
            spriteRenderer.flipX = targetPosition.x < transform.position.x;
        }

        private void Attack(Enemy target)
        {
            if (animator.runtimeAnimatorController != null)
                animator.SetTrigger(AnimAttack);
            target.TakeDamage(damage);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}
