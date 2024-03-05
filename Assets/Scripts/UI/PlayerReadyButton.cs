using Steamworks;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts.UI
{
    public class PlayerReadyButton : MonoBehaviour
    {

        [SerializeField] private bool _isReady = false;

        [SerializeField] private Color _nonReadyColor;
        [SerializeField] private Color _readyColor;
        [SerializeField] private Image _image;
        public void ChangePlayersState()
        {
            _isReady = !_isReady;
            _image.color = _isReady ? _readyColor : _nonReadyColor;
            GameManager.Singleton.AskToChangeMyReadyResultStateServerRpc(SteamClient.SteamId, _isReady);
        }
    }
}
