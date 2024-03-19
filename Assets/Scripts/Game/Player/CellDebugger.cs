using Assets.Scripts.MapGenerating;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Game.Player
{
    public class CellDebugger : MonoBehaviour
    {
        [SerializeField] protected LayerMask _mask;
        [SerializeField] protected TextMeshProUGUI _cellText;
        private void Update()
        {
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, _mask))
            {
                if (hit.collider.gameObject.TryGetComponent(out Cell cell))
                    _cellText.text = cell.ToString();
                else
                    _cellText.text = string.Empty;
            }
        }
    }
}
