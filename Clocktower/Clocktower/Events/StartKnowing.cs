﻿using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal static class StartKnowing
    {
        public static async Task Notify(Player player, Character characterAbility, IStoryteller storyteller, Grimoire grimoire, IReadOnlyCollection<Character> scriptCharacters, Random random)
        {
            switch (characterAbility)
            {
                case Character.Chef:
                    await new NotifyChef(storyteller, grimoire).RunEvent(player);
                    break;

                case Character.Steward:
                    await new NotifySteward(storyteller, grimoire).RunEvent(player);
                    break;

                case Character.Noble:
                    await new NotifyNoble(storyteller, grimoire, random).RunEvent(player);
                    break;

                case Character.Investigator:
                    await new NotifyInvestigator(storyteller, grimoire, scriptCharacters, random).RunEvent(player);
                    break;

                case Character.Librarian:
                    await new NotifyLibrarian(storyteller, grimoire, scriptCharacters, random).RunEvent(player);
                    break;

                case Character.Washerwoman:
                    await new NotifyWasherwoman(storyteller, grimoire, scriptCharacters, random).RunEvent(player);
                    break;

                case Character.Shugenja:
                    await new NotifyShugenja(storyteller, grimoire).RunEvent(player);
                    break;

                // The Ogre isn't actually a start-knowing character, but there's otherwise no spot for it on the night order (after night 1),
                // so let's run it at this spot.
                case Character.Ogre:
                    await new ChoiceFromOgre(storyteller, grimoire).RunOgre(player);
                    break;
            }
        }
    }
}
