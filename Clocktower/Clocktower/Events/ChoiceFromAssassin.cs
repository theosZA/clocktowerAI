﻿using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Options;

namespace Clocktower.Events
{
    internal class ChoiceFromAssassin : IGameEvent
    {
        public ChoiceFromAssassin(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task RunEvent()
        {
            var assassin = grimoire.GetPlayer(Character.Assassin);
            if (assassin == null || assassin.Tokens.Contains(Token.UsedOncePerGameAbility))
            {
                return;
            }

            var options = grimoire.Players.Select(player => (IOption)new PlayerOption(player))
                                          .Prepend(new PassOption())
                                          .ToList();

            var choice = await assassin.Agent.RequestChoiceFromAssassin(options);
            var target = choice is PlayerOption playerOption ? playerOption.Player : null;
            storyteller.ChoiceFromAssassin(assassin, target);

            if (target != null)
            {
                assassin.Tokens.Add(Token.UsedOncePerGameAbility);
                if (!assassin.DrunkOrPoisoned)
                {
                    target.Tokens.Add(Token.DiedAtNight);
                }
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
