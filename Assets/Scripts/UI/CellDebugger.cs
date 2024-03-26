using Assets.Scripts.MapGenerating;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class CellDebugger : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI _cellText;

        public void DebugCell(Cell cell)
            =>_cellText.text = cell is null ? string.Empty : cell.ToString();
    }
}
