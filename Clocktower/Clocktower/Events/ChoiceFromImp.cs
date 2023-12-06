using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Options;
using System.Diagnostics;

namespace Clocktower.Events
{
    internal class ChoiceFromImp : IGameEvent
    {
        public ChoiceFromImp(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            var imp = grimoire.GetAlivePlayer(Character.Imp);
            if (imp == null)
            {
                return;
            }

            var options = grimoire.Players.Select(player => new PlayerOption(player)).ToList();
            var choice = await imp.Agent.RequestChoiceFromImp(options);
            var playerOption = choice as PlayerOption;
            Debug.Assert(playerOption != null);
            var player = playerOption.Player;

            storyteller.ChoiceFromImp(imp, player);
            if (player.Alive)
            {
                player.Tokens.Add(Token.KilledByDemon);
                if (player == imp)
                {   // Star-pass
                    // For now it just goes to the first alive minion.
                    var newImp = grimoire.GetMinions().FirstOrDefault(minion => minion.Alive);
                    if (newImp != null)
                    {
                        newImp.AssignCharacter(Character.Imp, Alignment.Evil);
                        storyteller.AssignCharacter(newImp);
                    }
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
