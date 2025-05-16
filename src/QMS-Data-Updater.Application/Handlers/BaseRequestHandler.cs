using MediatR;
using QMS_Data_Updater.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace QMS_Data_Updater.Application.Handlers
{
    /// <summary>
    /// Provides a base class for event handlers to reduce boilerplate and enforce a consistent pattern.
    /// </summary>
    /// <typeparam name="TRequest">The event/request type.</typeparam>
    public abstract class BaseRequestHandler<TRequest> : IRequestHandler<TRequest, IOperationResult>
        where TRequest : IRequest<IOperationResult>
    {
        public async Task<IOperationResult> Handle(TRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return await HandleRequest(request, cancellationToken);
            }
            catch (System.Exception ex)
            {
                // Optionally log here if you inject a logger
                return OperationResult.Failure($"Unhandled exception: {ex.Message}", exception: ex);
            }
        }

        /// <summary>
        /// Implement this method in derived handlers for actual business logic.
        /// </summary>
        protected abstract Task<IOperationResult> HandleRequest(TRequest request, CancellationToken cancellationToken);
    }
}