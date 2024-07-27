using Habanerio.Core.Configurations;
using Habanerio.Core.DBs;

namespace Habanerio.Tests.Unit.Core.Configurations;

public static class AppSettingsHelperTests
{
    [Fact]
    public static void CanCall_GetSettingsWithSettingsKey()
    {
        // Act
        var result = AppSettingsHelper.GetSettings<TestAppSettings>(nameof(TestAppSettings));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("mongodb://mongo:mongo@server:port/", result.ConnectionString);
        Assert.Equal("samples-db-name", result.DatabaseName);
        Assert.Equal("something something", result.SomeProperty);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public static void CannotCall_GetSettingsWithSettingsKey_WithInvalid_SettingsKey(string value)
    {
        Assert.Throws<ArgumentNullException>(() => AppSettingsHelper.GetSettings<TestDoesntExistAppSettings>(value));
    }

    [Fact]
    public static void CanCall_GetSettingsWithNoParameters()
    {
        // Act
        var result = AppSettingsHelper.GetSettings<TestAppSettings>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("mongodb://mongo:mongo@server:port/", result.ConnectionString);
        Assert.Equal("samples-db-name", result.DatabaseName);
        Assert.Equal("something something", result.SomeProperty);
    }

    [Fact]
    public static void CanGet_AppSettings()
    {
        // Act
        var result = AppSettingsHelper.AppSettings;

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public static void CanGet_AspNetCoreEnvironmentVariableKey()
    {
        //Act
        var result = Assert.IsType<string>(AppSettingsHelper.AspNetCoreEnvironmentVariableKey);

        // Assert
        Assert.True(!string.IsNullOrWhiteSpace(result));
        Assert.Equal("ASPNETCORE_ENVIRONMENT", result);
    }

    [Fact]
    public static void CanGet_EnvironmentName()
    {
        var expected = "TestDevelopment";

        // Arrange
        Environment.SetEnvironmentVariable(AppSettingsHelper.AspNetCoreEnvironmentVariableKey, expected);

        // Assert
        var actual = AppSettingsHelper.EnvironmentName;
        Assert.IsType<string>(actual);
        Assert.Equal(expected, actual);
    }

    public class TestAppSettings : DbSettings
    {
        public string DatabaseName { get; set; } = "";

        public string SomeProperty { get; set; } = "";
    }

    public class TestDoesntExistAppSettings : DbSettings
    {
    }
}