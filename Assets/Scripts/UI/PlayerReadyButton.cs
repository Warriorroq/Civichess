using Steamworks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace Assets.Scripts.UI
{
    public class PlayerReadyButton : MonoBehaviour
    {

        public UnityEvent onReady;
        public UnityEvent onNotReady;

        [SerializeField] private bool _isReady = false;

        [SerializeField] private Color _nonReadyColor;
        [SerializeField] private Color _readyColor;
        [SerializeField] private Image _image;
        public void ChangePlayersState()
        {
            SetButtonsState(!_isReady);
            GameManager.Singleton.AskToChangeMyReadyResultStateServerRpc(SteamClient.SteamId, _isReady);
        }

        public void SetButtonsState(bool state)
        {
            _isReady = state;
            if(_isReady)
                SetColorAndRunEvent(_readyColor, onReady);
            else
                SetColorAndRunEvent(_nonReadyColor, onNotReady);
        }

        private void SetColorAndRunEvent(Color newColor, UnityEvent eventToRun)
        {
            _image.color = newColor;
            eventToRun.Invoke();
        }
    }
}
