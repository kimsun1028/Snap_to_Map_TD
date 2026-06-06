using System.Collections;
using UnityEngine;
using SnapToMapTD.Enemies;

namespace SnapToMapTD.Towers
{
    public class PriestTower : Tower
    {
        [Header("Holy Bolt (Normal Attack)")]
        [SerializeField] private GameObject holyBoltPrefab;

        [Header("Buff (Skill)")]
        [SerializeField] private GameObject buffEffectPrefab;
        [SerializeField] private float buffRange = 1.5f;
        [SerializeField] private float attackSpeedBuff = 1.5f;
        [SerializeField] private float buffDuration = 3f;

        private static readonly int AnimBuff = Animator.StringToHash("Buff");

        private Tower buffedTower;
        private GameObject buffEffectInstance;
        private float buffTimer;

        protected override void Update()
        {
            base.Update();

            if (buffedTower != null)
            {
                buffTimer -= Time.deltaTime;
                if (buffTimer <= 0f)
                    RemoveCurrentBuff();
            }
        }

        protected override void PerformAttack(Enemy target)
        {
            if (animator.runtimeAnimatorController != null)
                animator.SetTrigger(AnimAttack);
            if (holyBoltPrefab != null)
                StartCoroutine(SpawnHolyBoltAfterDelay(target, AttackHitDelay));
        }

        protected override void PerformSkill(Enemy target)
        {
            Tower nearest = FindNearestTower();
            if (nearest == null) return;

            RemoveCurrentBuff();
            buffedTower = nearest;
            buffedTower.ApplyBuff(attackSpeedBuff);
            buffTimer = buffDuration;

            if (animator.runtimeAnimatorController != null)
                animator.SetTrigger(AnimBuff);

            if (buffEffectPrefab != null)
            {
                buffEffectInstance = Instantiate(buffEffectPrefab, buffedTower.transform.position, Quaternion.identity);
                buffEffectInstance.transform.SetParent(buffedTower.transform);
            }
        }

        private IEnumerator SpawnHolyBoltAfterDelay(Enemy target, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (target == null) yield break;
            Instantiate(holyBoltPrefab, target.transform.position, Quaternion.identity);
            StartCoroutine(DealDamageAfterDelay(target, Damage, 0f));
        }

        private void RemoveCurrentBuff()
        {
            buffedTower?.RemoveBuff();
            buffedTower = null;
            if (buffEffectInstance != null)
            {
                Destroy(buffEffectInstance);
                buffEffectInstance = null;
            }
        }

        private Tower FindNearestTower()
        {
            Tower[] allTowers = FindObjectsByType<Tower>(FindObjectsSortMode.None);
            Tower nearest = null;
            float nearestDist = float.MaxValue;

            foreach (Tower t in allTowers)
            {
                if (t == this) continue;
                float dist = (t.transform.position - transform.position).sqrMagnitude;
                if (dist <= buffRange * buffRange && dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = t;
                }
            }
            return nearest;
        }

        private void OnDestroy()
        {
            RemoveCurrentBuff();
        }
    }
}
