using System.Collections;
using UnityEngine;
using SnapToMapTD.Enemies;

namespace SnapToMapTD.Towers
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
    public class Tower : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private string towerName = "Tower";
        [SerializeField] private float power = 10f;
        [SerializeField] private float range = 0.5f;
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private int cost = 50;

        [Header("Damage Ratios")]
        [SerializeField] private float attackRatio = 2f;
        [SerializeField] private float skillRatio = 8f;
        [SerializeField] private float attack2Ratio = 1f;

        [Header("Skill")]
        [SerializeField] private float skillCooldown = 5f;

        [Header("Attack Timing")]
        [SerializeField] private float attackHitDelay = 0.3f;
        [SerializeField] private float skillHitDelay = 0.5f;
        [SerializeField] private float skillAnimDuration = 1.5f;

        [Header("Alternate Attack (Attack2)")]
        [SerializeField] private bool alternateAttacks = false;
        [SerializeField] private float damage2HitDelay1 = 0.2f;
        [SerializeField] private float damage2HitDelay2 = 0.5f;

        [Header("Targeting")]
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private bool aoeAttack = false;

        [Header("Upgrade")]
        [SerializeField] private int maxLevel = 2;
        [SerializeField] private int upgradeCost = 75;
        [SerializeField] private float upgradePowerBonus = 5f;
        [SerializeField] private float upgradeRangeBonus = 0.2f;

        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private float attackTimer;
        private float skillTimer;
        private bool useSecondAttack;
        private bool hadTarget;

        private static readonly int AnimAttack = Animator.StringToHash("Attack");
        private static readonly int AnimAttack2 = Animator.StringToHash("Attack2");
        private static readonly int AnimSkill = Animator.StringToHash("Skill");

        private int Damage => Mathf.RoundToInt(power * attackRatio);
        private int SkillDamage => Mathf.RoundToInt(power * skillRatio);
        private int Damage2 => Mathf.RoundToInt(power * attack2Ratio);

        public string TowerName => towerName;
        public float Power => power;
        public float Range => range;
        public int Cost => cost;
        public int Level { get; private set; } = 1;
        public bool CanUpgrade => Level < maxLevel;
        public int UpgradeCost => upgradeCost;
        public int SellPrice => cost / 2;

        public void Upgrade()
        {
            if (!CanUpgrade) return;
            Level++;
            power += upgradePowerBonus;
            range += upgradeRangeBonus;
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            attackTimer = attackCooldown;
            skillTimer = skillCooldown;
        }

        private void Update()
        {
            Enemy target = FindNearestEnemy();

            if (target == null)
            {
                hadTarget = false;
                return;
            }

            if (!hadTarget)
            {
                attackTimer = 0.05f;
                hadTarget = true;
            }

            attackTimer -= Time.deltaTime;
            skillTimer -= Time.deltaTime;

            FaceTarget(target.transform.position);

            if (skillTimer <= 0f)
            {
                if (animator.runtimeAnimatorController != null)
                    animator.SetTrigger(AnimSkill);
                StartCoroutine(SkillHitAfterDelay(skillHitDelay));
                skillTimer = skillCooldown;
                attackTimer = skillAnimDuration;
                return;
            }

            if (attackTimer <= 0f)
            {
                if (alternateAttacks && useSecondAttack)
                {
                    if (animator.runtimeAnimatorController != null)
                        animator.SetTrigger(AnimAttack2);
                    if (aoeAttack)
                    {
                        foreach (Enemy e in FindAllEnemiesInRange())
                        {
                            StartCoroutine(DealDamageAfterDelay(e, Damage2, damage2HitDelay1));
                            StartCoroutine(DealDamageAfterDelay(e, Damage2, damage2HitDelay2));
                        }
                    }
                    else
                    {
                        StartCoroutine(DealDamageAfterDelay(target, Damage2, damage2HitDelay1));
                        StartCoroutine(DealDamageAfterDelay(target, Damage2, damage2HitDelay2));
                    }
                }
                else
                {
                    if (animator.runtimeAnimatorController != null)
                        animator.SetTrigger(AnimAttack);
                    if (aoeAttack)
                    {
                        foreach (Enemy e in FindAllEnemiesInRange())
                            StartCoroutine(DealDamageAfterDelay(e, Damage, attackHitDelay));
                    }
                    else
                    {
                        StartCoroutine(DealDamageAfterDelay(target, Damage, attackHitDelay));
                    }
                }

                if (alternateAttacks)
                    useSecondAttack = !useSecondAttack;

                attackTimer = attackCooldown;
            }
        }

        private IEnumerator SkillHitAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (aoeAttack)
            {
                foreach (Enemy e in FindAllEnemiesInRange())
                    e.TakeDamage(SkillDamage);
            }
            else
            {
                Enemy hit = FindNearestEnemy();
                if (hit != null)
                    hit.TakeDamage(SkillDamage);
            }
        }

        private IEnumerator DealDamageAfterDelay(Enemy target, int dmg, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (target == null) yield break;
            if (Vector2.Distance(transform.position, target.transform.position) <= range * 1.5f)
                target.TakeDamage(dmg);
        }

        private Enemy[] FindAllEnemiesInRange()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
            var result = new System.Collections.Generic.List<Enemy>();
            foreach (Collider2D hit in hits)
            {
                Enemy e = hit.GetComponent<Enemy>();
                if (e != null) result.Add(e);
            }
            return result.ToArray();
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
