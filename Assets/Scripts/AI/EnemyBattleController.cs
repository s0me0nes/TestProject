using Player_Controller;
using UnityEngine;

namespace AI
{
    public class EnemyBattleController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float speed = 1;
        [SerializeField] private float maxYDifferenceForMove = 0.5f;
        [Header("Attack Settings")]
        [SerializeField] private float minDistanceForAttack = 2;
        [SerializeField] private float attackDelay = 1;

        public NpcStats Stats { get; private set; }
        
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        private PlayerController _player;

        private float _changeAttackTimer;
        
        private static readonly int Grounded = Animator.StringToHash("Grounded");
        private static readonly int AnimState = Animator.StringToHash("AnimState");

        private const string Attack = "Attack";

        private void Awake()
        {
            Stats = GetComponent<NpcStats>();
            
            TryGetComponent(out _animator);
            TryGetComponent(out _spriteRenderer);

            _animator.SetBool(Grounded, true);
        }

        private void Start()
        {
            _player = PlayerController.Instance;
        }

        private void Update()
        {
            if (!_player)
                return;

            if (Mathf.Abs(_player.transform.position.y - transform.position.y) > maxYDifferenceForMove)
            {
                _animator.SetInteger(AnimState, 0);
                return;
            }

            Vector3 dir = _player.transform.position - transform.position;

            _spriteRenderer.flipX = dir.x < 0;
            
            float distance = dir.sqrMagnitude;

            if (distance >= minDistanceForAttack * minDistanceForAttack)
            {
                _animator.SetInteger(AnimState, 1);

                transform.position = Vector3.Lerp(transform.position, 
                    _player.transform.position, Time.deltaTime * speed);
            }
            else
            {
                _animator.SetInteger(AnimState, 0);
                
                _changeAttackTimer += Time.deltaTime;

                if (_changeAttackTimer < attackDelay)
                    return;
                
                _animator.SetTrigger(Attack + Random.Range(1, 4));
                _changeAttackTimer = 0;

                if (!_player.IsBlocking && _player.Stats != null)
                    _player.Stats.Damage(1);
            }
        }
    }
}