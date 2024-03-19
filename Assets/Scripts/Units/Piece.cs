using Assets.Scripts.GameLobby;
using Assets.Scripts.Units.PieceMovement;
using UnityEngine;

namespace Assets.Scripts.Units
{
    public class Piece : MonoBehaviour
    {
        public bool isAbleToMove = true;
        public Color teamColor;
        public Vector2Int currentPositionOnMap;
        protected MovementMap _movementMap;
        public virtual bool IsTakeable => true;

        protected virtual void Start()
        {
            if(GameLobbyManager.Singleton.isHost)
                RoundManager.Singleton.round.OnValueChanged += ResetPiece;

            SetUpMovementMap();
        }

        private void ResetPiece(int previousValue, int newValue)
            =>isAbleToMove = true;

        public virtual void OnDestroy()
        {
            if (GameLobbyManager.Singleton.isHost)
                RoundManager.Singleton.round.OnValueChanged -= ResetPiece;
        }

        protected virtual void SetUpMovementMap(){}
    }
}
