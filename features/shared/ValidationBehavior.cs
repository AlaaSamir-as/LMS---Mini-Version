using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace LMS___Mini_Version.features.shared
{
    public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!_validators.Any())
                return await next();

            var validationContext = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(
                _validators.Select(validator => validator.ValidateAsync(validationContext, cancellationToken)));
            var failures = validationResults
                .SelectMany(result => result.Errors)
                .Where(failure => failure is not null)
                .ToList();

            if (failures.Count == 0)
                return await next();

            var errors = failures
                .GroupBy(failure => failure.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(failure => failure.ErrorMessage).ToArray());

            return CreateFailureResponse(errors);
        }

        private static TResponse CreateFailureResponse(IDictionary<string, string[]> errors)
        {
            if (typeof(TResponse) == typeof(RequestResponse))
                return (TResponse)(object)RequestResponse.Fail(400, errors);

            if (typeof(TResponse).IsGenericType &&
                typeof(TResponse).GetGenericTypeDefinition() == typeof(RequestResponse<>))
            {
                return (TResponse)typeof(TResponse)
                    .GetMethod(nameof(RequestResponse<int>.Fail))!
                    .Invoke(null, new object?[] { 400, errors })!;
            }

            throw new InvalidOperationException(
                $"FluentValidation response type '{typeof(TResponse).Name}' is not supported.");
        }
    }
}
