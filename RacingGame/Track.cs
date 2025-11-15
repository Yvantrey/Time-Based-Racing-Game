using System;

namespace RacingGame
{
    /// <summary>
    /// Represents the racing track with lap configuration
    /// </summary>
    public class Track
    {
        public int TotalLaps { get; private set; } = 5;
        public double LapDistance { get; private set; } = 1000.0;

        /// <summary>
        /// Initializes a new track with default values (5 laps, 1000 distance)
        /// </summary>
        public Track()
        {
        }

        /// <summary>
        /// Initializes a new track with specified configuration
        /// </summary>
        /// <param name="totalLaps">Total number of laps to complete the race</param>
        /// <param name="lapDistance">Distance of each lap</param>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid</exception>
        public Track(int totalLaps, double lapDistance)
        {
            if (totalLaps <= 0)
                throw new ArgumentException("Total laps must be positive", nameof(totalLaps));
            if (lapDistance <= 0)
                throw new ArgumentException("Lap distance must be positive", nameof(lapDistance));
                
            TotalLaps = totalLaps;
            LapDistance = lapDistance;
        }
    }
}