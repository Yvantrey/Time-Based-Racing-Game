using Xunit;
using RacingGame;
using System;

namespace RacingGame.Tests
{
    public class TrackTests
    {
        [Fact]
        public void Track_ValidParameters_CreatesTrack()
        {
            var track = new Track(3, 500.0);
            
            Assert.Equal(3, track.TotalLaps);
            Assert.Equal(500.0, track.LapDistance);
        }

        [Fact]
        public void Track_InvalidTotalLaps_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new Track(0, 1000.0));
            Assert.Throws<ArgumentException>(() => new Track(-1, 1000.0));
        }

        [Fact]
        public void Track_InvalidLapDistance_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new Track(5, 0));
            Assert.Throws<ArgumentException>(() => new Track(5, -100.0));
        }
    }
}