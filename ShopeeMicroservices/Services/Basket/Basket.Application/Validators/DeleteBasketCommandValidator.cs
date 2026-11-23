using Basket.Application.Handlers.DeleteBasketCommandHandler;
using FluentValidation;

namespace Basket.Application.Validators
{
    public sealed class DeleteBasketCommandValidator : AbstractValidator<DeleteBasketCommand>
    {
        public DeleteBasketCommandValidator()
        {
            RuleFor(x => x.UserName).NotEmpty();
        }
    }
}
