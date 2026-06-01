using System.Collections;
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

        [Header("Skill")]
        [SerializeField] private int skillDamage = 80;
        [SerializeField] private float skillCooldown = 5f;

        [Header("Attack Timing")]
        [SerializeField] private float attackHitDelay = 0.3f;
        [SerializeField] private float skillHitDelay = 0.5f;
        [SerializeField] private float skillAnimDuration = 1.5f;

        [Header("Targeting")]
        [SerializeField] private LayerMask enemyLayer;

        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private float attackTimer;
        private float skillTimer;

        private static readonly int AnimAttack = Animator.StringToHash("Attack");
        private static readonly int AnimSkill = Animator.StringToHash("Skill");

        public int Cost => cost;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            skillTimer = skillCooldown;
        }

        private void Update()
        {
            attackTimer -= Time.deltaTime;
            skillTimer -= Time.deltaTime;

            Enemy target = FindNearestEnemy();
            if (target == null) return;

            FaceTarget(target.transform.position);

            if (skillTimer <= 0f)
            {
                if (animator.runtimeAnimatorController != null)
                    animator.SetTrigger(AnimSkill);
                StartCoroutine(DealDamageAfterDelay(target, skillDamage, skillHitDelay));
                skillTimer = skillCooldown;
                attackTimer = skillAnimDuration;
                return;
            }

            if (attackTimer <= 0f)
            {
                if (animator.runtimeAnimatorController != null)
                    animator.SetTrigger(AnimAttack);
                StartCoroutine(DealDamageAfterDelay(target, damage, attackHitDelay));
                attackTimer = attackCooldown;
            }
        }

        private IEnumerator DealDamageAfterDelay(Enemy target, int dmg, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (target != null)
                target.TakeDamage(dmg);
        }

        private Enemy FindNearestEnemy()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
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

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}
