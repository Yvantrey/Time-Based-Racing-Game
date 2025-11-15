using Xunit;
using RacingGame;

namespace RacingGame.Tests
{
    public class CarTests
    {
        [Fact]
        public void Car_Drive_ConsumeFuelAndReturnDistance()
        {
            var car = new Car("Test Car", 100, 50, 10);
            var distance = car.Drive(1.0);
            
            Assert.Equal(100, distance);
            Assert.Equal(40, car.Fuel);
        }

        [Fact]
        public void Car_SpeedUp_IncreaseSpeedAndConsumeFuel()
        {
            var car = new Car("Test Car", 100, 50, 10);
            car.SpeedUp();
            
            Assert.Equal(120, car.Speed);
            Assert.Equal(45, car.Fuel);
        }

        [Fact]
        public void Car_Refuel_RestoreToMaxCapacity()
        {
            var car = new Car("Test Car", 100, 50, 10);
            car.Drive(1.0);
            car.Refuel();
            
            Assert.Equal(50, car.Fuel);
        }
    }
}