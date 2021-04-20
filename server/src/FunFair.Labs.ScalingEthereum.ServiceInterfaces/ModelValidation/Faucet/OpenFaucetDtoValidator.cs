using FluentValidation;
using FunFair.Labs.ScalingEthereum.ServiceInterfaces.Models.Faucet;

namespace FunFair.Labs.ScalingEthereum.ServiceInterfaces.ModelValidation.Faucet
{
    /// <summary>
    ///     Validator of the <see cref="OpenFaucetDto" /> model.
    /// </summary>
    public sealed class OpenFaucetDtoValidator : AbstractValidator<OpenFaucetDto>
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public OpenFaucetDtoValidator()
        {
            this.RuleFor(expression: x => x.Network)
                .NotEmpty();

            this.RuleFor(expression: x => x.Address)
                .NotEmpty();
        }
    }
}