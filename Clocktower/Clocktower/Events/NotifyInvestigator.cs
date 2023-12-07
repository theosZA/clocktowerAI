using Clocktower.Agent;
using Clocktower.Game;

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
            foreach (var investigator in grimoire.GetLivingPlayers(Character.Investigator))
            {
                if (investigator.DrunkOrPoisoned)
                {
                    var investigatorTargetA = grimoire.Players.First(player => player != investigator && player.Alignment == Alignment.Good);
                    var investigatorTargetB = grimoire.Players.Last(player => player != investigator && player.Alignment == Alignment.Good);
                    var minionCharacter = Character.Assassin;
                    investigator.Agent.NotifyInvestigator(investigatorTargetA, investigatorTargetB, minionCharacter);
                    storyteller.NotifyInvestigator(investigator, investigatorTargetA, investigatorTargetB, minionCharacter);
                }
                else
                {
                    var recluse = grimoire.Players.FirstOrDefault(player => player.Character == Character.Recluse);
                    if (recluse != null)
                    {
                        var investigatorTargetA = recluse;
                        var investigatorTargetB = grimoire.Players.First(player => player != investigator && player.Alignment == Alignment.Good);
                        var minionCharacter = Character.Assassin;
                        investigator.Agent.NotifyInvestigator(investigatorTargetA, investigatorTargetB, minionCharacter);
                        storyteller.NotifyInvestigator(investigator, investigatorTargetA, investigatorTargetB, minionCharacter);
                    }
                    else
                    {
                        var investigatorTargetA = grimoire.Players.First(player => player.CharacterType == CharacterType.Minion);
                        var investigatorTargetB = grimoire.Players.First(player => player != investigator && player.Alignment == Alignment.Good);
                        var minionCharacter = investigatorTargetA.Character;
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
