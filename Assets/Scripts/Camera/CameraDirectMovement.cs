using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Camera
{
    public class CameraDirectMovement : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 10;
        [SerializeField] private float _rotationSpeed = 90;
        [SerializeField] [Range(0f, 1f)] private float _rectSize = .95f;

        private bool _isBlocked;
        private int _rotationDirection;
        private Vector2 _screenCenter;
        private Vector3 _moveVector;
        private Rect _screenRect;

        protected virtual void Start()
        {
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            _screenCenter = screenSize / 2f;
            _screenRect.position = screenSize * ((1 - _rectSize)/2f);
            _screenRect.width = screenSize.x * _rectSize;
            _screenRect.height = screenSize.y * _rectSize;
        }

        public void OnLeftRotation(InputAction.CallbackContext context)
            =>_rotationDirection = context.phase == InputActionPhase.Performed ? 1 : 0;

        public void OnRightRotation(InputAction.CallbackContext context)
            => _rotationDirection = context.phase == InputActionPhase.Performed ? -1 : 0;

        public void BlockMovement(InputAction.CallbackContext context)
            => _isBlocked = context.phase == InputActionPhase.Performed;

        public void GetMousePosition(InputAction.CallbackContext context)
        {
            if (_isBlocked)
                return;

            var mousePosition = context.ReadValue<Vector2>();
            if (!_screenRect.Contains(mousePosition))
            {
                _moveVector = (mousePosition - _screenCenter).normalized * _moveSpeed;
                _moveVector.z = _moveVector.y;
                _moveVector.y = 0;
            }
            else
                _moveVector = Vector3.zero;
        }

        private void Update()
        {
            if (_isBlocked)
                return;

            RotateCamera();
            MoveAlongMouse();
        }

        private void RotateCamera()
        {
            if (_rotationDirection == 0)
                return;

            var direction = transform.forward * (transform.position.y * 1.41f);
            var rotationCenter = transform.position + direction;
            direction = Quaternion.AngleAxis(_rotationDirection * (_rotationSpeed * Time.deltaTime), Vector3.up) * direction;
            transform.position = rotationCenter + -direction;
            transform.LookAt(rotationCenter);
        }

        private void MoveAlongMouse()
        {
            if (_moveVector == Vector3.zero)
                return;

            transform.position += transform.right * _moveVector.x * Time.deltaTime;
            var newForward = transform.forward;
            newForward.y = 0;
            transform.position += newForward.normalized * _moveVector.z * Time.deltaTime;
        }
    }
}
