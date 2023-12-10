using Clocktower.Game;
using Clocktower.Options;

namespace Clocktower.Agent
{
    internal class HumanAgent : IAgent
    {
        public HumanAgent(HumanAgentForm form)
        {
            this.form = form;
        }

        public void AssignCharacter(Character character, Alignment alignment)
        {
            form.AssignCharacter(character, alignment);
        }

        public void YouAreDead()
        {
            form.YouAreDead();
        }

        public void MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions)
        {
            form.MinionInformation(demon, fellowMinions);
        }

        public void DemonInformation(IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            form.DemonInformation(minions, notInPlayCharacters);
        }

        public void NotifyGodfather(IReadOnlyCollection<Character> outsiders)
        {
            form.NotifyGodfather(outsiders);
        }

        public void NotifyLibrarian(Player playerA, Player playerB, Character character)
        {
            form.NotifyLibrarian(playerA, playerB, character);
        }

        public void NotifyInvestigator(Player playerA, Player playerB, Character character)
        {
            form.NotifyInvestigator(playerA, playerB, character);
        }

        public void NotifySteward(Player goodPlayer)
        {
            form.NotifySteward(goodPlayer);
        }

        public void NotifyShugenja(bool clockwise)
        {
            form.NotifyShugenja(clockwise);
        }

        public void NotifyEmpath(Player neighbourA, Player neighbourB, int evilCount)
        {
            form.NotifyEmpath(neighbourA, neighbourB, evilCount);
        }

        public void NotifyFortuneTeller(Player targetA, Player targetB, bool reading)
        {
            form.NotifyFortuneTeller(targetA, targetB, reading);
        }

        public void NotifyRavenkeeper(Player target, Character character)
        {
            form.NotifyRavenkeeper(target, character);
        }

        public void NotifyUndertaker(Player executedPlayer, Character character)
        {
            form.NotifyUndertaker(executedPlayer, character);
        }

        public async Task<IOption> RequestChoiceFromImp(IReadOnlyCollection<IOption> options)
        {
            return await form.RequestChoiceFromImp(options);
        }

        public async Task<IOption> RequestChoiceFromPoisoner(IReadOnlyCollection<IOption> options)
        {
            return await form.RequestChoiceFromPoisoner(options);
        }

        public async Task<IOption> RequestChoiceFromAssassin(IReadOnlyCollection<IOption> options)
        {
            return await form.RequestChoiceFromAssassin(options);
        }

        public async Task<IOption> RequestChoiceFromGodfather(IReadOnlyCollection<IOption> options)
        {
            return await form.RequestChoiceFromGodfather(options);
        }

        public async Task<IOption> RequestChoiceFromFortuneTeller(IReadOnlyCollection<IOption> options)
        {
            return await form.RequestChoiceFromFortuneTeller(options);
        }

        public async Task<IOption> RequestChoiceFromMonk(IReadOnlyCollection<IOption> options)
        {
            return await form.RequestChoiceFromMonk(options);
        }

        public async Task<IOption> RequestChoiceFromRavenkeeper(IReadOnlyCollection<IOption> options)
        {
            return await form.RequestChoiceFromRavenkeeper(options);
        }

        public async Task<IOption> PromptSlayerShot(IReadOnlyCollection<IOption> options)
        {
            return await form.PromptSlayerShot(options);
        }

        public async Task<IOption> GetNomination(IReadOnlyCollection<IOption> options)
        {
            return await form.GetNomination(options);
        }

        public async Task<IOption> GetVote(IReadOnlyCollection<IOption> options)
        {
            return await form.GetVote(options);
        }

        private HumanAgentForm form;
    }
}
