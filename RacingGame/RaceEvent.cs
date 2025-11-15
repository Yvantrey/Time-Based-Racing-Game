using System;

namespace RacingGame
{
    /// <summary>
    /// Represents a race event with timestamp and description
    /// </summary>
    public struct RaceEvent
    {
        public DateTime Timestamp { get; }
        public string CarName { get; }
        public string EventType { get; }
        public string Description { get; }

        public RaceEvent(string carName, string eventType, string description)
        {
            Timestamp = DateTime.Now;
            CarName = carName;
            EventType = eventType;
            Description = description;
        }

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] {CarName}: {Description}";
        }
    }
}