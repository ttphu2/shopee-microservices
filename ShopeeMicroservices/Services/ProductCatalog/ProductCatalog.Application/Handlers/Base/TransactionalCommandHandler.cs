using Infrastructure.Abstractions.Messaging;
using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Application.Handlers.Base
{
    public abstract class TransactionalCommandHandler<TCommand, TResponse>
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        protected TransactionalCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<TResponse>> Handle(TCommand request, CancellationToken cancellationToken)
        {
            using var session = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = await ExecuteAsync(request, cancellationToken);

                await _unitOfWork.CommitAsync(
                    session,
                    cancellationToken);

                return result;
            }
            catch
            {
                await _unitOfWork.AbortAsync(session, cancellationToken);
                throw;
            }
        }

        protected abstract Task<Result<TResponse>> ExecuteAsync(TCommand request, CancellationToken cancellationToken);
    }
}
