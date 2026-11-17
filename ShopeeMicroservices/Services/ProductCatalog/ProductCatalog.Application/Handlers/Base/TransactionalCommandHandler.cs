using Infrastructure.Abstractions.Messaging;
using Infrastructure.Errors;
using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Application.Handlers.Base
{
    // Commits only when ExecuteAsync succeeds; a failed ValidationOutcome or an exception aborts the transaction.
    public abstract class TransactionalCommandHandler<TCommand, TResponse>
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        protected TransactionalCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Validation<TResponse>> Handle(TCommand request, CancellationToken cancellationToken)
        {
            using var session = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = await ExecuteAsync(request, cancellationToken);

                if (result.IsSuccessful)
                {
                    await _unitOfWork.CommitAsync(session, cancellationToken);
                }
                else
                {
                    await _unitOfWork.AbortAsync(session, cancellationToken);
                }

                return result;
            }
            catch
            {
                await _unitOfWork.AbortAsync(session, cancellationToken);
                throw;
            }
        }

        protected abstract Task<Validation<TResponse>> ExecuteAsync(TCommand request, CancellationToken cancellationToken);
    }

    // Same as above for commands that return no value.
    public abstract class TransactionalCommandHandler<TCommand>
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly IUnitOfWork _unitOfWork;

        protected TransactionalCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ValidationOutcome> Handle(TCommand request, CancellationToken cancellationToken)
        {
            using var session = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = await ExecuteAsync(request, cancellationToken);

                if (result.IsSuccessful)
                {
                    await _unitOfWork.CommitAsync(session, cancellationToken);
                }
                else
                {
                    await _unitOfWork.AbortAsync(session, cancellationToken);
                }

                return result;
            }
            catch
            {
                await _unitOfWork.AbortAsync(session, cancellationToken);
                throw;
            }
        }

        protected abstract Task<ValidationOutcome> ExecuteAsync(TCommand request, CancellationToken cancellationToken);
    }
}
