//using Microsoft.AspNetCore.Mvc;

//using Moq;

//using SFC.Identity.Api.Controllers;
//using SFC.Identity.Api.Infrastructure.Models.Existence;
//using SFC.Identity.Application.Common.Constants;
//using SFC.Identity.Application.Interfaces.Existence;

//namespace SFC.Identity.Api.Tests.Controllers;

//public class ExistenceControllerTests
//{
//    private readonly Mock<IExistenceService> _existenceServiceMock = new();

//    private readonly ExistenceController _controller;

//    public ExistenceControllerTests()
//    {
//        _controller = new ExistenceController(_existenceServiceMock.Object);
//    }

//    [Fact]
//    [Trait("API", "Controller")]
//    public async Task API_Controller_Existence_CheckByUserName_ShouldReturnSuccessResponse()
//    {
//        // Arrange
//        string userName = "username";

//        _existenceServiceMock.Setup(es => es.CheckByUserNameAsync(userName)).ReturnsAsync(true);

//        // Act
//        ActionResult<ExistenceResponse> result = await _controller.CheckExistenceByUserNameAsync(userName);

//        // Assert
//        AssertResponse(result);
//    }

//    [Fact]
//    [Trait("API", "Controller")]
//    public async Task API_Controller_Existence_CheckByEmail_ShouldReturnSuccessResponse()
//    {
//        // Arrange
//        string email = "email@mail.com";

//        _existenceServiceMock.Setup(es => es.CheckByEmailAsync(email)).ReturnsAsync(true);

//        // Act
//        ActionResult<ExistenceResponse> result = await _controller.CheckExistenceByEmailAsync(email);

//        // Assert
//        AssertResponse(result);
//    }

//    private static void AssertResponse(ActionResult<ExistenceResponse> result)
//    {
//        ActionResult<ExistenceResponse> actionResult = Assert.IsType<ActionResult<ExistenceResponse>>(result);

//        OkObjectResult? objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);

//        ExistenceResponse response = Assert.IsType<ExistenceResponse>(objectResult.Value);

//        Assert.True(response?.Success);
//        Assert.Equal(Localization.SuccessResult, response?.Message);
//        Assert.True(response?.Exist);
//    }
//}