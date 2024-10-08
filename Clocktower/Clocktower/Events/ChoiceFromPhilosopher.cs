﻿using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Storyteller;
using Clocktower.Triggers;

namespace Clocktower.Events
{
    internal class ChoiceFromPhilosopher : IGameEvent
    {
        public ChoiceFromPhilosopher(IStoryteller storyteller, Grimoire grimoire, IReadOnlyCollection<Character> scriptCharacters, bool firstNight)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.scriptCharacters = scriptCharacters;
            this.firstNight = firstNight;
        }

        public async Task RunEvent()
        {
            foreach (var philosopher in grimoire.PlayersForWhomWeShouldRunAbility(Character.Philosopher))
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
                await grimoire.GainCharacterAbility(philosopher, character);
                philosopher.Tokens.Add(Token.IsTheBadPhilosopher, philosopher);

                storyteller.ChoiceFromPhilosopher(philosopher, philosopherDrunkedPlayer: null, character);
            }
            else
            {   // Sober and healthy Philosopher.
                await grimoire.GainCharacterAbility(philosopher, character);

                var philosopherDrunkedPlayer = grimoire.Players.FirstOrDefault(player => player.RealCharacter == character);
                philosopherDrunkedPlayer?.Tokens.Add(Token.PhilosopherDrunk, philosopher);

                storyteller.ChoiceFromPhilosopher(philosopher, philosopherDrunkedPlayer, character);

                await ApplyImmediateEffectsOfCharacter(philosopher, character);
            }
        }

        private async Task ApplyImmediateEffectsOfCharacter(Player philosopher, Character character)
        {
            philosopher.Tokens.Add(Token.PhilosopherUsedAbilityTonight, philosopher);    // Any start-knowing information will be provided later tonight as indicated by this token.
            switch (character)
            {
                case Character.Fortune_Teller:
                    await new AssignFortuneTellerRedHerring(storyteller, grimoire).AssignRedHerring(fortuneTeller: philosopher);
                    break;

                case Character.Cannibal:
                    if (grimoire.MostRecentlyExecutedPlayerToDie != null)
                    {
                        await new CannibalDeathTrigger(storyteller, grimoire, scriptCharacters).RunCannibal(grimoire.MostRecentlyExecutedPlayerToDie, philosopher);
                    }
                    break;

                case Character.Bounty_Hunter:
                    if (firstNight && grimoire.AnyPlayerWithRealCharacter(Character.Kazali))
                    {   // If it's still the first night and the Demon is specifically the Kazali, then the Storyteller will choose
                        // the evil townsfolk only after the Kazali Minions have been chosen.
                        break;
                    }
                    var evilTownsfolk = await storyteller.GetEvilTownsfolk(philosopher, grimoire.Players.WithCharacterType(CharacterType.Townsfolk), optional: true);
                    if (evilTownsfolk != null)
                    {
                        await evilTownsfolk.ChangeAlignment(Alignment.Evil);
                    }
                    break;
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IReadOnlyCollection<Character> scriptCharacters;
        private readonly bool firstNight;
    }
}
