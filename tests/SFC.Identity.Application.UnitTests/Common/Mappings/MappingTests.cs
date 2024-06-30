using System;
using System.Runtime.Serialization;
using System.Text.Json;

using AutoMapper;
using AutoMapper.Internal;

using SFC.Identity.Application.Common.Mappings;
using SFC.Identity.Application.Models.Login;
using SFC.Identity.Application.Models.Logout;
using SFC.Identity.Application.Models.Registration;

using Xunit;

namespace SFC.Identity.Application.UnitTests.Common.Mappings;
public class MappingTests
{
    private readonly IConfigurationProvider _configuration;
    private readonly IMapper _mapper;

    public MappingTests()
    {
        _configuration = new MapperConfiguration(config =>
        {
            config.AddProfile<MappingProfile>();
        });

        _mapper = _configuration.CreateMapper();
    }

    [Fact]
    [Trait("Mapping", "Profile")]
    public void Mapping_Profile_ShouldHaveValidConfiguration()
    {
        // Assert
        _configuration.AssertConfigurationIsValid();
    }

    [Theory]
    [Trait("Mapping", "Login")]
    [InlineData(typeof(LoginRequest), typeof(LoginModel))]
    [InlineData(typeof(LoginResult), typeof(LoginResponse))]
    public void Mapping_Login_ShouldHaveValidConfiguration(Type source, Type destination)
    {
        // Arrange
        object? instance = GetInstanceOf(source);

        // Assert
        _mapper.Map(instance, source, destination);
    }

    [Theory]
    [Trait("Mapping", "Logout")]
    [InlineData(typeof(LogoutResult), typeof(LogoutResponse))]
    public void Mapping_Logout_ShouldHaveValidConfiguration(Type source, Type destination)
    {
        // Arrange
        object? instance = GetInstanceOf(source);

        // Assert
        _mapper.Map(instance, source, destination);
    }

    [Theory]
    [Trait("Mapping", "Registration")]
    [InlineData(typeof(RegistrationRequest), typeof(RegistrationModel))]
    public void Mapping_Registration_ShouldHaveValidConfiguration(Type source, Type destination)
    {
        // Arrange
        object? instance = GetInstanceOf(source);

        // Assert
        _mapper.Map(instance, source, destination);
    }

    private static object? GetInstanceOf(Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) != null)
            return Activator.CreateInstance(type)!;

        if (type == typeof(string))
            return string.Empty;

        string json = JsonSerializer.Serialize(type);
        return JsonSerializer.Deserialize<object>(json);
    }
}
