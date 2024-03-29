using Assets.Scripts.MapGenerating;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(TextMeshPro))]
    public class SizeInputField : MonoBehaviour
    {
        private TMP_InputField _cachedInputField;
        private void Start()
        {
            _cachedInputField = GetComponent<TMP_InputField>();
            _cachedInputField.onEndEdit.AddListener(OnEdit);
        }

        private void OnDestroy()
        {
            _cachedInputField.onEndEdit.RemoveListener(OnEdit);
        }

        public void OnEdit(string text)
        {
            try
            {
                var values = text.Split('x');
                int x = int.Parse(values[0]);
                int y = int.Parse(values[1]);
                MapManager.Singleton.map.size = new Vector2Int(x, y);
            }
            catch
            {
                ChatManager.Singleton.AddMessageToBox("[Game...] Enter correctly [size x]x[size y] (1x1)");
            }
        }
    }
}
