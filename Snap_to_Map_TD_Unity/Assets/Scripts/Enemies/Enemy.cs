using System;
using System.Collections.Generic;
using UnityEngine;

namespace SnapToMapTD.Enemies
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Animator), typeof(Collider2D))]
    public class Enemy : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int goldReward = 10;

        public event Action<Enemy> OnDeath;
        public event Action<Enemy> OnReachEnd;

        private List<Vector3> waypoints;
        private int waypointIndex;
        private int currentHealth;
        private bool isDead;
        private SpriteRenderer spriteRenderer;
        private Animator animator;

        private static readonly int AnimIsWalking = Animator.StringToHash("IsWalking");
        private static readonly int AnimHurt = Animator.StringToHash("Hurt");
        private static readonly int AnimDeath = Animator.StringToHash("Death");

        public int MaxHealth => maxHealth;
        public int CurrentHealth => currentHealth;
        public float NormalizedHealth => (float)currentHealth / maxHealth;
        public int GoldReward => goldReward;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            currentHealth = maxHealth;
        }

        public void Initialize(List<Vector3> pathWaypoints)
        {
            waypoints = pathWaypoints;
            waypointIndex = 0;
            if (waypoints.Count > 0)
                transform.position = waypoints[0];

            animator.SetBool(AnimIsWalking, true);
        }

        private void Update()
        {
            if (isDead || waypoints == null || waypointIndex >= waypoints.Count)
                return;

            MoveAlongPath();
        }

        private void MoveAlongPath()
        {
            Vector3 target = waypoints[waypointIndex];
            Vector3 direction = target - transform.position;

            if (Mathf.Abs(direction.x) > 0.01f)
                spriteRenderer.flipX = direction.x < 0;

            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            if ((transform.position - target).sqrMagnitude <= 0.01f)
            {
                waypointIndex++;
                if (waypointIndex >= waypoints.Count)
                    ReachEnd();
            }
        }

        public void TakeDamage(int amount)
        {
            if (isDead) return;

            currentHealth -= amount;
            if (currentHealth <= 0)
                Die();
            else
                animator.SetTrigger(AnimHurt);
        }

        private void Die()
        {
            isDead = true;
            animator.SetBool(AnimIsWalking, false);
            animator.SetTrigger(AnimDeath);
            GetComponent<Collider2D>().enabled = false;
            OnDeath?.Invoke(this);
            Destroy(gameObject, 1.5f);
        }

        private void ReachEnd()
        {
            OnReachEnd?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
