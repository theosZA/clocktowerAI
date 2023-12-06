using Clocktower.Agent;
using Clocktower.Game;
using System.Diagnostics;

namespace Clocktower.Events
{
    internal class NotifyInvestigator : IGameEvent
    {
        public NotifyInvestigator(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public Task RunEvent()
        {
            var investigator = grimoire.GetAlivePlayer(Character.Investigator);
            if (investigator != null)
            {
                // Hard-coded
                if (investigator.DrunkOrPoisoned)
                {
                    var investigatorTargetA = grimoire.GetRequiredPlayer(Character.Librarian);
                    var investigatorTargetB = grimoire.GetRequiredPlayer(Character.Slayer);
                    var minionCharacter = Character.Assassin;
                    investigator.Agent.NotifyInvestigator(investigatorTargetA, investigatorTargetB, minionCharacter);
                    storyteller.NotifyInvestigator(investigator, investigatorTargetA, investigatorTargetB, minionCharacter);
                }
                else
                {
                    var recluse = grimoire.GetPlayer(Character.Recluse);
                    if (recluse != null)
                    {
                        var investigatorTargetA = recluse;
                        var investigatorTargetB = grimoire.GetRequiredPlayer(Character.Slayer);
                        var minionCharacter = Character.Assassin;
                        investigator.Agent.NotifyInvestigator(investigatorTargetA, investigatorTargetB, minionCharacter);
                        storyteller.NotifyInvestigator(investigator, investigatorTargetA, investigatorTargetB, minionCharacter);
                    }
                    else
                    {
                        var investigatorTargetA = grimoire.GetMinions().First();
                        var investigatorTargetB = grimoire.GetRequiredPlayer(Character.Slayer);
                        Debug.Assert(investigatorTargetA.Character.HasValue);
                        var minionCharacter = investigatorTargetA.Character.Value;
                        investigator.Agent.NotifyInvestigator(investigatorTargetA, investigatorTargetB, minionCharacter);
                        storyteller.NotifyInvestigator(investigator, investigatorTargetA, investigatorTargetB, minionCharacter);
                    }
                }
            }

            return Task.CompletedTask;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
