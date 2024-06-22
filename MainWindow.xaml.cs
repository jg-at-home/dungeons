using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Xml;

namespace DragonsAndDungeons
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Ideally done ith MVVM but limited time, so straight to direct methods.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public List<Race> Races => races_;
        public List<Class> Classes => classes_;

        public Character FindCharacterWithName(string name)
        {
            return characters_.Find(c => c.Name == name);
        }

        private void characterList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedCharacter = (Character)characterList.SelectedItem;
            nameText.Text = selectedCharacter.Name;
            raceText.Text = selectedCharacter.Race.Name;
            classText.Text = selectedCharacter.Class.Name;
            moneyText.Text = selectedCharacter.Gold.ToString();
            statsText.Text = selectedCharacter.StatString;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            loadConfig();
            loadCharacters();
        }

        private void loadConfig()
        {
            var xmlConfig = new XmlDocument();
            xmlConfig.Load("Data/config.xml");
            var dataNode = xmlConfig.SelectSingleNode("data");
            readClasses(dataNode);
            readRaces(dataNode);
            readStats(dataNode);
            readConMods(dataNode);
        }

        private void readConMods(XmlNode dataNode)
        {
            var modElement = (XmlElement)dataNode.SelectSingleNode("CON_mods");
            var modListStr = modElement.InnerText;
            var modList = modListStr.Split(',');
            if (modList.Length != maxConMods) {
                throw new Exception("Invalid CON_mod count");
            }
            for(var i = 0; i < maxConMods; ++i) {
                conMods_[i] = Convert.ToInt32(modList[i]);
            }
        }

        private void readStats(XmlNode dataNode)
        {
            var statsList = dataNode.SelectNodes("stats/stat");
            foreach(var statNode in statsList) {
                var statElement = (XmlElement)statNode;
                var statInfo = new StatInfo();
                statInfo.Name = statElement.GetAttribute("name");
                statInfo.Abbreviation = statElement.GetAttribute("abbrev");
                Character.statInfo.Add(statInfo);
            }
        }

        private void readClasses(XmlNode dataNode)
        {
            var classListNode = dataNode.SelectNodes("classes/class");
            foreach(var classNode in classListNode) {
                var classElement = (XmlElement)classNode;
                var @class = new Class();
                @class.Name = classElement.GetAttribute("name");
                @class.DiceRoller = Utils.StringToDiceRoller(classElement.GetAttribute("hitDice"));
                @class.PrimaryStat = classElement.GetAttribute("primeStat");
                classes_.Add(@class);
            }
        }

        private void readRaces(XmlNode dataNode)
        {
            var racesListNode = dataNode.SelectNodes("races/race");
            foreach (var raceNode in racesListNode) {
                var race = new Race();
                var raceElement = (XmlElement)raceNode;
                race.Name = raceElement.GetAttribute("name");

                var allowedClassesStr = raceElement.GetAttribute("classes");
                if (allowedClassesStr == "*") {
                    foreach (var @class in classes_) {
                        race.AllowedClasses.Add(@class);
                    }
                }
                else {
                    var allowedClasses = allowedClassesStr.Split(',');
                    foreach (var allowedClassStr in allowedClasses) {
                        var className = allowedClassStr.Trim();
                        var @class = classes_.Find(c => c.Name == className);
                        Debug.Assert(@class != null);
                        race.AllowedClasses.Add(@class);
                    }
                }

                if (raceElement.HasAttribute("minStats")) {
                    var minStatStr = raceElement.GetAttribute("minStats");
                    var statAndValueList = Utils.GetStatAndValueList(minStatStr);
                    foreach(var statAndValue in statAndValueList) {
                        race.MinStats[statAndValue.Key] = statAndValue.Value;
                    }
                }

                if (raceElement.HasAttribute("mods")) {
                    var modListStr = raceElement.GetAttribute("mods");
                    var modList = Utils.GetStatAndValueList(modListStr);
                    foreach(var nameAndMod in modList) {
                        race.StatMods[nameAndMod.Key] = nameAndMod.Value;
                    }
                }

                if (raceElement.HasAttribute("ability")) {
                    var abilityStr = raceElement.GetAttribute("ability");
                    race.Ability = Enum.Parse<Ability>(abilityStr);
                }
                races_.Add(race);
            }
        }

        private void loadCharacters()
        {
            characterList.Items.Clear();
            var characterFiles = Directory.GetFiles("data/characters/", "*.xml");
            foreach(var characterFile in characterFiles) {
                loadCharacter(characterFile);
            }

            characterList.SelectedIndex = 0;
        }

        private void loadCharacter(string filename)
        {
            var xmlCharacter = new XmlDocument();
            xmlCharacter.Load(filename);
            var characterElement = (XmlElement)xmlCharacter.SelectSingleNode("character");
            var name = characterElement.GetAttribute("name");
            var raceStr = characterElement.GetAttribute("race");
            var race = races_.Find(r => r.Name == raceStr);
            var classStr = characterElement.GetAttribute("class");
            var @class = classes_.Find(c => c.Name == classStr);
            var character = new Character(name, race, @class);
            var goldStr = characterElement.GetAttribute("money");
            character.Gold = Convert.ToInt32(goldStr);
            characters_.Add(character);
            characterList.Items.Add(character);
            var statsList = characterElement.SelectNodes("stats/stat");
            foreach(var statNode in statsList) {
                var statElement = (XmlElement)statNode;
                var statName = statElement.GetAttribute("abbrev");
                var stat = character.GetStat(statName);
                var valueStr = statElement.GetAttribute("value");
                stat.BaseValue = Convert.ToInt32(valueStr);
                var modStr = statElement.GetAttribute("mods");
                stat.AddModifier(Convert.ToInt32(modStr));
            }
        }

        private void saveCharacter(Character character)
        {
            var xmlCharacter = new XmlDocument();
            var characterElement = xmlCharacter.CreateElement("character");
            xmlCharacter.AppendChild(characterElement);
            characterElement.SetAttribute("name", character.Name);
            characterElement.SetAttribute("race", character.Race.Name);
            characterElement.SetAttribute("class", character.Class.Name);
            characterElement.SetAttribute("money", character.Gold.ToString());
            var statsElement = xmlCharacter.CreateElement("stats");
            characterElement.AppendChild(statsElement);
            foreach(var stat in character.Stats) {
                var statElement = xmlCharacter.CreateElement("stat");
                statsElement.AppendChild(statElement);
                statElement.SetAttribute("abbrev", stat.Abbreviation);
                statElement.SetAttribute("value", stat.BaseValue.ToString());
                statElement.SetAttribute("mods", stat.Modifiers.ToString());
            }

            var fileName = character.Name.Replace(' ', '_');
            var filePath = $"data/characters/{fileName}.xml";
            xmlCharacter.Save(filePath);
        }

        private const int maxConMods = 18;
        private List<Race> races_ = new ();
        private List<Class> classes_ = new();
        private List<Character> characters_ = new();
        private int [] conMods_ = new int [maxConMods];

        private void NewCharacterItem_Click(object sender, RoutedEventArgs e)
        {
            var ncd = new NewCharacterDialog(this) {
                Owner = this
            };
            if (ncd.ShowDialog() == true) {
                var characterRace = ncd.CharacterRace;
                var characterClass = ncd.CharacterClass;
                var name = ncd.CharacterName;
                var character = new Character(name, characterRace, characterClass);
                characters_.Add(character);

                var generalRoller = Utils.StringToDiceRoller("3d6");
                foreach (var stat in character.Stats) {
                    var roller = (stat.Name == "HP") ? characterClass.DiceRoller : generalRoller;
                    stat.BaseValue = roller.Roll();
                    var mod = characterRace.GetModForStat(stat.Abbreviation);
                    if (mod != 0) {
                        stat.AddModifier(mod);
                    }

                    var minValue = characterRace.GetMinForStat(stat.Abbreviation);
                    stat.ClampToMin(minValue);
                }

                character.Gold = 10 * generalRoller.Roll();

                // Apply HP CON mod.
                var conStat = character.GetStat("CON");
                var conValue = conStat.Value;
                var conIndex = Math.Clamp(conValue, 1, maxConMods)-1;
                var conMod = conMods_[conIndex];
                if (conMod != 0) {
                    var hpStat = character.GetStat("HP");
                    hpStat.AddModifier(conMod);
                    var minValue = Math.Max(1, characterRace.GetMinForStat("HP"));
                    hpStat.ClampToMin(minValue);
                }

                characterList.Items.Add(character);
                characterList.SelectedItem = character;
                saveCharacter(character);
            }
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
