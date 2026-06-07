using System.Collections;
using UnityEngine;
using SnapToMapTD.Enemies;

namespace SnapToMapTD.Towers
{
    public class WizardTower : Tower
    {
        [Header("Fireball (Normal Attack)")]
        [SerializeField] private GameObject fireballPrefab;
        [SerializeField] private float projectileSpeed = 4f;
        [SerializeField] private float fireballAoeRadius = 0.5f;

        [Header("Blizzard (Skill)")]
        [SerializeField] private GameObject blizzardPrefab;
        [SerializeField] private float blizzardAoeRadius = 0.8f;
        [SerializeField] private float slowMultiplier = 0.4f;
        [SerializeField] private float blizzardLifetime = 3f;

        protected override void PerformAttack(Enemy target)
        {
            if (animator.runtimeAnimatorController != null)
                animator.SetTrigger(AnimAttack);
            if (fireballPrefab != null)
                StartCoroutine(SpawnFireballAfterDelay(target, AttackHitDelay));
        }

        protected override void PerformSkill(Enemy target)
        {
            if (animator.runtimeAnimatorController != null)
                animator.SetTrigger(AnimSkill);
            if (blizzardPrefab != null)
                StartCoroutine(SpawnBlizzardAfterDelay(target, SkillHitDelay));
        }

        private IEnumerator SpawnFireballAfterDelay(Enemy target, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (target == null) yield break;
            GameObject go = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
            Projectile proj = go.GetComponent<Projectile>();
            if (proj != null) proj.Init(target, Damage, projectileSpeed, fireballAoeRadius, enemyLayer);
        }

        private IEnumerator SpawnBlizzardAfterDelay(Enemy target, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (target == null || target.IsDead) yield break;

            Vector3 spawnPos = target.transform.position;
            Vector3 toTarget = spawnPos - transform.position;
            if (toTarget.magnitude > Range)
                spawnPos = transform.position + toTarget.normalized * Range;

            GameObject go = Instantiate(blizzardPrefab, spawnPos, Quaternion.identity);
            BlizzardEffect blizzard = go.GetComponent<BlizzardEffect>();
            if (blizzard != null) blizzard.Init(SkillDamage, blizzardAoeRadius, slowMultiplier, blizzardLifetime, enemyLayer);
        }
    }
}
