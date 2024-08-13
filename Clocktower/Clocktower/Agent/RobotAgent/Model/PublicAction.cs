using Clocktower.Game;
using Clocktower.Options;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Clocktower.Agent.RobotAgent.Model
{
    /// <summary>
    /// Robot agent model for specifying whether or not to play or bluff an action in public during the day.
    /// </summary>
    internal class PublicAction : IOptionSelection
    {
        [Required(AllowEmptyStrings = true)]
        public string Reasoning { get; set; } = string.Empty;

        [Required]
        public bool TakeAction { get; set; }

        [Required]
        public bool AlwaysPassInFuture { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Claim { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = true)]
        public string Target { get; set; } = string.Empty;

        public IOption? PickOption(IReadOnlyCollection<IOption> options)
        {
            var option = PickOptionWithoutTarget(options);
            if (option != null)
            {
                SetOptionTarget(option);
            }
            return option;
        }

        public string NoMatchingOptionPrompt(IReadOnlyCollection<IOption> options)
        {
            var option = PickOptionFromClaim(options);
            if (option == null)
            {
                var sb = new StringBuilder();
                sb.Append($"\"{Claim}\" is not a valid claim. `{nameof(Character)}` property must be one of ");
                var possibleClaims = options.Select(OptionToExpectedClaim)
                                            .Where(claim => claim != null);
                sb.Append(string.Join(", ", possibleClaims));
                sb.Append(" or set `\"TakeAction\"` to `false` to pass instead.");
                return sb.ToString();
            }
            else
            {
                return OptionToErrorPrompt(option);
            }
        }

        private IOption? PickOptionWithoutTarget(IReadOnlyCollection<IOption> options)
        {
            if (TakeAction)
            {
                return PickOptionFromClaim(options);
            }

            if (AlwaysPassInFuture)
            {
                var alwaysPass = options.FirstOrDefault(option => option is AlwaysPassOption);
                if (alwaysPass != null)
                {
                    return alwaysPass;
                }
            }

            return options.First(option => option is PassOption);
        }

        private IOption? PickOptionFromClaim(IReadOnlyCollection<IOption> options)
        {
            return options.FirstOrDefault(OptionMatchesClaim);
        }

        private void SetOptionTarget(IOption option)
        {
            switch (option)
            {
                case JugglerOption jugglerOption:
                    jugglerOption.AddJugglesFromText(Target);
                    break;

                case SlayerShotOption slayerOption:
                    slayerOption.SetTargetFromText(Target);
                    break;
            }
        }

        private bool OptionMatchesClaim(IOption option)
        {
            var expectedClaim = OptionToExpectedClaim(option);
            if (expectedClaim == null)
            {
                return false;
            }
            return string.Equals(Claim, expectedClaim, StringComparison.InvariantCultureIgnoreCase);
        }

        private static string? OptionToExpectedClaim(IOption option)
        {
            return option switch
            {
                JugglerOption _ => TextUtilities.CharacterToText(Character.Juggler),
                SlayerShotOption _ => TextUtilities.CharacterToText(Character.Slayer),
                _ => null,
            };
        }

        private string OptionToErrorPrompt(IOption option)
        {
            var sb = new StringBuilder();

            switch (option)
            {
                case JugglerOption _:
                    sb.Append($"\"{Target}\" is not a valid juggle. ");
                    sb.AppendFormattedText("If you wish to claim %c, make sure to provide \"Target\"=\"PLAYER_NAME AS CHARACTER, PLAYER_NAME AS CHARACTER, ...\"` with up to 5 player-character pairs.",
                                                 Character.Juggler);
                    break;

                case SlayerShotOption _:
                    sb.Append($"\"{Target}\" is not a valid Slayer-shot target. ");
                    sb.AppendFormattedText("If you wish to claim %c, make sure to provide \"Target\"=\"PLAYER_NAME\"` with the name of the player you wish to target.", Character.Slayer);
                    break;
            }

            return sb.ToString();
        }
    }
}
