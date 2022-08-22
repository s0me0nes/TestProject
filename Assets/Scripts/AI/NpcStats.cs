using UnityEngine;

namespace AI
{
    public class NpcStats : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float hp = 1;

        private Animator _animator;
        private Collider2D _collider;
        private Rigidbody2D _rigidbody;
        private EnemyBattleController _enemyController;
        
        private static readonly int Death = Animator.StringToHash("Death");

        private void Awake()
        {
            TryGetComponent(out _animator);
            TryGetComponent(out _enemyController);
            TryGetComponent(out _collider);
            TryGetComponent(out _rigidbody);
        }
        
        public void Damage(float damage)
        {
            hp -= damage;

            if (hp > 0)
                return;
            
            _animator.SetTrigger(Death);
            Destroy(_enemyController);
            Destroy(_rigidbody);
            Destroy(_collider);
            Destroy(gameObject, 10);
            
            EnemySpawner.Count--;
            
            Destroy(this);
        }
    }
}