
using AutoMapper;
using PineConePro.Erp.MyModule.Application.Mappings;

namespace PineConePro.Erp.MyModule.Application.Tests.Mappings;

/// <summary>
/// Unit tests for the <see cref="MyAggregateProfile"/> class.
/// </summary>
public sealed class AggregateProfileTests
{
    /// <summary>
    /// Tests that the mapping configuration is valid.
    /// </summary>
    [Fact]
    public void ShouldHaveValidConfiguration()
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MyAggregateProfile>();
        });

        // Assert
        configuration.AssertConfigurationIsValid();
    }
}
