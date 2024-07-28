using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class AssignMarionette : IGameEvent
    {
        public AssignMarionette(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            var marionetteCandidates = GetMarionetteCandidates().ToList();
            var marionette = marionetteCandidates.Count == 1 ? marionetteCandidates[0] : await storyteller.GetMarionette(marionetteCandidates);
            marionette.Tokens.Add(Token.IsTheMarionette, marionette);
            marionette.Alignment = Alignment.Evil;
            storyteller.AssignCharacter(marionette);
        }

        public IEnumerable<Player> GetMarionetteCandidates()
        {
            for (int i = 0; i < grimoire.Players.Count; i++)
            {
                var candidate = grimoire.Players.ElementAt(i);
                if (candidate.CharacterType == CharacterType.Townsfolk || candidate.CharacterType == CharacterType.Outsider)
                {
                    var neighbourA = grimoire.Players.ElementAt((i + 1) % grimoire.Players.Count);
                    var neighbourB = grimoire.Players.ElementAt((i + grimoire.Players.Count - 1) % grimoire.Players.Count);
                    if (neighbourA.CanRegisterAsDemon || neighbourB.CanRegisterAsDemon)
                    {
                        yield return candidate;
                    }
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
