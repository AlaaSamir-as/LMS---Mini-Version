using FluentValidation;
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
                .GroupBy(failure => string.IsNullOrWhiteSpace(failure.PropertyName) ? "General" : failure.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(failure => failure.ErrorMessage).Distinct().ToArray());

            int statusCode = DetermineStatusCode(failures);

            return CreateFailureResponse(errors, statusCode);
        }

        private static int DetermineStatusCode(List<ValidationFailure> failures)
        {
            var detectedCodes = new HashSet<int>();

            foreach (var failure in failures)
            {
                if (int.TryParse(failure.ErrorCode, out int parsedCode))
                {
                    detectedCodes.Add(parsedCode);
                    continue;
                }

                if (failure.CustomState is int stateCode)
                {
                    detectedCodes.Add(stateCode);
                    continue;
                }

                var errorCode = failure.ErrorCode?.ToLowerInvariant() ?? "";
                if (errorCode.Contains("notfound") || errorCode == "404")
                {
                    detectedCodes.Add(404);
                    continue;
                }
                if (errorCode.Contains("conflict") || errorCode == "409")
                {
                    detectedCodes.Add(409);
                    continue;
                }

                var msg = failure.ErrorMessage?.ToLowerInvariant() ?? "";
                if (msg.Contains("not found") || msg.Contains("does not exist"))
                {
                    detectedCodes.Add(404);
                    continue;
                }
                if (msg.Contains("already") || msg.Contains("conflict") || msg.Contains("cannot delete") ||
                    msg.Contains("cancelled") || msg.Contains("is not active"))
                {
                    detectedCodes.Add(409);
                    continue;
                }

                detectedCodes.Add(400);
            }

            // Priority:
            // 1. 400 Bad Request (Input / format errors)
            // 2. 404 Not Found (Entity does not exist)
            // 3. 409 Conflict (Duplicate, invalid state transition, business rule violation)
            if (detectedCodes.Contains(400))
                return 400;
            if (detectedCodes.Contains(404))
                return 404;
            if (detectedCodes.Contains(409))
                return 409;

            return detectedCodes.FirstOrDefault(400);
        }

        private static TResponse CreateFailureResponse(IDictionary<string, string[]> errors, int statusCode)
        {
            if (typeof(TResponse) == typeof(RequestResponse))
                return (TResponse)(object)RequestResponse.Fail(statusCode, errors);

            if (typeof(TResponse).IsGenericType &&
                typeof(TResponse).GetGenericTypeDefinition() == typeof(RequestResponse<>))
            {
                var failMethod = typeof(TResponse).GetMethod(
                    nameof(RequestResponse<object>.Fail),
                    new[] { typeof(int), typeof(IDictionary<string, string[]>) });

                if (failMethod is not null)
                {
                    return (TResponse)failMethod.Invoke(null, new object?[] { statusCode, errors })!;
                }
            }

            throw new InvalidOperationException(
                $"FluentValidation response type '{typeof(TResponse).Name}' is not supported. Expected RequestResponse or RequestResponse<T>.");
        }
    }
}
