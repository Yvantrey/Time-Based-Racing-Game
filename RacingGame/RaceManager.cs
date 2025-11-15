using System;
using System.Collections.Generic;
using System.Linq;

namespace RacingGame
{
    /// <summary>
    /// Manages the racing game logic, player turns, and race outcomes
    /// </summary>
    public class RaceManager
    {
        public Track Track { get; private set; }
        public List<Car> Cars { get; private set; }
        public Dictionary<Car, double> CarPositions { get; private set; }
        public Dictionary<Car, int> CarLaps { get; private set; }
        public Queue<RaceEvent> RaceEvents { get; private set; }
        public bool IsRaceFinished { get; private set; }
        public bool IsRaceStarted { get; private set; }
        public Car Winner { get; private set; }
        public int TotalTurns { get; private set; }
        public int MaxTurns { get; private set; } = 50;

        /// <summary>
        /// Initializes a new race manager with track and cars
        /// </summary>
        /// <param name="track">The racing track</param>
        /// <param name="cars">List of cars participating in the race</param>
        /// <exception cref="ArgumentNullException">Thrown when track or cars is null</exception>
        /// <exception cref="ArgumentException">Thrown when cars list is empty</exception>
        public RaceManager(Track track, List<Car> cars)
        {
            Track = track ?? throw new ArgumentNullException(nameof(track));
            Cars = cars ?? throw new ArgumentNullException(nameof(cars));
            
            if (cars.Count == 0)
                throw new ArgumentException("At least one car is required", nameof(cars));
            
            CarPositions = new Dictionary<Car, double>();
            CarLaps = new Dictionary<Car, int>();
            RaceEvents = new Queue<RaceEvent>();
            TotalTurns = 0;
            
            foreach (var car in Cars)
            {
                CarPositions[car] = 0.0;
                CarLaps[car] = 0;
            }
            
            AddRaceEvent("System", "Race", "Race initialized");
        }
        
        /// <summary>
        /// Starts the race
        /// </summary>
        public void StartRace()
        {
            IsRaceStarted = true;
            AddRaceEvent("System", "Race", "Race started!");
        }
        
        /// <summary>
        /// Finishes the race due to time running out
        /// </summary>
        public void FinishRace()
        {
            if (!IsRaceFinished)
            {
                IsRaceFinished = true;
                Winner = GetStandings().First();
                AddRaceEvent("System", "Race", "Race finished - time expired!");
            }
        }
        
        /// <summary>
        /// Updates all car positions automatically during the race
        /// </summary>
        /// <param name="timeStep">Time step for movement calculation</param>
        public void UpdateCarPositions(double timeStep)
        {
            if (!IsRaceStarted || IsRaceFinished) return;
            
            foreach (var car in Cars)
            {
                if (car.Fuel > 0)
                {
                    try
                    {
                        double distance = car.Drive(timeStep);
                        CarPositions[car] += distance;

                        while (CarPositions[car] >= Track.LapDistance && CarLaps[car] < Track.TotalLaps)
                        {
                            CarPositions[car] -= Track.LapDistance;
                            CarLaps[car]++;
                            AddRaceEvent(car.Name, "Lap", $"Completed lap {CarLaps[car]}");
                            
                            if (CarLaps[car] >= Track.TotalLaps)
                            {
                                IsRaceFinished = true;
                                Winner = car;
                                AddRaceEvent(car.Name, "Race", "Finished the race!");
                                break;
                            }
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        // Car ran out of fuel, skip movement
                    }
                }
            }
            
            CheckRaceCompletion();
        }

        /// <summary>
        /// Processes a player's turn with the specified action
        /// </summary>
        /// <param name="car">The car performing the action</param>
        /// <param name="action">The action to perform</param>
        /// <exception cref="InvalidOperationException">Thrown when race is not started or finished</exception>
        /// <exception cref="ArgumentException">Thrown when car is not in race</exception>
        /// <exception cref="ArgumentNullException">Thrown when car is null</exception>
        public void ProcessTurn(Car car, ActionType action)
        {
            if (car == null)
                throw new ArgumentNullException(nameof(car));
                
            if (!IsRaceStarted)
                throw new InvalidOperationException("Race has not started yet");
                
            if (IsRaceFinished)
                throw new InvalidOperationException("Race is already finished");

            if (!Cars.Contains(car))
                throw new ArgumentException("Car is not in this race", nameof(car));

            try
            {
                switch (action)
                {
                    case ActionType.SpeedUp:
                        car.SpeedUp();
                        MoveCar(car, 1.0);
                        AddRaceEvent(car.Name, "Action", "Speed increased");
                        break;
                    case ActionType.MaintainSpeed:
                        MoveCar(car, 1.0);
                        AddRaceEvent(car.Name, "Action", "Maintained speed");
                        break;
                    case ActionType.PitStop:
                        car.Refuel();
                        AddRaceEvent(car.Name, "Action", "Pit stop - refueled");
                        break;
                    default:
                        throw new ArgumentException($"Invalid action type: {action}", nameof(action));
                }
            }
            catch (InvalidOperationException ex)
            {
                AddRaceEvent(car.Name, "Error", ex.Message);
                throw;
            }

            TotalTurns++;
            CheckRaceCompletion();
        }

        /// <summary>
        /// Moves a car and updates its position and lap count
        /// </summary>
        /// <param name="car">The car to move</param>
        /// <param name="time">Time duration for movement</param>
        public void MoveCar(Car car, double time)
        {
            if (time <= 0)
                throw new ArgumentException("Time must be positive", nameof(time));
                
            double distance = car.Drive(time);
            CarPositions[car] += distance;

            while (CarPositions[car] >= Track.LapDistance)
            {
                CarPositions[car] -= Track.LapDistance;
                CarLaps[car]++;
                AddRaceEvent(car.Name, "Lap", $"Completed lap {CarLaps[car]}");
            }
        }

        /// <summary>
        /// Checks if race should end (laps completed, time/fuel out)
        /// </summary>
        private void CheckRaceCompletion()
        {
            // Check if any car completed all laps
            var finishedCar = CarLaps.FirstOrDefault(kvp => kvp.Value >= Track.TotalLaps);
            if (finishedCar.Key != null)
            {
                IsRaceFinished = true;
                Winner = finishedCar.Key;
                return;
            }

            // Check if time ran out or all cars out of fuel
            if (TotalTurns >= MaxTurns || Cars.All(car => car.Fuel <= 0))
            {
                IsRaceFinished = true;
                Winner = GetStandings().First();
            }
        }

        /// <summary>
        /// Gets the current race standings
        /// </summary>
        public List<Car> GetStandings()
        {
            return Cars.OrderByDescending(car => CarLaps[car])
                      .ThenByDescending(car => CarPositions[car])
                      .ToList();
        }

        /// <summary>
        /// Gets race status information
        /// </summary>
        public string GetRaceStatus()
        {
            if (!IsRaceStarted)
            {
                return "Race ready to start. Click 'Start New Race' to begin!";
            }
            
            if (IsRaceFinished)
            {
                var reason = CarLaps[Winner] >= Track.TotalLaps ? "completed all laps" : 
                           "time ran out";
                return $"Race finished! Winner: {Winner.Name} ({reason})";
            }

            var leader = GetStandings().First();
            return $"Race in progress. Leader: {leader.Name} (Lap {CarLaps[leader] + 1}/{Track.TotalLaps})";
        }
        
        /// <summary>
        /// Adds a race event to the event queue
        /// </summary>
        /// <param name="carName">Name of the car involved</param>
        /// <param name="eventType">Type of event</param>
        /// <param name="description">Event description</param>
        private void AddRaceEvent(string carName, string eventType, string description)
        {
            var raceEvent = new RaceEvent(carName, eventType, description);
            RaceEvents.Enqueue(raceEvent);
            
            // Keep only last 50 events
            while (RaceEvents.Count > 50)
            {
                RaceEvents.Dequeue();
            }
        }
        
        /// <summary>
        /// Gets recent race events
        /// </summary>
        /// <returns>Array of recent race events</returns>
        public RaceEvent[] GetRecentEvents()
        {
            return RaceEvents.ToArray();
        }
    }
}