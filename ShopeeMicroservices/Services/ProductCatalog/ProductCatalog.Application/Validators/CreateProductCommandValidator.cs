using FluentValidation;
using ProductCatalog.Application.Handlers.CreateProductCommandHandler;

namespace ProductCatalog.Application.Validators
{
    public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.ProductCode).NotEmpty();

            Include(new ProductCommandValidator());
        }
    }
}
