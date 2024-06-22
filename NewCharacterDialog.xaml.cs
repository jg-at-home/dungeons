using System;
using System.Windows;
using System.Windows.Controls;

namespace DragonsAndDungeons
{
    /// <summary>
    /// Interaction logic for NewCharacterDialog.xaml
    /// </summary>
    public partial class NewCharacterDialog : Window
    {
        public NewCharacterDialog(MainWindow mainWindow)
        {
            mainWindow_ = mainWindow;
            InitializeComponent();
        }

        public string CharacterName => nameText.Text;
        public Class CharacterClass => classCombo.SelectedItem as Class;
        public Race CharacterRace => raceCombo.SelectedItem as Race;

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void nameText_TextChanged(object sender, TextChangedEventArgs e)
        {
            validate();
        }

        private void validate()
        {
            okButton.IsEnabled = validateName(nameText.Text);
        }

        private bool validateName(string name)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name)) {
                return false;
            }

            if (mainWindow_.FindCharacterWithName(name) != null) {
                return false;
            }

            return true;
        }

        private void raceCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var race = (Race)raceCombo.SelectedItem;
            updateClassListForRace(race);
        }

        private void classCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedClass = classCombo.SelectedItem as Class;
            if (selectedClass != null) {
                primaryStatText.Text = selectedClass.PrimaryStat;
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            foreach(var race in mainWindow_.Races) {
                raceCombo.Items.Add(race);
            }

            raceCombo.SelectedIndex = 0;
            validate();           
        }

        void updateClassListForRace(Race race)
        {
            // Try to keep the current class if it's valid for the race.
            var currentClass = classCombo.SelectedItem as Class;
            var newIndex = 0;

            var index = 0;
            classCombo.Items.Clear();
            foreach(var @class in race.AllowedClasses) {
                classCombo.Items.Add(@class);
                if (@class == currentClass) {
                    newIndex = index;
                }
                ++index;
            }

            classCombo.SelectedIndex = newIndex;
        }

        // Ideally: would just pass references to a Data Model rather than a window around.
        private MainWindow mainWindow_;
    }
}
