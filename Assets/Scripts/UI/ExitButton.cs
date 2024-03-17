using Assets.Scripts.GameLobby;
using UnityEngine;
namespace Assets.Scripts.UI
{
    public class ExitButton : MonoBehaviour
    {
        public void Quit()
            => GameLobbyManager.Singleton.Quit();
    }
}