using System;
using System.Collections.Generic;
using UnityEngine;

namespace SnapToMapTD.Enemies
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
    public class Enemy : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private string enemyName = "Enemy";
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

        public string EnemyName => enemyName;
        public int MaxHealth => maxHealth;
        public int CurrentHealth => currentHealth;
        public float NormalizedHealth => (float)currentHealth / maxHealth;
        public float MoveSpeed => moveSpeed;
        public int GoldReward => goldReward;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>(); // 없어도 동작
            currentHealth = maxHealth;
        }

        public void Initialize(List<Vector3> pathWaypoints)
        {
            waypoints = pathWaypoints;
            waypointIndex = 0;
            if (waypoints.Count > 0)
                transform.position = waypoints[0];

            animator?.SetBool(AnimIsWalking, true);
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

            // 5개 앞 웨이포인트 방향으로 좌우 판단 → 미세한 지그재그에 흔들리지 않음
            int lookAhead = Mathf.Min(waypointIndex + 15, waypoints.Count - 1);
            float dx = waypoints[lookAhead].x - transform.position.x;
            if (Mathf.Abs(dx) > 0.05f)
                spriteRenderer.flipX = dx < 0;

            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            if ((transform.position - target).sqrMagnitude <= 0.01f)
            {
                waypointIndex++;
                if (waypointIndex >= waypoints.Count)
                    ReachEnd();
            }
        }

        private void OnMouseDown()
        {
            if (isDead) return;
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
            SnapToMapTD.UI.EnemyInfoPanel.Instance?.Show(this);
        }

        public void TakeDamage(int amount)
        {
            if (isDead) return;

            currentHealth -= amount;
            if (currentHealth <= 0)
                Die();
            else
                animator?.SetTrigger(AnimHurt);
        }

        private void Die()
        {
            isDead = true;
            animator?.SetBool(AnimIsWalking, false);
            animator?.SetTrigger(AnimDeath);
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
