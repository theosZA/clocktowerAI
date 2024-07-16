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
            foreach (var philosopher in grimoire.GetPlayersWithAbility(Character.Philosopher))
            {
                var character = await philosopher.Agent.RequestChoiceFromPhilosopher(scriptCharacters.Where(character => character.Alignment() == Alignment.Good 
                                                                                                                      && character != Character.Philosopher
                                                                                                                      && character != Character.Drunk));
                if (character != null)
                {
                    await ApplyPhilosopherChoice(philosopher, character.Value);
                }
            }   
        }

        private async Task ApplyPhilosopherChoice(Player philosopher, Character character)
        {
            if (philosopher.DrunkOrPoisoned)
            {   // The Philosopher believes they've gained their new ability, but they haven't.
                // We treat this like they're a drunk version of the character whose ability they think they've gained for as long
                // as they are drunk or poisoned.
                philosopher.GainCharacterAbility(character);
                philosopher.Tokens.Add(Token.IsTheBadPhilosopher, philosopher);

                storyteller.ChoiceFromPhilosopher(philosopher, philosopherDrunkedPlayer: null, character);
            }
            else
            {   // Sober and healthy Philosopher.
                philosopher.GainCharacterAbility(character);

                var philosopherDrunkedPlayer = grimoire.Players.FirstOrDefault(player => player.RealCharacter == character);
                philosopherDrunkedPlayer?.Tokens.Add(Token.PhilosopherDrunk, philosopher);

                storyteller.ChoiceFromPhilosopher(philosopher, philosopherDrunkedPlayer, character);

                await ApplyImmediateEffectsOfCharacter(philosopher, character);
            }
        }

        private async Task ApplyImmediateEffectsOfCharacter(Player philosopher, Character character)
        {
            philosopher.Tokens.Add(Token.PhilosopherUsedAbilityTonight, philosopher);    // Any start-knowing information will be provided later tonight as indicated by this token.
            if (character == Character.Fortune_Teller)
            {
                await new AssignFortuneTellerRedHerring(storyteller, grimoire).AssignRedHerring(fortuneTeller: philosopher);
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IReadOnlyCollection<Character> scriptCharacters;
    }
}
