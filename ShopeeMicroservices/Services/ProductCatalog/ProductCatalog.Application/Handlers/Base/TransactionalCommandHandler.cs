using Infrastructure.Abstractions.Messaging;
using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Application.Handlers.Base
{
    // Commits only when ExecuteAsync succeeds; a failure Result or an exception aborts the transaction.
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

                if (result.IsSuccess)
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

        protected abstract Task<Result<TResponse>> ExecuteAsync(TCommand request, CancellationToken cancellationToken);
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

        public async Task<Result> Handle(TCommand request, CancellationToken cancellationToken)
        {
            using var session = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = await ExecuteAsync(request, cancellationToken);

                if (result.IsSuccess)
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

        protected abstract Task<Result> ExecuteAsync(TCommand request, CancellationToken cancellationToken);
    }
}
