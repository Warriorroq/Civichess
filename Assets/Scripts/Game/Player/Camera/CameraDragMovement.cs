using Assets.Scripts.Game.Units.PreparedTypes;
using Assets.Scripts.Game.Units;
using Assets.Scripts.GameLobby;
using Steamworks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Game.Player.Camera
{
    public class CameraDragMovement : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 10f;
        [SerializeField] private float _rotationSpeed = 90f;

        private Vector3 _moveVectorInertia;
        private Vector2 _lastMovementInputDelta;

        private float _rotationInertia;
        private Vector2 _lastRotationInputDelta;

        public void OnLeftButtonClick(InputAction.CallbackContext context)
        {
            var mouseNewDelta = -context.ReadValue<Vector2>();
            if (mouseNewDelta == Vector2.zero)
                return;

            var last = _lastMovementInputDelta;
            _lastMovementInputDelta = mouseNewDelta;
            if (Vector2.Dot(last, mouseNewDelta) <= 0)
            {
                _moveVectorInertia = Vector3.zero;
                return;
            }

            _moveVectorInertia += new Vector3(mouseNewDelta.x, 0, mouseNewDelta.y) * _moveSpeed;
        }

        public void OnRightButtonClick(InputAction.CallbackContext context)
        {
            var mouseNewDelta = context.ReadValue<Vector2>();
            if (mouseNewDelta == Vector2.zero)
                return;
            
            var last = _lastRotationInputDelta;
            _lastRotationInputDelta = mouseNewDelta;
            if (last.x * mouseNewDelta.x < 0)
            {
                _rotationInertia = 0;
                return;
            }

            _rotationInertia += _rotationSpeed * mouseNewDelta.x;
        }

        public void CenterOnKing(InputAction.CallbackContext context)
        {
            Team team = GameManager.Singleton.party.GetTeamByPlayerId(SteamClient.SteamId);
            var direction = transform.forward * (transform.position.y * 1.41f);
            foreach (Piece piece in team.pieces.Values)
            {
                if (piece is not King)
                    continue;
                var position = piece.transform.position;
                position.y = 0;
                transform.position = position - direction;
            }
        }

        private void FixedUpdate()
        {
            MoveByInertia();
            RotateByInertia();
        }

        private void RotateByInertia()
        {
            if (Mathf.Abs(_rotationInertia) < .3f)
                return;

            var rotationInertia = _rotationInertia * Time.fixedDeltaTime;
            var direction = transform.forward * (transform.position.y * 1.41f);
            var rotationCenter = transform.position + direction;

            direction = Quaternion.AngleAxis(rotationInertia, Vector3.up) * direction;

            transform.position = rotationCenter + -direction;
            transform.LookAt(rotationCenter);

            _rotationInertia -= rotationInertia;
        }

        private void MoveByInertia()
        {
            if(_moveVectorInertia.sqrMagnitude < 3f) 
                return;

            var directionOfMovement = _moveVectorInertia * Time.fixedDeltaTime;
            transform.position += transform.right * directionOfMovement.x * Time.fixedDeltaTime;
            var newForward = transform.forward;
            newForward.y = 0;
            transform.position += newForward.normalized * directionOfMovement.z * Time.fixedDeltaTime;

            _moveVectorInertia -= directionOfMovement;
        }
    }
}
