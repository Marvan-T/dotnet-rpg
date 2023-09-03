using System.Linq.Expressions;

namespace dotnet_rpg.Tests;

public static class TestHelper
{
    public static void CheckResponse<T>(ActionResult<ServiceResponse<T>> result, Type expectedObjectResultType,
        ServiceResponse<T> expectedServiceResponse)
    {
        // Assert the type of the Result
        Assert.IsType(expectedObjectResultType, result.Result);

        // Then cast the result to its actual type to access its Value property
        var objectResult = (ObjectResult)result.Result;
        var response = Assert.IsType<ServiceResponse<T>>(objectResult.Value);

        //Assert against the properties of service response
        response.Success.Should().Be(expectedServiceResponse.Success);
        response.Message.Should().Be(expectedServiceResponse.Message);
        response.Data.Should().BeEquivalentTo(expectedServiceResponse.Data);
    }

    public static ServiceResponse<T> CreateServiceResponse<T>(T data, bool success = true, string message = null)
    {
        return new ServiceResponse<T> { Data = data, Success = success, Message = message };
    }

    public static void SetupMockServiceCall<TService, TResponse>(this Mock<TService> mockService,
        Expression<Func<TService, Task<ServiceResponse<TResponse>>>> call,
        ServiceResponse<TResponse> response) where TService : class
    {
        mockService.Setup(call).ReturnsAsync(response);
    }

    public static void SetupMockMethodCall<TMock, TResult>(this Mock<TMock> mock, Expression<Func<TMock, TResult>> call,
        TResult result) where TMock : class
    {
        mock.Setup(call).Returns(result);
    }
}