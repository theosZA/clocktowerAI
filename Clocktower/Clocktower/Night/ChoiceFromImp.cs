using Clocktower.Agent;
using Clocktower.Game;

namespace Clocktower.Night
{
    internal class ChoiceFromImp : INightEvent
    {
        public ChoiceFromImp(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public void RunEvent(Action onEventFinished)
        {
            var imp = grimoire.GetAlivePlayer(Character.Imp);
            if (imp == null)
            {
                onEventFinished();
                return;
            }

            imp.Agent.RequestChoiceFromImp(grimoire.Players, player =>
            {
                storyteller.ChoiceFromImp(imp, player);
                if (player.Alive)
                {
                    player.Tokens.Add(Token.DiedAtNight);
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
                onEventFinished();
            });
        }

        private IStoryteller storyteller;
        private Grimoire grimoire;
    }
}
