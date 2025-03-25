namespace Lyt.Avalonia.AstroPic.Service; 

public static class Utilities
{
    public static void ThrowIfBad ( this HttpStatusCode statusCode )
    {
        switch (statusCode)
        {
            case HttpStatusCode.OK:
                return; 

            case HttpStatusCode.BadRequest:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Bad Request");
            case HttpStatusCode.Unauthorized:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Unauthorized");
            case HttpStatusCode.Forbidden:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Forbidden");
            case HttpStatusCode.NotFound:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Not Found");
            case HttpStatusCode.MethodNotAllowed:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Method Not Allowed");
            case HttpStatusCode.NotAcceptable:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Not Acceptable");
            case HttpStatusCode.ProxyAuthenticationRequired:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Proxy Authentication Required");
            case HttpStatusCode.RequestTimeout:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Request Timeout");
            case HttpStatusCode.Conflict:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Conflict");
            case HttpStatusCode.Gone:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Gone");
            case HttpStatusCode.LengthRequired:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Length Required");
            case HttpStatusCode.PreconditionFailed:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Precondition Failed");
            case HttpStatusCode.RequestEntityTooLarge:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Request Entity Too Large");
            case HttpStatusCode.RequestUriTooLong:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Request Uri Too Long");
            case HttpStatusCode.UnsupportedMediaType:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Unsupported Media Type");
            case HttpStatusCode.RequestedRangeNotSatisfiable:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Requested Range Not Satisfiable");
            case HttpStatusCode.ExpectationFailed:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Expectation Failed");
            case HttpStatusCode.MisdirectedRequest:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Misdirected Request");
            case HttpStatusCode.UnprocessableEntity:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Unprocessable Entity");
            case HttpStatusCode.Locked:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Locked");
            case HttpStatusCode.FailedDependency:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Failed Dependency");
            case HttpStatusCode.UpgradeRequired:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Upgrade Required");
            case HttpStatusCode.PreconditionRequired:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Precondition Required");
            case HttpStatusCode.TooManyRequests:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Too Many Requests");
            case HttpStatusCode.RequestHeaderFieldsTooLarge:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Request Header Fields Too Large");
            case HttpStatusCode.UnavailableForLegalReasons:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Unavailable For Legal Reasons");
            case HttpStatusCode.InternalServerError:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Internal Server Error");
            case HttpStatusCode.NotImplemented:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Not Implemented");
            case HttpStatusCode.BadGateway:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Bad Gateway");
            case HttpStatusCode.ServiceUnavailable:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Service Unavailable");
            case HttpStatusCode.GatewayTimeout:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Gateway Timeout");
            case HttpStatusCode.HttpVersionNotSupported:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: HTTP Version Not Supported");
            case HttpStatusCode.VariantAlsoNegotiates:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Variant Also Negotiates");
            case HttpStatusCode.InsufficientStorage:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Insufficient Storage");
            case HttpStatusCode.LoopDetected:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Loop Detected");
            case HttpStatusCode.NotExtended:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Not Extended");
            case HttpStatusCode.NetworkAuthenticationRequired:
                throw new Exception($"Endpoint unavailable \n{(int)statusCode}: Network Authentication Required");
            default:
                throw new Exception($"Unknown status code: {statusCode}");
        }
    }
}
