using UnityEngine;

namespace Player_Controller
{
    public class Sensor : MonoBehaviour
    {
        private int _collisionCount;

        private float _disableTimer;

        private void OnEnable()
        {
            _collisionCount = 0;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _collisionCount++;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _collisionCount--;
        }

        private void Update()
        {
            _disableTimer -= Time.deltaTime;
        }

        public void Disable(float duration)
        {
            _disableTimer = duration;
        }
    
        public bool State()
        {
            if (_disableTimer > 0)
                return false;
            return _collisionCount > 0;
        }
    }
}