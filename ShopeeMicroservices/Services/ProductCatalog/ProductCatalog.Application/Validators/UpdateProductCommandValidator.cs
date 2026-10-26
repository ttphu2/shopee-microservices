using FluentValidation;
using ProductCatalog.Application.Handlers.UpdateProductCommandHandler;

namespace ProductCatalog.Application.Validators
{
    public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();

            Include(new ProductCommandValidator());
        }
    }
}
