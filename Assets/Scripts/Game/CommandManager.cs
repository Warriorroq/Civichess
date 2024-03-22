using Assets.Scripts.Structures;
using System.Collections.Generic;
using Unity.Netcode;
using Assets.Scripts.Game.Commands;
namespace Assets.Scripts.Game
{
    public class CommandManager : MonoNetworkSingleton<CommandManager>
    {
        private List<ICommand> _historyOFCommands;

        protected override void Awake()
        {
            base.Awake();
            _historyOFCommands = new List<ICommand>();
        }

        [ServerRpc(RequireOwnership = false)]
        public void AskForApprovingCommandServerRpc(MovementCommand movementCommand)
        {
            if (!movementCommand.IsApproved())
                return;

            DoCommandClientRpc(movementCommand);
        }

        [ClientRpc]
        public void DoCommandClientRpc(MovementCommand movementCommand)
        {
            _historyOFCommands.Add(movementCommand);
            movementCommand.Execute();
        }

        [ServerRpc(RequireOwnership = false)]
        public void AskForApprovingCommandServerRpc(RoundChangeCommand roundChange)
        {
            if (!roundChange.IsApproved())
                return;

            DoCommandClientRpc(roundChange);
        }

        [ClientRpc]
        public void DoCommandClientRpc(RoundChangeCommand roundChange)
        {
            _historyOFCommands.Add(roundChange);
            roundChange.Execute();
        }
    }
}
