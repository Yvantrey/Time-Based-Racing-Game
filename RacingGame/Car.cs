using System;

namespace RacingGame
{
    /// <summary>
    /// Represents a racing car with speed, fuel capacity, and consumption properties
    /// </summary>
    public class Car
    {
        public string Name { get; set; }
        public double Speed { get; set; }
        public double Fuel { get; set; }
        public double FuelCapacity { get; set; }
        public double FuelConsumptionRate { get; set; }

        /// <summary>
        /// Initializes a new racing car
        /// </summary>
        /// <param name="name">Car name</param>
        /// <param name="speed">Initial speed</param>
        /// <param name="fuelCapacity">Maximum fuel capacity</param>
        /// <param name="fuelConsumptionRate">Fuel consumption rate per time unit</param>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid</exception>
        public Car(string name, double speed, double fuelCapacity, double fuelConsumptionRate)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Car name cannot be empty", nameof(name));
            if (speed <= 0)
                throw new ArgumentException("Speed must be positive", nameof(speed));
            if (fuelCapacity <= 0)
                throw new ArgumentException("Fuel capacity must be positive", nameof(fuelCapacity));
            if (fuelConsumptionRate <= 0)
                throw new ArgumentException("Fuel consumption rate must be positive", nameof(fuelConsumptionRate));
                
            Name = name;
            Speed = speed;
            FuelCapacity = fuelCapacity;
            Fuel = fuelCapacity;
            FuelConsumptionRate = fuelConsumptionRate;
        }

        /// <summary>
        /// Simulates car movement for one turn, consuming fuel based on time
        /// </summary>
        /// <param name="time">Time spent driving</param>
        /// <returns>Distance traveled</returns>
        /// <exception cref="InvalidOperationException">Thrown when car has no fuel</exception>
        /// <exception cref="ArgumentException">Thrown when time is not positive</exception>
        public double Drive(double time)
        {
            if (time <= 0)
                throw new ArgumentException("Time must be positive", nameof(time));
                
            if (Fuel <= 0)
                throw new InvalidOperationException("Cannot drive with no fuel");

            double fuelNeeded = FuelConsumptionRate * time;
            if (fuelNeeded > Fuel)
                fuelNeeded = Fuel;

            Fuel -= fuelNeeded;
            return Speed * time * (fuelNeeded / (FuelConsumptionRate * time));
        }

        /// <summary>
        /// Refuels the car to maximum capacity
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when fuel tank is already full</exception>
        public void Refuel()
        {
            if (Fuel >= FuelCapacity)
                throw new InvalidOperationException("Fuel tank is already full");
                
            Fuel = FuelCapacity;
        }

        /// <summary>
        /// Increases car speed, consuming extra fuel
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when insufficient fuel for speed boost</exception>
        public void SpeedUp()
        {
            if (Fuel < 5)
                throw new InvalidOperationException("Not enough fuel to speed up (requires 5 units)");
            
            Speed *= 1.2;
            Fuel -= 5;
        }
    }
}