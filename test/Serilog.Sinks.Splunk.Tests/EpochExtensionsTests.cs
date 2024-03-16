using Serilog.Sinks.Splunk;
using System;
using Xunit;

namespace Serilog.Sinks.Splunk.Tests
{
    public class EpochExtensionsTests
    {
        [Fact]
        public void ToEpoch_ShouldReturnCorrectEpochTime()
        {
            // Arrange
            var dateTimeOffset = new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var expectedEpochTime = 1640995200.000; // Epoch time for 2022-01-01 00:00:00

            // Act
            var result = dateTimeOffset.ToEpoch();

            // Assert
            Assert.Equal(expectedEpochTime, result);
        }

        [Fact]
        public void ToEpoch_ShouldReturnCorrectEpochTime_Milliseconds()
        {
            // Arrange
            var dateTimeOffset = new DateTimeOffset(2022, 1, 1, 0, 0, 0, 123, TimeSpan.Zero);
            var expectedEpochTime = 1640995200.123; // Epoch time for 2022-01-01 00:00:00.123

            // Act
            var result = dateTimeOffset.ToEpoch(SubSecondPrecision.Milliseconds);

            // Assert
            Assert.Equal(expectedEpochTime, result);
        }

        [Fact]
        public void ToEpoch_ShouldReturnCorrectEpochTime_Microseconds()
        {
            // Arrange
            var dateTimeOffset = new DateTimeOffset(2022, 1, 1, 0, 0, 0, 123, TimeSpan.Zero) + TimeSpan.FromMicroseconds(456);
            var expectedEpochTime = 1640995200.123456; // Epoch time for 2022-01-01 00:00:00.123

            // Act
            var result = dateTimeOffset.ToEpoch(SubSecondPrecision.Microseconds);

            // Assert
            Assert.Equal(expectedEpochTime, result);
        }

        [Fact]
        public void ToEpoch_ShouldReturnCorrectEpochTime_Nanoseconds()
        {
            // Arrange
            // using from ticks here, NanoSeconds is not available in TimeSpan. Nanoseconds Per Tick = 100L.
            var dateTimeOffset = new DateTimeOffset(2022, 1, 1, 0, 0, 0, 123, TimeSpan.Zero) + TimeSpan.FromMicroseconds(456) + TimeSpan.FromTicks(7);
            var expectedEpochTime = 1640995200.123456700; // Epoch time for 2022-01-01 00:00:00.123

            // Act
            var result = dateTimeOffset.ToEpoch(SubSecondPrecision.Nanoseconds);

            // Assert
            Assert.Equal(expectedEpochTime, result);
        }
    }
}