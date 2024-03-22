using UnityEngine;

namespace Assets.Scripts.Game.Units
{
    [RequireComponent(typeof(Piece))]
    public class PieceDecorationChange : MonoBehaviour
    {
        [SerializeField] protected MeshRenderer _teamDisplay;
        [SerializeField] protected MeshRenderer _pieceDisplay;

        private void Start()
        {
            _teamDisplay.material = new Material(_teamDisplay.material) { color = GetComponent<Piece>().teamColor };
            _pieceDisplay.material = new Material(_pieceDisplay.material);
        }

        public void OnUsedStateChanged(bool state)
            =>_pieceDisplay.material.color = state ? Color.white : Color.black;
    }
}
