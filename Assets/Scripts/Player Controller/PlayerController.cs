using AI;
using UI;
using UnityEngine;

namespace Player_Controller
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance;
        
        [Header("Settings")]
        [SerializeField] private float speed = 4.0f;
        [SerializeField] private float jumpForce = 7.5f;
        [SerializeField] private float rollForce = 6.0f;
        [Space]
        [SerializeField] private bool noBlood;
        
        public Stats Stats { get; private set; }
        
        public bool IsBlocking { get; private set; }

        private EnemyBattleController _enemy;

        private Animator _animator;
        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;
    
        private Sensor _groundSensor;
        private Sensor _wallSensorRightDown;
        private Sensor _wallSensorRightUp;
        private Sensor _wallSensorLeftDown;
        private Sensor _wallSensorLeftUp;
    
        private float _timeSinceAttack;
        private float _delayToIdle;
        private float _rollCurrentTime;

        private int _facingDirection = 1;
        private int _currentAttack;
    
        private bool _isWallSliding;
        private bool _grounded;
        private bool _rolling;
    
        private static readonly int Grounded = Animator.StringToHash("Grounded");
        private static readonly int AirSpeedY = Animator.StringToHash("AirSpeedY");
        private static readonly int WallSlide = Animator.StringToHash("WallSlide");
        private static readonly int NoBlood = Animator.StringToHash("noBlood");
        private static readonly int Death = Animator.StringToHash("Death");
        private static readonly int Hurt = Animator.StringToHash("Hurt");
        private static readonly int Block = Animator.StringToHash("Block");
        private static readonly int IdleBlock = Animator.StringToHash("IdleBlock");
        private static readonly int Roll = Animator.StringToHash("Roll");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int AnimState = Animator.StringToHash("AnimState");

        private const string Attack = "Attack";
        private const float RollDuration = 8.0f / 14.0f;

        private void Awake()
        {
            Instance = this;

            Stats = GetComponent<Stats>();
            TryGetComponent(out _animator);
            TryGetComponent(out _rigidbody);
            TryGetComponent(out _spriteRenderer);

            transform.GetChild(0).TryGetComponent(out _groundSensor);
            transform.GetChild(1).TryGetComponent(out _wallSensorRightDown);
            transform.GetChild(2).TryGetComponent(out _wallSensorRightUp);
            transform.GetChild(3).TryGetComponent(out _wallSensorLeftDown);
            transform.GetChild(4).TryGetComponent(out _wallSensorLeftUp);
        }

        private void Update ()
        {
            _timeSinceAttack += Time.deltaTime;
        
            if (_rolling)
                _rollCurrentTime += Time.deltaTime;
        
            if (_rollCurrentTime > RollDuration)
                _rolling = false;
        
            if (!_grounded && _groundSensor.State())
            {
                _grounded = true;
                _animator.SetBool(Grounded, _grounded);
            }

            if (_grounded && !_groundSensor.State())
            {
                _grounded = false;
                _animator.SetBool(Grounded, _grounded);
            }

            float inputX = Input.GetAxis("Horizontal");

            if (inputX > 0)
            {
                _spriteRenderer.flipX = false;
                _facingDirection = 1;
            }
            else if (inputX < 0)
            {
                _spriteRenderer.flipX = true;
                _facingDirection = -1;
            }
        
            if (!_rolling)
                _rigidbody.velocity = new Vector2(inputX * speed, _rigidbody.velocity.y);
        
            _animator.SetFloat(AirSpeedY, _rigidbody.velocity.y);
        
            _isWallSliding = (_wallSensorRightDown.State() && _wallSensorRightUp.State()) || 
                             (_wallSensorLeftDown.State() && _wallSensorLeftUp.State());
            _animator.SetBool(WallSlide, _isWallSliding);
        
            if (Input.GetKeyDown(KeyCode.E) && !_rolling)
            {
                _animator.SetBool(NoBlood, noBlood);
                _animator.SetTrigger(Death);
            }
        
            else if (Input.GetKeyDown(KeyCode.Q) && !_rolling)
                _animator.SetTrigger(Hurt);
        
            else if (Input.GetMouseButtonDown(0) && _timeSinceAttack > 0.25f && !_rolling)
            {
                _currentAttack++;
            
                if (_currentAttack > 3)
                    _currentAttack = 1;
            
                if (_timeSinceAttack > 1.0f)
                    _currentAttack = 1;
            
                _animator.SetTrigger(Attack + _currentAttack);

                if (_enemy != null)
                    if (_enemy.Stats != null)
                    {
                        _enemy.Stats.Damage(1);
                        UIController.OnAddScore?.Invoke();
                    }

                _timeSinceAttack = 0.0f;
            }
        
            else if (Input.GetMouseButtonDown(1) && !_rolling)
            {
                IsBlocking = true;
                
                _animator.SetTrigger(Block);
                _animator.SetBool(IdleBlock, IsBlocking);
            }

            else if (Input.GetMouseButtonUp(1))
            {
                IsBlocking = false;
                _animator.SetBool(IdleBlock, IsBlocking);
            }
        
            else if (Input.GetKeyDown(KeyCode.LeftShift) && !_rolling && !_isWallSliding)
            {
                _rolling = true;
                _animator.SetTrigger(Roll);
                _rigidbody.velocity = new Vector2(_facingDirection * rollForce, _rigidbody.velocity.y);
            }
        
            else if (Input.GetKeyDown(KeyCode.Space) && _grounded && !_rolling)
            {
                _animator.SetTrigger(Jump);
                _grounded = false;
                _animator.SetBool(Grounded, _grounded);
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpForce);
                _groundSensor.Disable(0.2f);
            }
        
            else if (Mathf.Abs(inputX) > Mathf.Epsilon)
            {
                _delayToIdle = 0.05f;
                _animator.SetInteger(AnimState, 1);
            }
        
            else
            {
                _delayToIdle -= Time.deltaTime;
                if(_delayToIdle < 0)
                    _animator.SetInteger(AnimState, 0);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Enemy"))
                return;

            other.TryGetComponent(out _enemy);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Enemy"))
                return;
            
            _enemy = null;
        }
    }
}
