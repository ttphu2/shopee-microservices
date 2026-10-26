using FluentValidation;
using ProductCatalog.Application.Handlers.DeleteProductCommandHandler;

namespace ProductCatalog.Application.Validators
{
    public sealed class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
