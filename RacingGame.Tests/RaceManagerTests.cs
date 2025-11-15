using Xunit;
using RacingGame;
using System.Collections.Generic;

namespace RacingGame.Tests
{
    public class RaceManagerTests
    {
        [Fact]
        public void RaceManager_ProcessTurn_UpdatesCarPosition()
        {
            var track = new Track(3, 1000);
            var cars = new List<Car> { new Car("Test Car", 100, 50, 10) };
            var raceManager = new RaceManager(track, cars);
            
            raceManager.ProcessTurn(cars[0], ActionType.MaintainSpeed);
            
            Assert.Equal(100, raceManager.CarPositions[cars[0]]);
        }

        [Fact]
        public void RaceManager_CompleteLaps_UpdatesLapCount()
        {
            var track = new Track(3, 100);
            var cars = new List<Car> { new Car("Fast Car", 150, 50, 5) };
            var raceManager = new RaceManager(track, cars);
            
            raceManager.ProcessTurn(cars[0], ActionType.MaintainSpeed);
            
            Assert.Equal(1, raceManager.CarLaps[cars[0]]);
            Assert.Equal(50, raceManager.CarPositions[cars[0]]);
        }

        [Fact]
        public void RaceManager_FinishRace_SetsWinner()
        {
            var track = new Track(1, 100);
            var cars = new List<Car> { new Car("Winner", 150, 50, 5) };
            var raceManager = new RaceManager(track, cars);
            
            raceManager.ProcessTurn(cars[0], ActionType.MaintainSpeed);
            
            Assert.True(raceManager.IsRaceFinished);
            Assert.Equal(cars[0], raceManager.Winner);
        }
    }
}