using Assets.Scripts.GameLobby;
using UnityEngine;
namespace Assets.Scripts.UI
{
    public class ExitButton : MonoBehaviour
    {
        public void Quit()
            => GameManager.Singleton.Quit();
    }
}