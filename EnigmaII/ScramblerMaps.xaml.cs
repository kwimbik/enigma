using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EnigmaII
{
    /// <summary>
    /// Second window, used for creatin costum hash maps - scramblers by user
    /// </summary>
    public partial class ScramblerMaps : Window
    {
        public ScramblerMaps(Controller mainController)
        {
            InitializeComponent();
            ViewII newView = new ViewII();
            ControllerII myController = new ControllerII();

            
            newView.MyController = myController;
            myController.MyViewII = newView;

            myController.MainController = mainController;
            newView.CanvasForScramblerWindow = scramblerCanvas_cvs;
            newView.CreateScramblerMapsLayout();
        }

        /*
         * Creates layout of the second window and gives controls its functionalities
         * FUNCTIONS:
         * CreateScramblerMapsLayout()
         * private void ClearPreviousHashMaps()
         * GenerateScramblerTextBoxes(int numberOfTextBoxesToCreate)
         * 
         */

        public class ViewII
        {
            Canvas canvasForScramblerWindow;
            public Canvas CanvasForScramblerWindow   // property
            {
                get { return canvasForScramblerWindow; }   // get method
                set { canvasForScramblerWindow = value; }  // set method
            }

            ControllerII myController;
            public ControllerII MyController   // property
            {
                get { return myController; }   // get method
                set { myController = value; }  // set method
            }

            public List<List<Button>> listOfButtonLists = new List<List<Button>>();
            public List<List<Button>> listOfButtonListsII = new List<List<Button>>();


            List<Button> listOfScramblerMaps = new List<Button>();
            List<Button> listOfScramblerMapsII = new List<Button>();
            Button controlButtonForHashMaps = null;

             public List<TextBox> hashMapxTextBoxes = new List<TextBox>();

            public void CreateScramblerMapsLayout()
            {
                Label ConnectionsForPlugboard = new Label
                {
                    Margin = new Thickness(50, 50, 0, 0),
                    Opacity = 1,
                    Height = 40,
                    Width = 250,
                    Content = "How many scramblers do you wish to use",
                    Name = "numberOfScramblersToUse",
                    IsEnabled = false,
                };
                CanvasForScramblerWindow.Children.Add(ConnectionsForPlugboard);

                TextBox numberorScrablers = new TextBox
                {
                    Margin = new Thickness(50, 100, 0, 0),
                    Opacity = 1,
                    Height = 40,
                    Width = 50,
                    Text = "3",
                    Name = "numberOfScramblersToUse",
                    IsEnabled = false,
                };
                CanvasForScramblerWindow.Children.Add(numberorScrablers);


                ScrollBar scrlbar = new ScrollBar
                {
                    Margin = new Thickness(110, 100, 0, 0),
                    Height = 40,
                    Width = 20,
                    Value = 3,
                    SmallChange = 1,
                    Maximum = 8,
                    Minimum = 1,
                    IsEnabled = true,
                    Orientation = Orientation.Vertical,
                    
                };
                CanvasForScramblerWindow.Children.Add(scrlbar);

                scrlbar.ValueChanged += (sender, e) =>
                {
                    numberorScrablers.Text = scrlbar.Value.ToString();
                };


                Button confirmButton = new Button
                {
                    Margin = new Thickness(250, 150, 0, 0),
                    Opacity = 1,
                    Height = 50,
                    Width = 180,
                    Content = "Confirm settings",
                    Background = Brushes.Green,
                    IsEnabled = false,
                };
                CanvasForScramblerWindow.Children.Add(confirmButton);

                myController.ConfirmButtonForHashMapSetting = confirmButton;

                //Create parsing method for parsing the maps
                //Closes the window, resets the variables
                confirmButton.Click += (sender, e) =>
                {
                    myController.SetupTheCostumModeEnigmaSettings();
                };

                Button addScramblerButton = new Button
                {
                    Margin = new Thickness(50, 150, 0, 0),
                    Opacity = 1,
                    Height = 50,
                    Width = 180,
                    Content = "OK",
                    IsEnabled = true,
                };
                CanvasForScramblerWindow.Children.Add(addScramblerButton);

                // Onclic will generate desired number of user-friendly and fully costumizable hash maps
                addScramblerButton.Click += (sender, e) =>
                {
                    ClearPreviousHashMaps();

                    myController.indexOfCurrentScramblerMap = 0;
                    
                    for (int i = 0; i < int.Parse(numberorScrablers.Text); i++)
                    {
                        listOfButtonLists.Add(new List<Button>());
                        listOfButtonListsII.Add(new List<Button>());
                    }
                    GenerateScmrablerButtons(int.Parse(numberorScrablers.Text));
                    GenerateScramblerTextBoxes(int.Parse(numberorScrablers.Text));
                    myController.EnableButtons();
                };
            }

            /// <summary>
            /// Clears all lists and canvas
            /// </summary>
            private void ClearPreviousHashMaps()
            {
                foreach (TextBox txt in hashMapxTextBoxes)
                {
                    CanvasForScramblerWindow.Children.Remove(txt);
                }

                foreach (List<Button> btnlist in listOfButtonLists)
                {
                    foreach (Button btn in btnlist)
                    {
                        CanvasForScramblerWindow.Children.Remove(btn);
                    }
                }
                foreach (List<Button> btnlist in listOfButtonListsII)
                {
                    foreach (Button btn in btnlist)
                    {
                        CanvasForScramblerWindow.Children.Remove(btn);
                    }
                }

                hashMapxTextBoxes.Clear();
                listOfButtonLists.Clear();
                listOfButtonListsII.Clear();
            }

            /// <summary>
            /// Generates text boxes next to the settings of hashmaps, so user sees the connections he/she created
            /// </summary>
            /// <param name="numberOfTextBoxesToCreate"></param>
            public void GenerateScramblerTextBoxes(int numberOfTextBoxesToCreate)
            {
                for (int i = 0; i < numberOfTextBoxesToCreate; i++)
                {
                    TextBox hashMaps = new TextBox
                    {
                        Margin = new Thickness(90 + i * 150, 223, 0, 0),
                        Opacity = 1,
                        Height = 500,
                        Width = 80,
                        Text = "",
                        Name = "",
                        IsEnabled = false,
                    };
                    CanvasForScramblerWindow.Children.Add(hashMaps);
                    hashMapxTextBoxes.Add(hashMaps);

                    hashMaps.TextChanged += (sender, e) =>
                    {
                        if (hashMaps.Text.Length > 197)
                        {
                            myController.indexOfCurrentScramblerMap++;
                            myController.EnableButtons();
                        }
                    };
                }
            }

            /// <summary>
            /// Generates  the customizable scramble maps via buttons, interactive on clic, user can connect one from the left and one from the right column
            /// </summary>
            /// <param name="numberOfScramblermapsToCreate"></param>
            public void GenerateScmrablerButtons(int numberOfScramblermapsToCreate)
            {
                for (int controlvar = 0; controlvar < numberOfScramblermapsToCreate; controlvar++)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 26; j++)
                        {
                            if (i == 0)
                            {
                                Button TestbuttonForPlugboard = new Button
                                {
                                    Margin = new Thickness(((i + 1) * 25) + (controlvar * 150), ((j + 1) * 23) + 200, 0, 0),
                                    Height = 20,
                                    Width = 20,
                                    Background = new SolidColorBrush(Colors.Green),
                                    Content = j.ToString(),
                                    FontSize = 10,
                                    IsEnabled = false,
                                    
                                };
                                CanvasForScramblerWindow.Children.Add(TestbuttonForPlugboard);
                                listOfButtonLists[controlvar].Add(TestbuttonForPlugboard);

                                /*Onclic logic, allowing you to only connect  pair from opposite columns
                                 * Removing already used buttons
                                 */
                                TestbuttonForPlugboard.Click += (sender, e) =>
                                {
                                    foreach (Button btn in listOfButtonLists[myController.indexOfCurrentScramblerMap])
                                    {
                                        btn.IsEnabled = false;
                                    }

                                    TestbuttonForPlugboard.IsEnabled = true;
                                    TestbuttonForPlugboard.Background = new SolidColorBrush(Colors.Red);
                                    controlButtonForHashMaps = TestbuttonForPlugboard;

                                    listOfButtonLists[myController.indexOfCurrentScramblerMap].Remove(TestbuttonForPlugboard);
                                    CanvasForScramblerWindow.Children.Remove(TestbuttonForPlugboard);

                                    foreach (Button btn in listOfButtonListsII[myController.indexOfCurrentScramblerMap])
                                    {
                                        btn.IsEnabled = true;
                                    }
                                };
                            }
                            else
                            {
                                Button TestbuttonForPlugboardII = new Button
                                {
                                    Margin = new Thickness(((i + 1) * 25) + (controlvar * 150), ((j + 1) * 23) + 200, 0, 0),
                                    Height = 20,
                                    Width = 20,
                                    Background = new SolidColorBrush(Colors.Green),
                                    Content = (char)(j + 65),
                                    FontSize = 10,
                                    IsEnabled = false,
                                };
                                CanvasForScramblerWindow.Children.Add(TestbuttonForPlugboardII);
                                listOfButtonListsII[controlvar].Add(TestbuttonForPlugboardII);

                                /*Onclic logic, allowing you to only connect  pair from opposite columns
                                 * Removing already used buttons
                                 */
                                TestbuttonForPlugboardII.Click += (sender, e) =>
                                {
                                    controlButtonForHashMaps.IsEnabled = false;
                                    TestbuttonForPlugboardII.IsEnabled = false;

                                    listOfButtonListsII[myController.indexOfCurrentScramblerMap].Remove(TestbuttonForPlugboardII);
                                    CanvasForScramblerWindow.Children.Remove(TestbuttonForPlugboardII);

                                    foreach (Button btn in listOfButtonLists[myController.indexOfCurrentScramblerMap])
                                    {
                                        btn.IsEnabled = true;
                                    }
                                    foreach (Button btn in listOfButtonListsII[myController.indexOfCurrentScramblerMap])
                                    {
                                        btn.IsEnabled = false;
                                    }
                                    hashMapxTextBoxes[myController.indexOfCurrentScramblerMap].Text += controlButtonForHashMaps.Content + " - " + TestbuttonForPlugboardII.Content + "\r\n";
                                };
                            }
                        }
                    }
                }
            }
        }

        /*
         * Controls and distributes informations in the second window, parse maps into the scrambler hashmaps
         * FUNCTIONS:
         * EnableButtons()
         * SetupTheCostumModeEnigmaSettings()
         * ParseCustomHashMapsIntoStringLists()
         * ParseStringListsIntoListOfIntegerArrays()
         */

        public class ControllerII
        {
            public ControllerII()
            {
            }

            public int indexOfCurrentScramblerMap = 0;

            ViewII myViewII;
            public ViewII MyViewII  // property
            {
                get { return myViewII; }   // get method
                set { myViewII = value; }  // set method
            }

            Controller mainController;
            public Controller MainController  // property
            {
                get { return mainController; }   // get method
                set { mainController = value; }  // set method
            }

            Window mainEnigmaWindow;
            public Window MainEnigmaWindow  // property
            {
                get { return mainEnigmaWindow; }   // get method
                set { mainEnigmaWindow = value; }  // set method
            }

            public List<List<string>> listOfListWithHashMapChars = new List<List<string>>();
            public List<int[,]> listOfCustomHashMaps = new List<int[,]>();

            Button confirmButtonForHashMapSetting;
            public Button ConfirmButtonForHashMapSetting  // property
            {
                get { return confirmButtonForHashMapSetting; }   // get method
                set { confirmButtonForHashMapSetting = value; }  // set method
            }

            /// <summary>
            /// Only one hash map (the very left one) is avalible to created at a time
            /// </summary>
            public void EnableButtons()
            {
                if (indexOfCurrentScramblerMap >= myViewII.listOfButtonLists.Count)
                {
                    confirmButtonForHashMapSetting.IsEnabled = true;
                }
                else
                {
                    foreach (Button btn in myViewII.listOfButtonLists[indexOfCurrentScramblerMap])
                    {
                        btn.IsEnabled = true;
                    }
                }
            }

            /// <summary>
            /// Wraps the functions that parse custom maps, feeds them to main window and closes this one
            /// </summary>
            public void SetupTheCostumModeEnigmaSettings()
            {
                ParseCustomHashMapsIntoStringLists();
                mainController.SetTheCostumSettings(ref listOfCustomHashMaps);
            }

            /// <summary>
            /// First step in map parsing, creates list of string lists with only numbers and letter, corresponding to user created pairs
            /// </summary>
            public void ParseCustomHashMapsIntoStringLists()
            {
                //Creates lists for the number of ScramblerMapIndex, feeds them with strings from user maps and puts them into one list
                for (int i = 0; i < indexOfCurrentScramblerMap; i++)
                {
                    // TEST rychlejsiho algoritmu
                    int[,] controlArray = new int[26, 2];
                    int controlvar = 0;
                    //////////////////////////////


                    List<string> listOfParsingelements = new List<String>();
                    string testString;
                    testString = myViewII.hashMapxTextBoxes[i].Text.Replace(Environment.NewLine, "");
                    testString = testString.Replace(" ", "");
                    testString = testString.Replace("-", "");

                    //checks whether parsing number or letter, making sure the 2 digit numbers stay together in string and are not split apart by accident
                    //parse testString into hashmaps
                    string controlStringForParsingMethod = "";
                    for (int j = 0; j < testString.Length; j++)
                    {
                        if ((char)testString[j] <= 90 && (char)testString[j] >= 65)
                        {
                            controlArray[controlvar, 0] = int.Parse(controlStringForParsingMethod);
                            controlArray[controlvar, 1] = (char)testString[j] - 65;
                            controlStringForParsingMethod = "";
                            controlvar++;
                        }
                        else controlStringForParsingMethod += testString[j];
                    }
                    listOfCustomHashMaps.Add(controlArray);
                }
            }
        }
    }
}
