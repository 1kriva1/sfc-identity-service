using AutoMapper;

using SFC.Identity.Application.Common.Extensions;

namespace SFC.Identity.Application.UnitTests.Common.Extensions;
public class MappingExtensionsTests
{
    private class TestSource
    {
        public string MappedProp { get; set; } = string.Empty;

        public string NotMappedProp { get; set; } = string.Empty;
    }

    private class TestDestination
    {
        public string MappedProp { get; set; } = string.Empty;

        public string DestinationMappedProp { get; set; } = string.Empty;
    }

    public class MappingProfileTest : Profile
    {
        public MappingProfileTest()
        {
            CreateMap<TestSource, TestDestination>()
                .IgnoreAllNonExisting();
        }
    }

    private readonly IConfigurationProvider _configuration;
    private readonly IMapper _mapper;

    public MappingExtensionsTests()
    {
        _configuration = new MapperConfiguration(config => config.AddProfile<MappingProfileTest>());
        _mapper = _configuration.CreateMapper();
    }

    [Fact]
    [Trait("Extension", "Mapping")]
    public void Mapping_Profile_ShouldHaveValidConfiguration()
    {
        // Assert
        _configuration.AssertConfigurationIsValid();
    }

    [Fact]
    [Trait("Extension", "Mapping")]
    public void Extension_Mapping_ShouldMapOnlyMappedProperty()
    {
        // Arrange
        TestSource source = new()
        {
            MappedProp = "Value",
            NotMappedProp = "Another_Value"
        };

        // Act
        TestDestination destination = _mapper.Map<TestDestination>(source);

        // Assert
        Assert.NotNull(destination);
        Assert.Equal(source.MappedProp, destination.MappedProp);
        Assert.NotEqual(source.NotMappedProp, destination.DestinationMappedProp);
        Assert.Equal(string.Empty, destination.DestinationMappedProp);
    }
}
