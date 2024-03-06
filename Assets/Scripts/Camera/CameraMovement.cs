using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Camera
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;
        [SerializeField] [Range(0f, 1f)] private float _rectSize;
        private Rect _screenRect;
        private Vector2 _screenCenter;
        private Vector3 _moveVector;
        protected virtual void Start()
        {
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            _screenCenter = screenSize / 2f;
            _screenRect.position = screenSize * ((1 - _rectSize)/2f);
            _screenRect.width = screenSize.x * _rectSize;
            _screenRect.height = screenSize.y * _rectSize;
        }

        public void GetMousePosition(InputAction.CallbackContext context)
        {
            var mousePosition = context.ReadValue<Vector2>();
            if (!_screenRect.Contains(mousePosition))
            {
                _moveVector = (mousePosition - _screenCenter).normalized;
                _moveVector *= _moveSpeed;
                _moveVector.z = _moveVector.y;
                _moveVector.y = 0;
            }
            else
                _moveVector = Vector3.zero;
        }

        private void Update()
        {
            transform.position += _moveVector * Time.deltaTime;
        }
    }
}
