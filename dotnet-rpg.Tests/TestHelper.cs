

namespace dotnet_rpg.Tests;

public static class TestHelper
{
    public static void CheckResponse<T>(ActionResult<ServiceResponse<T>> result, Type expectedObjectResultType, ServiceResponse<T> expectedServiceResponse)
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
}