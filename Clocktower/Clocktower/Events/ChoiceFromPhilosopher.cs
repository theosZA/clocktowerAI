using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class ChoiceFromPhilosopher : IGameEvent
    {
        public ChoiceFromPhilosopher(IStoryteller storyteller, Grimoire grimoire, IReadOnlyCollection<Character> scriptCharacters)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.scriptCharacters = scriptCharacters;
        }

        public async Task RunEvent()
        {
            foreach (var philosopher in grimoire.GetLivingPlayers(Character.Philosopher))
            {
                var character = await philosopher.Agent.RequestChoiceFromPhilosopher(scriptCharacters.Where(character => (int)character < 2000 && character != Character.Philosopher));
                if (character != null)
                {
                    ApplyPhilosopherChoice(philosopher, character.Value);
                }
            }   
        }

        private void ApplyPhilosopherChoice(Player philosopher, Character character)
        {
            if (philosopher.DrunkOrPoisoned)
            {   // The Philosopher believes they've gained their new ability, but they haven't.
                // We treat this like they're a drunk version of the character whose ability they think they've gained.
                philosopher.GainCharacterAbility(character);
                philosopher.Tokens.Add(Token.IsTheBadPhilosopher);

                storyteller.ChoiceFromPhilosopher(philosopher, philosopherDrunkedPlayer: null, character);
            }
            else
            {   // Sober and healthy Philosopher.
                philosopher.GainCharacterAbility(character);

                var philosopherDrunkedPlayer = grimoire.Players.FirstOrDefault(player => player.RealCharacter == character);
                philosopherDrunkedPlayer?.Tokens.Add(Token.PhilosopherDrunk);

                storyteller.ChoiceFromPhilosopher(philosopher, philosopherDrunkedPlayer, character);
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IReadOnlyCollection<Character> scriptCharacters;
    }
}
