using Assets.Scripts.Game.Units.PieceCommand;
using Assets.Scripts.Structures;
using System.Collections.Generic;
using Unity.Netcode;

namespace Assets.Scripts.Game.Units
{
    public class CommandManager : MonoNetworkSingleton<CommandManager>
    {
        private List<IPieceCommand> _historyOFCommands;

        protected override void Awake()
        {
            base.Awake();
            _historyOFCommands = new List<IPieceCommand>();
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
    }
}
