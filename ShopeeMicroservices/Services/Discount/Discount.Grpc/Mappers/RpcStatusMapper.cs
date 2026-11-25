using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Google.Rpc;
using Grpc.Core;
using Infrastructure.Errors;
using RpcStatus = Google.Rpc.Status;

namespace Discount.Grpc.Mappers
{
    // The one place where handler outcomes become gRPC errors. Handlers stay transport-neutral and keep returning
    // Error values; failed outcomes are thrown here as RpcException with the standard rich error details:
    // google.rpc.BadRequest for invalid fields and google.rpc.ErrorInfo for the business error code.
    public static class RpcStatusMapper
    {
        private const string ErrorDomain = "discount";

        private static readonly Dictionary<string, StatusCode> BusinessStatusCodes = new()
        {
            [Error.NotFound.Code] = StatusCode.NotFound,
            [Error.AlreadyExists.Code] = StatusCode.AlreadyExists
        };

        public static void ThrowIfFailed(this ValidationOutcome outcome)
        {
            if (outcome.IsSuccessful)
            {
                return;
            }

            throw ToRpcStatus(outcome.ValidationMessages!.Errors).ToRpcException();
        }

        // A successful Validation<TValue> never holds null (a null value becomes Error.NullValue).
        public static TValue GetValueOrThrow<TValue>(this Validation<TValue> outcome)
        {
            outcome.ThrowIfFailed();

            return outcome.Value!;
        }

        private static RpcStatus ToRpcStatus(IReadOnlyList<Error> errors)
        {
            var status = new RpcStatus
            {
                Code = (int)StatusCode.InvalidArgument,
                Message = string.Join(" ", errors.Select(error => error.Message))
            };

            // Anything that is not a known business error came from the validators, whose Code is the property path.
            var fieldErrors = errors.Where(error => !IsBusinessError(error)).ToList();
            if (fieldErrors.Count > 0)
            {
                status.Details.Add(Any.Pack(ToBadRequest(fieldErrors)));
            }

            var businessError = errors.FirstOrDefault(IsBusinessError);
            if (businessError is not null)
            {
                status.Code = (int)BusinessStatusCodes[businessError.Code];
                status.Details.Add(Any.Pack(new ErrorInfo { Reason = businessError.Code, Domain = ErrorDomain }));
            }

            return status;
        }

        private static bool IsBusinessError(Error error)
        {
            return BusinessStatusCodes.ContainsKey(error.Code);
        }

        private static BadRequest ToBadRequest(IEnumerable<Error> fieldErrors)
        {
            var badRequest = new BadRequest();

            badRequest.FieldViolations.AddRange(fieldErrors.Select(error => new BadRequest.Types.FieldViolation
            {
                Field = ToProtoFieldName(error.Code),
                Description = error.Message
            }));

            return badRequest;
        }

        // Commands may nest what the proto keeps flat (DeleteDiscountCommand.Key.SkuCode is sku_code on the wire),
        // so only the last segment of the property path is used, converted to the proto's snake_case.
        private static string ToProtoFieldName(string propertyPath)
        {
            var propertyName = propertyPath[(propertyPath.LastIndexOf('.') + 1)..];

            return JsonNamingPolicy.SnakeCaseLower.ConvertName(propertyName);
        }
    }
}
