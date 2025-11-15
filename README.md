# Racing Game

A real-time racing simulation game built with C# and WPF where players control cars in a timed race with fuel management mechanics.

## Description

This is a strategic racing game where players manage multiple cars competing in a 2-minute timed race. Each car has unique characteristics including speed, fuel capacity, and consumption rates. The game features real-time movement, lap tracking, and strategic decision-making around fuel management and pit stops.

### Key Features
- **Real-time Racing**: Cars move automatically during the race with 100ms updates
- **Fuel Management**: Strategic fuel consumption and pit stop decisions
- **Multiple Cars**: Control different cars with unique stats (Red Racer, Blue Bolt, Green Machine)
- **Lap System**: Complete 5 laps of 1000m each to win
- **Time Pressure**: 2-minute race timer adds urgency
- **Live Statistics**: Real-time display of speed, fuel, position, and lap progress

## How to Play

### Starting the Race
1. Launch the application
2. Select a car from the dropdown menu
3. Click "🏁 Start New Race" to begin the 2-minute timer
4. Cars will automatically move based on their speed and fuel

### Controls
- **Speed Up**: Increases car speed by 20% but consumes 5 fuel units (requires 5+ fuel)
- **Maintain Speed**: Car continues at current speed with normal fuel consumption
- **Pit Stop**: Instantly refuels the car to maximum capacity

### Winning Conditions
- **Primary**: First car to complete 5 laps wins immediately
- **Secondary**: If time runs out, the car with the most progress (laps + position) wins

### Strategy Tips
- Monitor fuel levels carefully - cars stop moving when fuel reaches zero
- Use speed boosts strategically when you have sufficient fuel
- Time your pit stops to avoid running out of fuel
- Balance speed increases with fuel conservation

### Car Specifications
- **Red Racer**: Speed 100, Fuel 50, Consumption 1.0
- **Blue Bolt**: Speed 90, Fuel 60, Consumption 0.8 (most efficient)
- **Green Machine**: Speed 110, Fuel 40, Consumption 1.2 (fastest but thirsty)

## How to Run the Project

### Prerequisites
- .NET 6.0 or later
- Windows operating system (WPF application)
- Visual Studio 2022 or Visual Studio Code with C# extension

### Running from Source
1. **Clone or download** the project to your local machine
2. **Open terminal/command prompt** and navigate to the project directory:
   ```
   cd "\Racing Game"
   ```
3. **Build the project**:
   ```
   dotnet build
   ```
4. **Run the application**:
   ```
   dotnet run --project RacingGame
   ```

### Running from Visual Studio
1. Open `RacingGame.sln` in Visual Studio
2. Set `RacingGame` as the startup project
3. Press `F5` or click "Start" to run

### Running the Executable
After building, you can run the executable directly:
```
cd "RacingGame\bin\Debug\net6.0-windows"
RacingGame.exe
```

### Running Tests
To run the unit tests:
```
dotnet test
```

## Project Structure
```
RacingGame/
├── RacingGame/           # Main application
│   ├── Car.cs           # Car model with fuel and speed mechanics
│   ├── Track.cs         # Track configuration (laps, distance)
│   ├── RaceManager.cs   # Core game logic and race management
│   ├── MainWindow.xaml  # UI layout
│   └── MainWindow.xaml.cs # UI logic and event handling
├── RacingGame.Tests/    # Unit tests
└── README.md           # This file
```

## Technology Stack
- **Framework**: .NET 6.0
- **UI**: WPF (Windows Presentation Foundation)
- **Language**: C#
- **Testing**: xUnit