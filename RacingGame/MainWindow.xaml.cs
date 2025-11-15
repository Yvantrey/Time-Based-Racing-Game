using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RacingGame
{
    public partial class MainWindow : Window
    {
        private RaceManager _raceManager;
        private ObservableCollection<CarViewModel> _carViewModels;
        private Car _selectedCar;
        private DispatcherTimer _raceTimer;
        private DateTime _raceStartTime;
        private TimeSpan _raceDuration = TimeSpan.FromMinutes(2); // 2 minute race

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            InitializeRace();
        }

        private void InitializeTimer()
        {
            _raceTimer = new DispatcherTimer();
            _raceTimer.Interval = TimeSpan.FromMilliseconds(100); // Update every 100ms
            _raceTimer.Tick += RaceTimer_Tick;
        }

        private void RaceTimer_Tick(object sender, EventArgs e)
        {
            if (_raceManager?.IsRaceStarted == true && !_raceManager.IsRaceFinished)
            {
                var elapsed = DateTime.Now - _raceStartTime;
                var remaining = _raceDuration - elapsed;
                
                if (remaining <= TimeSpan.Zero)
                {
                    _raceManager.FinishRace();
                    _raceTimer.Stop();
                    remaining = TimeSpan.Zero;
                }
                else
                {
                    // Move all cars automatically
                    _raceManager.UpdateCarPositions(0.1); // 0.1 second time step
                }
                
                // Update time display
                TimeProgressBar.Maximum = _raceDuration.TotalSeconds;
                TimeProgressBar.Value = remaining.TotalSeconds;
                TimeLabel.Text = $"{remaining.Minutes:D2}:{remaining.Seconds:D2}";
                
                UpdateDisplay();
            }
        }

        private void InitializeRace()
        {
            // Stop any running timer
            _raceTimer?.Stop();
            
            var track = new Track(5, 1000);
            var cars = new List<Car>
            {
                new Car("Red Racer", 100, 50, 1),
                new Car("Blue Bolt", 90, 60, 0.8),
                new Car("Green Machine", 110, 40, 1.2)
            };

            _raceManager = new RaceManager(track, cars);
            _carViewModels = new ObservableCollection<CarViewModel>();

            foreach (var car in cars)
            {
                _carViewModels.Add(new CarViewModel(car, _raceManager));
            }

            CarsListView.ItemsSource = _carViewModels;
            CarSelectionComboBox.ItemsSource = cars;
            CarSelectionComboBox.DisplayMemberPath = "Name";
            
            if (cars.Count > 0)
            {
                CarSelectionComboBox.SelectedIndex = 0;
                _selectedCar = cars[0];
            }
            
            // Reset button text and UI
            StartRaceButton.Content = "🏁 Start New Race";
            
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            RaceStatusText.Text = _raceManager.GetRaceStatus();
            
            // Time is now updated by the timer
            if (!_raceManager?.IsRaceStarted == true)
            {
                TimeProgressBar.Value = _raceDuration.TotalSeconds;
                TimeLabel.Text = $"{_raceDuration.Minutes:D2}:{_raceDuration.Seconds:D2}";
            }
            
            // Update selected car details
            if (_selectedCar != null)
            {
                UpdateSelectedCarDisplay();
            }
            
            foreach (var carVM in _carViewModels)
            {
                carVM.UpdateStatus();
            }
        }
        
        private void UpdateSelectedCarDisplay()
        {
            if (_selectedCar == null) return;
            
            SelectedCarName.Text = _selectedCar.Name;
            
            // Update fuel progress bar
            FuelProgressBar.Maximum = _selectedCar.FuelCapacity;
            FuelProgressBar.Value = _selectedCar.Fuel;
            FuelLabel.Text = $"{_selectedCar.Fuel:F1}/{_selectedCar.FuelCapacity}";
            
            // Update current lap
            var currentLap = Math.Min(_raceManager.CarLaps[_selectedCar] + 1, _raceManager.Track.TotalLaps);
            CurrentLapLabel.Text = $"Lap: {currentLap}/{_raceManager.Track.TotalLaps}";
            
            // Update speed/status
            SpeedStatusLabel.Text = $"Speed: {_selectedCar.Speed:F1} km/h | Fuel: {_selectedCar.Fuel:F1}";
            
            // Update progress indicator
            UpdateProgressIndicator();
            
            // Enable/disable buttons
            bool canAct = _raceManager.IsRaceStarted && !_raceManager.IsRaceFinished;
            SpeedUpButton.IsEnabled = canAct && _selectedCar.Fuel >= 5;
            MaintainSpeedButton.IsEnabled = canAct && _selectedCar.Fuel > 0;
            PitStopButton.IsEnabled = canAct;
        }
        
        private void UpdateProgressIndicator()
        {
            if (_selectedCar == null) return;
            
            var position = _raceManager.CarPositions[_selectedCar];
            var lapDistance = _raceManager.Track.LapDistance;
            var progress = position / lapDistance;
            
            var indicator = new char[20];
            for (int i = 0; i < 20; i++)
            {
                indicator[i] = i < progress * 20 ? '=' : ' ';
            }
            
            var arrowPos = (int)(progress * 19);
            if (arrowPos < 19) indicator[arrowPos] = '>';
            
            ProgressIndicator.Text = $"[{new string(indicator)}]";
        }

        private void CarSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            _selectedCar = CarSelectionComboBox.SelectedItem as Car;
            UpdateSelectedCarDisplay();
        }
        
        private void SpeedUp_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCar != null && !_raceManager.IsRaceFinished)
            {
                try
                {
                    _raceManager.ProcessTurn(_selectedCar, ActionType.SpeedUp);
                    UpdateDisplay();
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Action Failed");
                }
            }
        }

        private void MaintainSpeed_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCar != null && !_raceManager.IsRaceFinished)
            {
                try
                {
                    _raceManager.ProcessTurn(_selectedCar, ActionType.MaintainSpeed);
                    UpdateDisplay();
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Action Failed");
                }
            }
        }

        private void PitStop_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCar != null && !_raceManager.IsRaceFinished)
            {
                _raceManager.ProcessTurn(_selectedCar, ActionType.PitStop);
                UpdateDisplay();
            }
        }

        private void StartRace_Click(object sender, RoutedEventArgs e)
        {
            if (_raceManager?.IsRaceStarted == true)
            {
                // Restart race
                InitializeRace();
            }
            
            // Start new race
            _raceStartTime = DateTime.Now;
            _raceManager.StartRace();
            _raceTimer.Start();
            StartRaceButton.Content = "🔄 Restart Race";
            UpdateDisplay();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            InitializeRace();
        }
    }

    public class CarViewModel : INotifyPropertyChanged
    {
        public Car Car { get; }
        private RaceManager _raceManager;
        private string _status;

        public string Name => Car.Name;
        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public CarViewModel(Car car, RaceManager raceManager)
        {
            Car = car;
            _raceManager = raceManager;
            UpdateStatus();
        }

        public void UpdateStatus()
        {
            var lap = Math.Min(_raceManager.CarLaps[Car] + 1, _raceManager.Track.TotalLaps);
            var position = _raceManager.CarPositions[Car];
            Status = $"Lap {lap}/{_raceManager.Track.TotalLaps} | Speed: {Car.Speed:F1} | Fuel: {Car.Fuel:F1} | Pos: {position:F0}m";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}