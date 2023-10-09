namespace dotnet_rpg.Services.Helper;

public static class ServiceResponseHelper
{
    public static void HandleServiceException<T>(ServiceResponse<T> response, Exception exception)
    {
        response.Success = false;
        response.Message = exception.Message;
    }
}
