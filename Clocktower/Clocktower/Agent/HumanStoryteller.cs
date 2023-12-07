﻿using Clocktower.Game;
using Clocktower.Options;

namespace Clocktower.Agent
{
    internal class HumanStoryteller : IStoryteller
    {
        public HumanStoryteller(StorytellerForm form)
        {
            this.form = form;
        }

        public async Task<IOption> GetDrunk(IReadOnlyCollection<IOption> drunkCandidates)
        {
            return await form.GetDrunk(drunkCandidates);
        }

        public async Task<IOption> GetStewardPing(Player steward, IReadOnlyCollection<IOption> stewardPingCandidates)
        {
            return await form.GetStewardPing(steward, stewardPingCandidates);
        }

        public async Task<IOption> GetEmpathNumber(Player empath, Player neighbourA, Player neighbourB, IReadOnlyCollection<IOption> empathOptions)
        {
            return await form.GetEmpathNumber(empath, neighbourA, neighbourB, empathOptions);
        }

        public void AssignCharacter(Player player)
        {
            form.AssignCharacter(player);
        }

        public void MinionInformation(Player minion, Player demon, IReadOnlyCollection<Player> fellowMinions)
        {
            form.MinionInformation(minion, demon, fellowMinions);
        }

        public void DemonInformation(Player demon, IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            form.DemonInformation(demon, minions, notInPlayCharacters);
        }

        public void NotifyGodfather(Player godfather, IReadOnlyCollection<Character> outsiders)
        {
            form.NotifyGodfather(godfather, outsiders);
        }

        public void NotifyLibrarian(Player librarian, Player playerA, Player playerB, Character character)
        {
            form.NotifyLibrarian(librarian, playerA, playerB, character);
        }

        public void NotifyInvestigator(Player investigator, Player playerA, Player playerB, Character character)
        {
            form.NotifyInvestigator(investigator, playerA, playerB, character);
        }

        public void NotifySteward(Player steward, Player goodPlayer)
        {
            form.NotifySteward(steward, goodPlayer);
        }

        public void NotifyEmpath(Player empath, Player neighbourA, Player neighbourB, int evilCount)
        {
            form.NotifyEmpath(empath, neighbourA, neighbourB, evilCount);
        }

        public void ChoiceFromImp(Player imp, Player target)
        {
            form.ChoiceFromImp(imp, target);
        }

        public void ChoiceFromAssassin(Player assassin, Player? target)
        {
            form.ChoiceFromAssassin(assassin, target);
        }

        public void ChoiceFromGodfather(Player godfather, Player target)
        {
            form.ChoiceFromGodfather(godfather, target);
        }

        public void ChoiceFromRavenkeeper(Player ravenkeeper, Player target, Character character)
        {
            form.ChoiceFromRavenkeeper(ravenkeeper, target, character);
        }

        private StorytellerForm form;
    }
}
