using Assets.Scripts.Units.PieceMovement;
using Unity.Netcode;

namespace Assets.Scripts.Units
{
    public class Piece : NetworkBehaviour
    {
        public MovementMap movementMap;

        public virtual void SetUpMovementMap()
        {

        }
    }
}
