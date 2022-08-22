using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Player_Controller
{
    public class Stats : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image healthBar;
        [Header("Settings")]
        [SerializeField] private float hp = 20;

        private Animator _animator;
        private PlayerController _playerController;
        
        private float _smoothHp;
        
        private static readonly int Death = Animator.StringToHash("Death");

        private void Awake()
        {
            TryGetComponent(out _animator);
            TryGetComponent(out _playerController);
        }
        
        private void Update()
        {
            _smoothHp = Mathf.Lerp(_smoothHp, hp, Time.deltaTime / 0.1f);
            healthBar.fillAmount = Mathf.InverseLerp(0, 20, _smoothHp);
        }
        
        public void Damage(float damage)
        {
            hp -= damage;

            if (hp > 0)
                return;
            
            UIController.OnDeath?.Invoke();
            _animator.SetTrigger(Death);
            Destroy(_playerController);
            Destroy(this);
        }
    }
}