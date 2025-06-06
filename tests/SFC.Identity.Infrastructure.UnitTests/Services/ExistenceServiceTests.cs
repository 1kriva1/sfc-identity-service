﻿//using Microsoft.AspNetCore.Identity;

//using Moq;

//using SFC.Identity.Infrastructure.Persistence.Entities;
//using SFC.Identity.Infrastructure.Services;

//namespace SFC.Identity.Infrastructure.UnitTests.Services;

//public class ExistenceServiceTests
//{
//    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

//    private readonly ExistenceService _service;

//    public ExistenceServiceTests()
//    {
//        Mock<IUserStore<ApplicationUser>> userStore = new();
//        _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStore.Object,
//            null!, null!, null!, null!, null!, null!, null!, null!);
//        _service = new ExistenceService(_userManagerMock.Object);
//    }

//    [Fact]
//    [Trait("Service", "Existence")]
//    public async Task Service_Existence_ShouldReturnPlayerExistByUserName()
//    {
//        // Arrange
//        string username = "username";

//        _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(new ApplicationUser());

//        // Act
//        bool result = await _service.CheckByUserNameAsync(username);

//        // Assert
//        Assert.True(result);
//    }

//    [Fact]
//    [Trait("Service", "Existence")]
//    public async Task Service_Existence_ShouldReturnPlayerNotExistByUserName()
//    {
//        // Arrange
//        string username = "username";

//        _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(new ApplicationUser());

//        // Act
//        bool result = await _service.CheckByUserNameAsync("username_another");

//        // Assert
//        Assert.False(result);
//    }

//    [Fact]
//    [Trait("Service", "Existence")]
//    public async Task Service_Existence_ShouldReturnPlayerExistByEmail()
//    {
//        // Arrange
//        string email = "email@mail.com";

//        _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(new ApplicationUser());

//        // Act
//        bool result = await _service.CheckByEmailAsync(email);

//        // Assert
//        Assert.True(result);
//    }

//    [Fact]
//    [Trait("Service", "Existence")]
//    public async Task Service_Existence_ShouldReturnPlayerNotExistByEmail()
//    {
//        // Arrange
//        string email = "email@mail.com";

//        _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(new ApplicationUser());

//        // Act
//        bool result = await _service.CheckByEmailAsync("email_another@mail.com");

//        // Assert
//        Assert.False(result);
//    }
//}