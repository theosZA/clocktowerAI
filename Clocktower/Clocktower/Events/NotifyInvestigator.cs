using Clocktower.Agent;
using Clocktower.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clocktower.Events
{
    internal class NotifyInvestigator : IGameEvent
    {
        public NotifyInvestigator(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public void RunEvent(Action onEventFinished)
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

            onEventFinished();
        }

        private IStoryteller storyteller;
        private Grimoire grimoire;
    }
}
