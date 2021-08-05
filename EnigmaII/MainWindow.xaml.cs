using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EnigmaII
{ /*
    *Virtual Enigma is actual virtual copy of a german crypting machine, used before and mainly during the Second World War.
    *Scramblers, Rotors and all historical parts of the machine are included in its virtual state, except the light bulbs.
    * Physical connections are represented by integer hash maps
    * User custom settings are implemented, improving the strength to be almost "Unbreakable"
    * The real historical settings are not implemented, although might be by user himself via custom settings.
    * Slow crypting and mandatory knowledge of the key and the settings are huge price for its safety, thus unfitting for any commercial use nowadays. 
    */

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            Controller myController = new Controller();
            Coder myCoder = new Coder();
            View myView = new View(myController);

            myView.MyCanvas = myCanvas;
            myView.CreateLayout();
            myController.MyCoder = myCoder;
            myController.MyView = myView;
            myController.IntitialSettings();
            myController.SetDefaultSettings();
        }
    }

    /*
     * Creates layout of the programm and gives buttons, checkboxes, textboxes etc. their functionality
     * Functions:
     * CreateRadioButtons()
     * CreatePlugboard()
     * CreateEncryptingEditors()
     * CreateLayout()
     * 
     */

    public class View
    {
        public View(Controller myControl)
        {
            myController = myControl;
        }

        Canvas PlugboardWindowCanvas;
        Controller myController;

        Canvas myCanvas;
        public Canvas MyCanvas   // property
        {
            get { return myCanvas; }   // get method
            set { myCanvas = value; }  // set method
        }

        /// <summary>
        /// Generates RadioButtons and their functionalities
        /// </summary>
        private void CreateRadioButtons()
        {
            RadioButton defaultSettingsRdb = new RadioButton
            {
                Margin = new Thickness(1400, 50, 0, 0),
                Content = "Default settings",
                Name = "radioButtonDefault_rdibtn",
                IsChecked = true,
            };
            myCanvas.Children.Add(defaultSettingsRdb);

            RadioButton customSettingsRdb = new RadioButton
            {
                Margin = new Thickness(1400, 80, 0, 0),
                Content = "Custom settings",
                Name = "radioButtonCustom_rdibtn",
            };
            myCanvas.Children.Add(customSettingsRdb);
            myController.radioButtonForCustomSettings = customSettingsRdb;

            customSettingsRdb.Click += (sender, e) =>
            {
                myController.changeSettingsToCostum = true;
                myController.EnableOrDisableChangeSettingsButton();
            };

            defaultSettingsRdb.Click += (sender, e) =>
            {
                myController.changeSettingsToCostum = false;
                myController.EnableOrDisableChangeSettingsButton();
            };
        }

        /// <summary>
        /// Generates Plugboard and things along
        /// </summary>
        private void CreatePlugboard()
        {
            //SecondCanvas for plugboard
            Canvas PlugBoardWindow = new Canvas
            {
                Margin = new Thickness(1500, 720, 0, 0),
                Opacity = 1,
                Height = 600,
                Width = 400,
            };
            PlugboardWindowCanvas = PlugBoardWindow;
            myCanvas.Children.Add(PlugBoardWindow);

            TextBox ConnectionsForPlugboard = new TextBox
            {
                Margin = new Thickness(0, 0, 0, 0),
                Opacity = 1,
                Height = 50,
                Width = 230,
                Text = "",
                Name = "testTexbox_txtbox",
                IsEnabled = false
            };
            PlugboardWindowCanvas.Children.Add(ConnectionsForPlugboard);
            myController.connectionsForPlugboard = ConnectionsForPlugboard;

            int controlvariableForCreatingNamesOfplugboardButton = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (i == 3 && j == 5)
                    {
                        //Creates a delete button on plugboard
                        Button DeleteButtonPlugboard = new Button
                        {
                            Margin = new Thickness(j * 35, (i + 1) * 52, 0, 0),
                            Height = 20,
                            Width = 60,
                            Background = new SolidColorBrush(Colors.Red),
                            Content = "DELETE",

                        };
                        PlugboardWindowCanvas.Children.Add(DeleteButtonPlugboard);
                        DeleteButtonPlugboard.Click += (sender, e) =>
                        {
                            ConnectionsForPlugboard.Text = "";
                            foreach (Button btn in myController.KeysAtPlugboard)
                            {
                                btn.IsEnabled = true;
                            }
                        };
                        break;
                    }

                    //Creates  a butto for every letter so user can easily make connections betweenthem
                    Button TestbuttonForPlugboard = new Button
                    {
                        Margin = new Thickness(j * 35, (i + 1) * 52, 0, 0),
                        Height = 20,
                        Width = 20,
                        Background = new SolidColorBrush(Colors.White),
                        Content = (char)(controlvariableForCreatingNamesOfplugboardButton + 65),

                    };
                    PlugboardWindowCanvas.Children.Add(TestbuttonForPlugboard);
                    controlvariableForCreatingNamesOfplugboardButton++;
                    myController.KeysAtPlugboard.Add(TestbuttonForPlugboard);
                }
            }

            foreach (Button btn in myController.KeysAtPlugboard)
            {
                //function that will check whether another button is clicked, if so, create a pair and uncheck both
                btn.Click += (sender, e) =>
                {
                    btn.Background = new SolidColorBrush(Colors.Red);
                    if (myController.controlButtonForPlugBoard == null) myController.controlButtonForPlugBoard = btn;
                    else
                    {
                        ConnectionsForPlugboard.Text += " " + myController.controlButtonForPlugBoard.Content.ToString() + "|" + btn.Content.ToString() + " ";
                        myController.controlButtonForPlugBoard.IsEnabled = false;
                        myController.controlButtonForPlugBoard.Background = new SolidColorBrush(Colors.White);
                        myController.controlButtonForPlugBoard = null;
                        btn.IsEnabled = false;
                        btn.Background = new SolidColorBrush(Colors.White);
                    }
                };
            }
        }

        /// <summary>
        /// Generates Editors for messages and encrypt button
        /// </summary>
        private void CreateEncryptingEditors()
        {
            TextBox plainTextTextBox = new TextBox
            {
                Margin = new Thickness(300, 700, 0, 0),
                Opacity = 1,
                Height = 250,
                CharacterCasing = CharacterCasing.Upper,
                Width = 600,
                Text = "HELLOWORLD",
                Name = "testTexbox_txtbox"
            };
            myCanvas.Children.Add(plainTextTextBox);

            plainTextTextBox.TextChanged += (sender, e) =>
            {
                foreach (char letter in plainTextTextBox.Text.ToUpper())
                {
                    if ((int)letter > 90 || (int)letter < 64 && (int)letter != 32)
                    {
                        MessageBox.Show("Enter valid message to cypher");
                        plainTextTextBox.Text = "";
                    }
                }
            };

            TextBox cipherTextTextBox = new TextBox
            {
                Margin = new Thickness(300, 200, 0, 0),
                Opacity = 1,
                Height = 250,
                Width = 600,
                Text = "",
                Name = "testTexbox_txtbox"
            };
            myCanvas.Children.Add(cipherTextTextBox);

            Button ecryptButton = new Button
            {
                Margin = new Thickness(950, 700, 0, 0),
                Opacity = 1,
                Height = 250,
                Width = 250,
                FontSize = 30,
                Name = "testButton_btn",
                Content = "Encrypt",
            };
            myCanvas.Children.Add(ecryptButton);

            //onclic when the button "enrypct is clicked"
            ecryptButton.Click += (sender, e) =>
            {
                //function for setting a PlugboardHashMap
                myController.SetHashMapForPlugboard(myController.connectionsForPlugboard.Text);

                //sets the user selected rotation for the scramblers
                try
                {
                    int i = 0;
                    foreach (TextBox scramblerDefaultRotation in myController.startingPositionOfScramblers)
                    {
                        char[] c = scramblerDefaultRotation.Text.ToCharArray();
                        myController.defaultRotationValues[i] = ((int)c[0] - 65);
                        i++;
                    }
                    myController.MyCoder.MyRotor.RotateControlsToTheStartingPosition(myController.defaultRotationValues);
                }
                catch (Exception)
                {
                    MessageBox.Show("Enter valid rotation value in order to process the encryption!");
                    return;
                }

                //runs the encrypting system
                string final = "";
                foreach (char letter in plainTextTextBox.Text.Replace(" ", ""))
                {
                    final += (char)(myController.MyCoder.Coding((int)letter - 65) + 65);
                    myController.MyCoder.MyRotor.Rotate();
                    myController.RotateGUIEnigmaSetting();
                }
                cipherTextTextBox.Text = final;
            };
        }

        /// <summary>
        /// Calls "create" functions and generates rest of the labels, textboxes etc
        /// </summary>
        public void CreateLayout()
        {
            CreateRadioButtons();
            CreatePlugboard();
            CreateEncryptingEditors();

            Button createScramblerCustomHasMaps = new Button
            {
                Margin = new Thickness(1450, 300, 0, 0),
                Height = 150,
                Width = 300,
                Background = new SolidColorBrush(Colors.White),
                Content = "Change settings", // s textem zde se da trochu hrat podle toho, jake je zrovna nastaveni
                IsEnabled = false,
            };
            myCanvas.Children.Add(createScramblerCustomHasMaps);
            myController.changeSettingsButton = createScramblerCustomHasMaps;

            createScramblerCustomHasMaps.Click += (sender, e) =>
            {
                if (myController.changeSettingsToCostum == true)
                {
                    myController.win2 = new ScramblerMaps(myController);
                    myController.win2.InitializeComponent();
                    myController.win2.Show();
                }
                else myController.SetDefaultSettings();
            };

            Label canvasCaption = new Label
            {
                Margin = new Thickness(20, -50, 0, 0),
                Opacity = 1,
                Height = 100,
                Width = 230,
                FontSize = 30,
                Content = "PLUGBOARD",

                Name = "testTexbox_txtbox"
            };
            PlugboardWindowCanvas.Children.Add(canvasCaption);

            Label cipherTextCaption = new Label
            {
                Margin = new Thickness(450, 100, 0, 0),
                Opacity = 1,
                Height = 100,
                Width = 400,
                FontSize = 30,
                Content = "CIPHER TEXT EDITOR",

            };
            myCanvas.Children.Add(cipherTextCaption);


            Label plainTextCaption = new Label
            {
                Margin = new Thickness(450, 600, 0, 0),
                Opacity = 1,
                Height = 100,
                Width = 400,
                FontSize = 30,
                Content = "PLAIN TEXT EDITOR",

            };
            myCanvas.Children.Add(plainTextCaption);

            //Create rectangle borders - will be replaced with something later on (or not)
            Rectangle plaintextBoxBackground = new Rectangle
            {
                Margin = new Thickness(0, 550, 0, 0),
                Opacity = 1,
                Height = 450,
                Width = 1300,
                Name = "testRectangle_rct",
                Stroke = Brushes.Black,
                StrokeThickness = 4,
            };
            myCanvas.Children.Add(plaintextBoxBackground);

            Rectangle cipherTextBoxBackground = new Rectangle
            {
                Margin = new Thickness(0, 0, 0, 0),
                Opacity = 1,
                Height = 550,
                Width = 1300,
                Name = "testRectangle_rct",
                Stroke = Brushes.Black,
                StrokeThickness = 4,
            };
            myCanvas.Children.Add(cipherTextBoxBackground);

            Rectangle PlugboardBoxBackground = new Rectangle
            {
                Margin = new Thickness(1300, 550, 0, 0),
                Opacity = 1,
                Height = 450,
                Width = 600,
                Name = "testRectangle_rct",
                Stroke = Brushes.Black,
                StrokeThickness = 4,
            };
            myCanvas.Children.Add(PlugboardBoxBackground);

            Rectangle settingsBoxBackground = new Rectangle
            {
                Margin = new Thickness(1300, 0, 0, 0),
                Opacity = 1,
                Height = 550,
                Width = 600,
                Name = "testRectangle_rct",
                Stroke = Brushes.Black,
                StrokeThickness = 4,
            };
            myCanvas.Children.Add(settingsBoxBackground);

            

            Label rotorRotationCaprion = new Label
            {
                Margin = new Thickness(990, 570, 0, 0),
                Opacity = 1,
                Height = 50,
                Width = 250,
                FontSize = 30,
                Content = "ROTATIONS",
            };
            myCanvas.Children.Add(rotorRotationCaprion);
        }

        /// <summary>
        /// Generates boxes with letters, representing the current rotation of Enigma, may be changed by the user and their number depends on scramblers created by user
        /// </summary>
        /// <param name="numberOfWindowsToCreate"></param>
        public void CreateRotationWindows(int numberOfWindowsToCreate)
        {
            myController.startingPositionOfScramblers.Clear();

            for (int i = 0; i < numberOfWindowsToCreate; i++)
            {
                TextBox startingPointOfRotots = new TextBox
                {
                    Margin = new Thickness(950 + (i * 60), 630, 0, 0),
                    Opacity = 1,
                    Padding = new Thickness(5),
                    FontSize = 25,
                    CharacterCasing = CharacterCasing.Upper,
                    Height = 40,
                    Width = 40,
                    Text = "A",
                    MaxLength = 1,
                    Name = "testTexbox_txtbox"
                };
                MyCanvas.Children.Add(startingPointOfRotots);
                myController.startingPositionOfScramblers.Add(startingPointOfRotots);

                startingPointOfRotots.GotFocus += (sender, e) =>
                {
                    startingPointOfRotots.Text = "";
                };

                startingPointOfRotots.TextChanged += (sender, e) =>
                {
                    if (startingPointOfRotots.Text != "")
                    {
                        if ((int)startingPointOfRotots.Text[0] > 90 || (int)startingPointOfRotots.Text[0] < 65)
                        {
                            startingPointOfRotots.Text = "";
                            MessageBox.Show("please enter valid rotation (A - Z)!");
                        }
                    }
                };
            }
        }
    }

    /*
     * Main class of the program
     * Provides informatinos between objects, handles the settings, the current state of the program and all important events
     * FUNCTIONS: 
     * IntitialSettings()
     * SetDefaultSettings()
     * RotateGUIEnigmaSetting()
     * SetHashMapForPlugboard(string connections)
     * SetTheCostumSettings(ref List<int[,]> listOfCustomHashMapsFromUser)
     * EnableOrDisableChangeSettingsButton()
     * 
     */
    public class Controller
    {
        public Controller()
        {
        }

        Coder myCoder;
        public Coder MyCoder   // property
        {
            get { return myCoder; }   // get method
            set { myCoder = value; }  // set method
        }

        private List<char> listOrCharsToCode = new List<char>();
        public List<char> ListOrCharsToCode   // property
        {
            get { return listOrCharsToCode; }   // get method
            set { listOrCharsToCode = value; }  // set method
        }

        //Default Reflector map
        static int[,] defaultReflectMap = { { 1, 0 }, { 16, 11 }, { 17, 10 }, { 18, 20 }, { 21, 3 }, { 4, 7 }, { 25, 6 }, { 2, 5 }, { 23, 8 }, { 24, 13 }, { 14, 9 }, { 12, 15 }, { 19, 22 } };

        static int[,] plugboardHashMapForGUI;

        public ScramblerMaps win2; // second window with custom maps

        View myView;
        public View MyView   
        {
            get { return myView; }   // get method
            set { myView = value; }  // set method
        }

        //Bools and objects to help determine the state of Enigma's settings
        public bool usingDefaultSettings = true;
        public bool changeSettingsToCostum = false;
        public Button changeSettingsButton;
        public RadioButton radioButtonForCustomSettings;
        
        public List<int> defaultRotationValues = new List<int>();

        public TextBox connectionsForPlugboard;

        private List<Scrambler> defaultScramblers = new List<Scrambler>();

        public Button controlButtonForPlugBoard = null;

        public List<TextBox> startingPositionOfScramblers = new List<TextBox>(); //Works with rotor, determins wheteher rotation is neccesseary and sets its value via rotor functions

        public List<Button> KeysAtPlugboard = new List<Button>(); //list of all buttons representing letters on plugboard

        Reflector defaultReflector = new Reflector();
        Plugboard plugboard = new Plugboard();

        /// <summary>
        /// Non changable aspects of the program that need to only be instantiated once, other might be added if other features are wanted
        /// </summary>
        public void IntitialSettings()
        {
            myCoder.MyPlugboard = plugboard;
            myCoder.MyRotor = new Rotor();

            myCoder.MyRotor.MyCoder = myCoder;
        }


        /// <summary>
        /// Sets Enigma to its default settings every time you restart the project or manually after custom crypting
        /// </summary>
        public void SetDefaultSettings()
        {
            //Default settings of enigmas scramblers
             int[,] testHasMapDefault = { { 0, 16 }, { 1, 25 }, { 2, 14 }, { 3, 11 }, { 4, 15 }, { 5, 18 }, { 6, 22 }, { 7, 6 }, { 8, 10 }, { 9, 7 }, { 10, 4 }, { 11, 2 }, { 12, 9 }, { 13, 8 }, { 14, 0 }, { 15, 21 }, { 16, 17 }, { 17, 13 }, { 18, 1 }, { 19, 5 }, { 20, 24 }, { 21, 12 }, { 22, 23 }, { 23, 20 }, { 24, 19 }, { 25, 3 } };
             int[,] testHasMap2Default = { { 0, 16 }, { 1, 22 }, { 2, 3 }, { 3, 1 }, { 4, 21 }, { 5, 7 }, { 6, 0 }, { 7, 11 }, { 8, 13 }, { 9, 2 }, { 10, 9 }, { 11, 4 }, { 12, 15 }, { 13, 10 }, { 14, 12 }, { 15, 19 }, { 16, 23 }, { 17, 18 }, { 18, 14 }, { 19, 5 }, { 20, 25 }, { 21, 24 }, { 22, 6 }, { 23, 17 }, { 24, 8 }, { 25, 20 } };
             int[,] testHasMap3Default = { { 0, 15 }, { 1, 21 }, { 2, 2 }, { 3, 0 }, { 4, 20 }, { 5, 6 }, { 6, 25 }, { 7, 10 }, { 8, 12 }, { 9, 1 }, { 10, 8 }, { 11, 3 }, { 12, 14 }, { 13, 9 }, { 14, 11 }, { 15, 18 }, { 16, 22 }, { 17, 17 }, { 18, 13 }, { 19, 4 }, { 20, 24 }, { 21, 23 }, { 22, 5 }, { 23, 16 }, { 24, 7 }, { 25, 19 } };


            //Instantiate Reflector with its map
            myCoder.MyReflector = defaultReflector;
            defaultReflector.Hashmap = defaultReflectMap;

            defaultRotationValues.Clear();
            defaultRotationValues.Add(0);
            defaultRotationValues.Add(0);
            defaultRotationValues.Add(0);

            //Initialize default scramblers
            List<Scrambler>  workScramblers = new List<Scrambler>();
            Scrambler scrambler1 = new Scrambler(testHasMapDefault);
            Scrambler scrambler2 = new Scrambler(testHasMap2Default);
            Scrambler scrambler3 = new Scrambler(testHasMap3Default);
            workScramblers.Add(scrambler1); workScramblers.Add(scrambler2); workScramblers.Add(scrambler3);

            myCoder.MyRotor.MyScramblers = workScramblers;
            myCoder.MyRotor.ControllVariablesForRoation = new List<int>(defaultRotationValues);

            myCoder.MyScramblers = new List<Scrambler>(workScramblers);

            usingDefaultSettings = true; 
            EnableOrDisableChangeSettingsButton();

            //Create a rotation box
            myView.CreateRotationWindows(3);
        }

        /// <summary>
        /// //"Rotates" the setting of enigma in GUI, so the user knows what setting is he at
        /// </summary>
        public void RotateGUIEnigmaSetting()
        {
            int i = 0;
            foreach (TextBox txtbox in startingPositionOfScramblers)
            {
                txtbox.Text = ((char)(myCoder.MyRotor.controllVariablesForRoation[i] + 65)).ToString();
                i++;
            }
        }

        /// <summary>
        /// Creates and sets the map to plugboard depending on what conncetions user chose 
        /// </summary>
        /// <param name="connections"></param>
        public void SetHashMapForPlugboard(string connections)
        {
            string lettersForPlugBoardHasMap = "";
            foreach (char character in connections)
            {
                if ((int)character >= 65 && (int)character <= 90) lettersForPlugBoardHasMap += character.ToString();
            }
            plugboardHashMapForGUI = new int[(lettersForPlugBoardHasMap.Length) / 2, 2];

            //Creates the Hashmap
            int controlCounter = 0;
            for (int i = 0; i < (lettersForPlugBoardHasMap.Length) / 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    plugboardHashMapForGUI[i, j] = ((int)lettersForPlugBoardHasMap[controlCounter]) - 65;
                    controlCounter++;
                }
            }
            MyCoder.MyPlugboard.PlugBoardHasmap = plugboardHashMapForGUI;
        }

        /// <summary>
        ///  Sets the enigma to the custom settings
        /// </summary>

        /* 
        * The ref list of int[,] is sent from second window, there the custom hasmaps are created
        * Create corresponding number of Scramblers, list<int> with corresponding number of rotations
        * Initialize new rotor and feed it with the Scrambler list
        * Remove previously used rotationBoxes and generates correspoding number of new ones
        * Sets the state of using to custom
        * 
        *************/
        public void SetTheCostumSettings(ref List<int[,]> listOfCustomHashMapsFromUser)
        {
            List<Scrambler> myCostumScramblerList = new List<Scrambler>();
            List<int> defaultRotationValuesCustom = new List<int>();

            for (int i = 0; i < listOfCustomHashMapsFromUser.Count; i++)
            {
                defaultRotationValuesCustom.Add(0);
                Scrambler myScrambler = new Scrambler(listOfCustomHashMapsFromUser[i]);
                myCostumScramblerList.Add(myScrambler);
            }

            myCoder.MyRotor.MyScramblers = new List<Scrambler>(myCostumScramblerList);
            myCoder.MyRotor.controllVariablesForRoation = new List<int>(defaultRotationValuesCustom);
            win2.Close();
            
            foreach (TextBox txtbox in startingPositionOfScramblers)
            {
                myView.MyCanvas.Children.Remove(txtbox);
            }
            defaultRotationValues = new List<int>(defaultRotationValuesCustom);


            //generates the correct number of rotation windows
            myView.CreateRotationWindows(listOfCustomHashMapsFromUser.Count);

            myCoder.MyScramblers = new List<Scrambler>(myCostumScramblerList);

            usingDefaultSettings = false;
        }

        /// <summary>
        /// //Checks whether it is necesseary to allow user to change settings (It is not when default state is wanted and also being used)
        /// </summary>
        public void EnableOrDisableChangeSettingsButton()
        {
            if (usingDefaultSettings == true && changeSettingsToCostum == false) changeSettingsButton.IsEnabled = false;
            else changeSettingsButton.IsEnabled = true;
        }
    }


    /*
     * Responsible for all the coding operations
     * FUNCTIONS:
     * Coding(int charToCdoe)
     * CodingForward(int codedChar)
     * GetReflectedCharFromReflector(int charToReflect)
     * CodingBackbward(int codedChar)
     * CLASSES:
     * Rotor
     * Reflector
     * Plugboard
     * Scrambler
     */
    public class Coder
    {
        public Coder()
        {
        }

        Rotor myRotor;
        public Rotor MyRotor   // property
        {
            get { return myRotor; }   // get method
            set { myRotor = value; }  // set method
        }

        Reflector myReflector;
        public Reflector MyReflector   // property
        {
            get { return myReflector; }   // get method
            set { myReflector = value; }  // set method
        }

        Plugboard myPlugboard;
        public Plugboard MyPlugboard   // property
        {
            get { return myPlugboard; }   // get method
            set { myPlugboard = value; }  // set method
        }

        public List<Scrambler> myScramblers;
        public List<Scrambler> MyScramblers   // property
        {
            get { return myScramblers; }   // get method
            set { myScramblers = value; }  // set method
        }


        public int charToEncode;

        /// <summary>
        /// Main coding function, wrapping all the functions together
        /// </summary>
        /// <param name="charToCdoe"></param>
        /// <returns> encrypted Letter in integer form </returns>
        public int Coding(int charToCdoe)
        {
            int toReturn = charToCdoe;
            toReturn = myPlugboard.ChechAndReturnPlugBoard(toReturn);
            toReturn = CodingForward(toReturn);
            toReturn = GetReflectedCharFromReflector(toReturn);
            toReturn = CodingBackbward(toReturn);
            toReturn = myPlugboard.ChechAndReturnPlugBoard(toReturn);
            return toReturn;
        }

        /// <summary>
        /// Encrypted integer follows the maps of all scramblers (Scrambler(1) - Scrambler(n))
        /// </summary>
        /// <param name="codedChar"></param>
        /// <returns> integer to reflector</returns>
        public int CodingForward(int codedChar)
        {
            int encodedChar = codedChar;
            foreach (Scrambler myScrambler in myScramblers)
            {
                encodedChar = myScrambler.returnWantedInt(encodedChar, true);
            }
            return encodedChar;
        }

        /// <summary>
        /// Connects integer in parametr to its corresponding pair via reflector map
        /// </summary>
        /// <param name="charToReflect"></param>
        /// <returns>integer for coding backwards</returns>
        public int GetReflectedCharFromReflector(int charToReflect)
        {
            return myReflector.ReflectEncodedChar(charToReflect);
        }

        /// <summary>
        /// Encrypted integer follows the maps of all scramblers in reverse direction from reflctor (Scrambler(n) - Scrambler(1))
        /// </summary>
        /// <param name="codedChar"></param>
        /// <returns>integer to be checked on plugboard </returns>
        public int CodingBackbward(int codedChar)
        {
            int encodedChar = codedChar;
            for (int i = myScramblers.Count - 1; i >= 0; i--)
            {
                encodedChar = myScramblers[i].returnWantedInt(encodedChar, false);
            }
            return encodedChar;
        }
    }

    /*
     * Let user pair letters (up to 13 pairs). While crypting/encrypting, if the char was paired on plugboard, send the other char from the pair to be crypted instead.
     * FUNCTIONS:
     * ChechAndReturnPlugBoard(int charToCheck)
     */

    public class Plugboard
    {
        public Plugboard()
        {
        }

        int[,] plugBoardHasmap;
        public int[,] PlugBoardHasmap   // property - integers randomly connected into pairs
        {
            get { return plugBoardHasmap; }   // get method
            set { plugBoardHasmap = value; }  // set method
        }
        
        /// <summary>
        /// First and final step of coding, swaping enrypted integer with its corresponding pair via plugboard map, if such integer is found
        /// </summary>
        /// <param name="charToCheck"></param>
        /// <returns>
        /// 1) the same integer if it was not found on the plugboard map
        /// 2) swapped integer for full encryption when call at the beggining of enrypting
        /// 3) final, encrypted product if called at the end of coding function
        /// </returns>
        public int ChechAndReturnPlugBoard(int charToCheck)
        {
            for (int i = 0; i < plugBoardHasmap.GetLength(0); i++)
            {
                for (int j = 0; j < plugBoardHasmap.GetLength(1); j++)
                {
                    if (charToCheck == plugBoardHasmap[i, j])
                    {
                        if (j == 0) return plugBoardHasmap[i, 1];
                        else return plugBoardHasmap[i, j - 1];
                    }
                }
            }
            return charToCheck;
        }
    }

    /*
     * Handles rotation events. Checks the positions of scramblers, changes into rotation done by user and adapt rotation controls.
     * FUNCTIONS:
     * RotateControlsToTheStartingPosition(List<int> settingOfEnigma)
     * Rotate()
     * RotateControlls()
     * RotateScramblers(int numberOfScramblersToRotate)
     */
    public class Rotor
    {
        public Rotor()
        {
        }

        Coder myCoder;
        public Coder MyCoder   // property
        {
            get { return myCoder; }   // get method
            set { myCoder = value; }  // set method
        }

        public List<int> controllVariablesForRoation;
        public List<int> ControllVariablesForRoation   // property
        {
            get { return controllVariablesForRoation; }   // get method
            set { controllVariablesForRoation = value; }  // set method
        }

        List<Scrambler> myScramblers;
        public List<Scrambler> MyScramblers   // property
        {
            get { return myScramblers; }   // get method
            set { myScramblers = value; }  // set method
        }

        int numberOfScramblersToRotate = 0;

        /// <summary>
        /// Checks whether user changed the rotation values
        /// 
        /// 1) No - encryption will continue with the lastly used rotation
        /// 2) Yes - this function determines how it changed
        /// </summary>
        /// <param name="settingOfEnigma"></param>
        public void RotateControlsToTheStartingPosition(List<int> settingOfEnigma)
        {
            for (int i = 0; i < controllVariablesForRoation.Count; i++)
            {
                for (int j = 0; j < myScramblers[i].hashMap.GetLength(0); j++)
                {
                    myScramblers[i].hashMap[j, 0] = (myScramblers[i].hashMap[j, 0] + ((settingOfEnigma[i] - controllVariablesForRoation[i] + 26) % 26)) % 26;
                }
                controllVariablesForRoation[i] = settingOfEnigma[i];
            }
        }

        public void Rotate()
        {
            RotateControlls();
            RotateScramblers(numberOfScramblersToRotate);
        }

        /// <summary>
        /// Control function for checking how much each scrambler has rotated and whether it is necesseary to rotate the next one as well: (Z,A,A) -> (A,B,A)
        /// </summary>
        private void RotateControlls()
        {
            for (int i = 0; i < controllVariablesForRoation.Count; i++)
            {
                if (controllVariablesForRoation[i] > 24) controllVariablesForRoation[i] = 0;
                else
                {
                    controllVariablesForRoation[i] += 1;
                    numberOfScramblersToRotate = i;
                    break;
                }
                numberOfScramblersToRotate = i;
            }
        }

        /// <summary>
        /// Rotate the scramblers counter-clockwise
        /// </summary>
        /// <param name="numberOfScramblersToRotate"></param>
        private void RotateScramblers(int numberOfScramblersToRotate)
        {
            for (int i = 0; i <= numberOfScramblersToRotate; i++)
            {
                for (int j = 0; j < myScramblers[i].hashMap.GetLength(0); j++)
                {
                    myScramblers[i].hashMap[j, 0] = (myScramblers[i].hashMap[j, 0] + 1) % 26;
                }
            }
        }
    }

    /*
     * Contains the hash maps, the actual encrypting is happening here.
     * FUNCTIONS:
     * returnWantedInt(int intToEncrypt, bool codingForward)
     */
    public class Scrambler
    {

        public Scrambler(int[,] connectionHashmap)
        {
            hashMap = connectionHashmap;
        }

        public int[,] hashMap = new int[26, 2];

        /// <summary>
        /// Finds corresponding integer on scrambler map and returns it - the actually crypting of letters
        /// </summary>
        /// <param name="intToEncrypt"></param>
        /// <param name="codingForward"></param>
        /// <returns>
        /// Forward: integer for reflector
        /// Backwards: Integer for plugboard check
        /// </returns>
        public int returnWantedInt(int intToEncrypt, bool codingForward)
        {
            if (codingForward == true)
            {
                int counter = 0;
                int toReturn = intToEncrypt;
                while (toReturn == intToEncrypt)
                {
                    if (hashMap[counter, 0] == toReturn)
                    {
                        toReturn = hashMap[counter, 1];
                        break;
                    }
                    counter++;
                }
                return toReturn;
            }
            else
            {
                int counter = 0;
                int toReturn = intToEncrypt;
                while (toReturn == intToEncrypt)
                {
                    if (hashMap[counter, 1] == toReturn)
                    {
                        toReturn = hashMap[counter, 0];
                        break;
                    }
                    counter++;
                }
                return toReturn;
            }
        }
    }

    /*
     * Contains map, connecting 26 letters into pairs. When Integer gets here, it is exchanged for its "twin" and sends back through the scramblers.
     * This enables the 
     * FUNCTIONS:
     * ReflectEncodedChar(int toReflect)
     */
    public class Reflector
    {
        public Reflector()
        {
        }

        int[,] reflectorHashmap;
        public int[,] Hashmap   // property - 26 integers randomly connected into pairs
        {
            get { return reflectorHashmap; }   // get method
            set { reflectorHashmap = value; }  // set method
        }

        /// <summary>
        /// Finds corresponding integer on ReflectorMap and returns it 
        /// f.e ->   {1,5},{4,20} > ReflectEncodedChar(5) -> returns 1;
        /// </summary>
        /// <param name="toReflect"></param>
        /// <returns>corresponding integer on reflector map</returns>
        public int ReflectEncodedChar(int toReflect)
        {
            int reflected = 0;
            for (int i = 0; i < reflectorHashmap.GetLength(0); i++)
            {
                for (int j = 0; j < reflectorHashmap.GetLength(1); j++)
                {
                    if (reflectorHashmap[i, j] == toReflect)
                    {
                        if (j == 0) reflected = reflectorHashmap[i, j + 1];
                        else reflected = reflectorHashmap[i, j - 1];
                    }
                }
            }
            return reflected;
        }
    }
}
