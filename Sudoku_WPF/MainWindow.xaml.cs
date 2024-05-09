using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.Windows.Threading;

namespace Sudoku_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static int? GetDigit { get; set; } = null;
        private static int DifficultyLevel { get; set; }

        private static bool GameOver { get; set; } = false;


        private const int PUZZLE_SIZE = 9;
        private static readonly int[] VALUES = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        private static int[,] sudokuTableArr;
        private static int[,] answerTable;

        private static List<Button> nameList = new List<Button>();
        private List<Image> heartList = new List<Image>();// no static?

        private Button mainWindowButton = new Button();
        private Label gameOverLbl;
        //private Button hint;

        private static Random rand = new Random();

        private void RestartGame()
        {
            int cnt = heartList.Count();

            Chances.Children.Remove(gameOverLbl);

            GameOver = false;

            minutes = 0;
            seconds = 0;
            hours = 0;

            secondsLabel.Content = 0;
            minutesLabel.Content = 0;
            hoursLabel.Content = 0;


            for (int i = 0; i < 3 - cnt; i++)
            {

                BitmapImage btm = new BitmapImage(new Uri("/Images/heart.png", UriKind.Relative));

                //Image image = FindName($"image{i}") as Image;
                //image.Source = btm;

                Image image = new Image();
                image.Name = $"image{i}";
                image.Margin = new Thickness(2, 0, 0, 0);
                image.Width = 20;
                image.Source = btm;

                Chances.Children.Add(image);
                heartList.Add(image);
            }
        }

        private void FillButtons()
        {
            //MessageBox.Show("Enter fillButtons");
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {

                    if (sudokuTableArr[row, col] > 0)
                    {
                        string name = $"Button{row}{col}";

                        Button myButt = nameList.Where(n => n.Name == name).FirstOrDefault();

                        //MessageBox.Show(myButt.Name);

                        myButt.Content = $"{sudokuTableArr[row, col]}".ToString();
                        //MessageBox.Show("Access event");
                    }

                }
            }
        }

        private static bool IsValid(int row, int col, int val)
        {
            //MessageBox.Show("Validation");
            for (int c = 0; c < PUZZLE_SIZE; c++)
            {
                if (sudokuTableArr[row, c] == val) return false;
            }

            for (int r = 0; r < PUZZLE_SIZE; r++)
                if (sudokuTableArr[r, col] == val) return false;

            int grid = 3;

            int gridRow = (int)(Math.Floor(row / (float)grid) * grid), gridCol = (int)(Math.Floor(col / (float)grid) * grid);


            for (int r = gridRow; r < gridRow + grid; r++)
            {
                for (int c = gridCol; c < gridCol + grid; c++)
                {
                    if (sudokuTableArr[r, c] == val)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool StartGame(int row, int col)
        {
            //MessageBox.Show("Start Game");
            if (row == PUZZLE_SIZE - 1 && col == PUZZLE_SIZE)
            {
                return true;
            }

            if (col == PUZZLE_SIZE)
            {
                col = 0;
                row++;
            }

            if (sudokuTableArr[row, col] > 0) return StartGame(row, col + 1);

            for (int i = 0; i < PUZZLE_SIZE; i++)
            {
                if (IsValid(row, col, VALUES[i]))
                {
                    sudokuTableArr[row, col] = VALUES[i];
                    if (StartGame(row, col + 1))
                    {
                        return true;
                    }
                }


                sudokuTableArr[row, col] = 0;
            }

            return false;
        }

        private static int GetRandomNumber(int min, int max)
        {
            return rand.Next(min, max);
        }
        private static int GetRandomNumber()
        {
            return rand.Next(255) % PUZZLE_SIZE;//???
        }

        private static void GeneratePuzzle(int level)
        {
            //MessageBox.Show("Inside Puzzle");
            int emptyCell = (int)Math.Pow(PUZZLE_SIZE, 2) - level;

            while (emptyCell > 0)
            {
                int randomRow, randomColumn, digit;
                randomRow = GetRandomNumber();
                randomColumn = GetRandomNumber();
                digit = GetRandomNumber() + 1;

                if (sudokuTableArr[randomRow, randomColumn] == 0)
                {
                    //MessageBox.Show("Second loop");
                    bool valid = false;
                    while (!valid)
                    {
                        if (sudokuTableArr[randomRow, randomColumn] != 0)
                        {
                            sudokuTableArr[randomRow, randomColumn] = 0;
                            valid = true;

                        }

                        randomRow = GetRandomNumber();
                        randomColumn = GetRandomNumber();
                        digit = GetRandomNumber();
                    }
                }
                else
                {
                    sudokuTableArr[randomRow, randomColumn] = 0;
                }

                emptyCell--;
            }

            //Draw();
        }

        private static void GenerateAnswerTable()
        {
            for (int i = 0; i < PUZZLE_SIZE; i++)
            {
                for (int j = 0; j < PUZZLE_SIZE; j++)
                {
                    answerTable[i, j] = sudokuTableArr[i, j];
                }
            }
        }

        private static void Generator(int difficulty)
        {
            //MessageBox.Show("Inside Generator");
            int randomNumber = 0;

            switch (difficulty)
            {
                case 1:
                    randomNumber = GetRandomNumber(38, 50);
                    break;
                case 2:
                    randomNumber = GetRandomNumber(30, 40);
                    break;
                case 3:
                    randomNumber = GetRandomNumber(28, 35);
                    break;
                case 4:
                    randomNumber = 28;
                    break;
                default:
                    randomNumber = 55;
                    break;
            }

            int randomRow, randomColumn, digit;

            randomRow = GetRandomNumber();
            randomColumn = GetRandomNumber();
            digit = GetRandomNumber();

            sudokuTableArr[randomRow, randomColumn] = digit;

            StartGame(0, 0);

            GenerateAnswerTable();

            GeneratePuzzle(randomNumber);

        }

        private void HintClicked()
        {
            if (!GameOver)
            {

                int randomRow, randomColumn;

                randomRow = GetRandomNumber();
                randomColumn = GetRandomNumber();

                int hint;

                if (sudokuTableArr[randomRow, randomColumn] == 0)
                {
                    hint = answerTable[randomRow, randomColumn];
                }
                else
                {
                    while (sudokuTableArr[randomRow, randomColumn] != 0)
                    {
                        randomRow = GetRandomNumber();
                        randomColumn = GetRandomNumber();

                        if (sudokuTableArr[randomRow, randomColumn] == 0)
                        {
                            break;
                        }
                    }

                    hint = answerTable[randomRow, randomColumn];
                }

                GetDigit = hint;

                Button myButt = nameList.Where(n => n.Name == $"Button{randomRow}{randomColumn}").FirstOrDefault();

                myButt.Background = Brushes.Green;
                myButt.Content = GetDigit.ToString();

                if (heartList.Count > 1)
                {
                    Image heart = heartList[heartList.Count - 1];
                    Chances.Children.Remove(heart);
                    heartList.RemoveAt(heartList.Count - 1);


                    //MessageBox.Show($"img count from IF {heartList.Count} heart Name {heart.Name}");
                }
                else
                {
                    Image heart = heartList[heartList.Count - 1];
                    Chances.Children.Remove(heart);
                    heartList.RemoveAt(heartList.Count - 1);

                    //MessageBox.Show($"img count from else {heartList.Count} heart Name {heart.Name}");

                    gameOverLbl = new Label()
                    {
                        Content = "Game Over!"
                    };

                    GameOver = true;


                    gameOverLbl.FontSize = 12;
                    gameOverLbl.FontWeight = FontWeights.Bold;
                    gameOverLbl.VerticalContentAlignment = VerticalAlignment.Center;

                    Chances.Children.Add(gameOverLbl);
                }

                GetDigit = null;

            }
        }

        // WPF methods Below
        public MainWindow()
        {
            InitializeComponent();

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    Border border = new Border();
                    border.BorderBrush = Brushes.Black;
                    //border.BorderThickness = new Thickness(1, 1, 1, 2);

                    if (col == 0)
                    {
                        border.BorderThickness = new Thickness(3, 1, 1, 1);
                    }
                    else if (col == 8)
                    {
                        border.BorderThickness = new Thickness(1, 1, 3, 1);
                    }
                    else
                    {
                        border.BorderThickness = new Thickness(1, 1, 1, 1);
                    }


                    if(row == 0)
                    {
                        border.BorderThickness = new Thickness(1, 3, 1, 1);
                    }
                    if(row == 8)
                    {
                        border.BorderThickness = new Thickness(1, 1, 1, 3);
                    }





                    Grid.SetRow(border, row);
                    Grid.SetColumn(border, col);

                    mainWindowButton = new Button();
                    mainWindowButton.Name = $"Button{row}{col}";
                    mainWindowButton.Click += Button_Click;
                    mainWindowButton.FontSize = 18;
                    mainWindowButton.FontWeight = FontWeights.Bold;

                    border.Child = mainWindowButton;
                    sudokuTable.Children.Add(border);

                    nameList.Add(mainWindowButton);

                }
            }

            //const int grid = 2;
            //int cntHorizontal = 0;
            //int cntVertical = 0;

            //for (int row = 0; row < 9; row++)
            //{
            //    Border border = new Border();
            //    border.BorderBrush = Brushes.Black;

            //    if (cntHorizontal == grid)
            //    {
            //        border.BorderThickness = new Thickness(0.5, 2, 0.5, 0.5);
            //        cntHorizontal = 0;
            //    }
            //    else
            //    {
            //        cntHorizontal++;
            //    }

            //    for (int col = 0; col < 9; col++)
            //    {
            //        Grid.SetRow(border, row);
            //        Grid.SetColumn(border, col);

            //        if (cntVertical == grid)
            //        {
            //            border.BorderThickness = new Thickness(2, 0.5, 0.5, 0.5);
            //            cntVertical = 0;
            //        } else
            //        {
            //            cntVertical++;
            //        }

            //        if(col == 8 && grid == 2)
            //        {
            //            border.BorderThickness = new Thickness(0.5, 0.5, 2, 0.5);
            //        }

            //    }
            //}

            RestartGame();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!GameOver)
            {
                Button button = (Button)sender;
                if (GetDigit != null)
                {
                    string getComboBoxName = button.Name;
                    int row = Convert.ToByte(getComboBoxName[getComboBoxName.Length - 2] - 48);
                    int col = Convert.ToByte(getComboBoxName[getComboBoxName.Length - 1] - 48);

                    if (GetDigit == answerTable[row, col])
                    {
                        button.Background = Brushes.Transparent;
                        button.Content = GetDigit.ToString();
                        sudokuTableArr[row, col] = (int)GetDigit;
                    }
                    else
                    {
                        if (sudokuTableArr[row, col] == 0)
                        {
                            button.Background = Brushes.Pink;

                            if (heartList.Count > 1)
                            {
                                Image heart = heartList[heartList.Count - 1];
                                Chances.Children.Remove(heart);
                                heartList.RemoveAt(heartList.Count - 1);
                            }
                            else
                            {
                                Image heart = heartList[heartList.Count - 1];
                                Chances.Children.Remove(heart);
                                heartList.RemoveAt(heartList.Count - 1);

                                gameOverLbl = new Label()
                                {
                                    Content = "Game Over!"
                                };

                                GameOver = true;


                                gameOverLbl.FontSize = 12;
                                gameOverLbl.FontWeight = FontWeights.Bold;
                                gameOverLbl.VerticalContentAlignment = VerticalAlignment.Center;

                                Chances.Children.Add(gameOverLbl);
                            }
                        }
                    }

                    GetDigit = null;
                }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (!GameOver)
            {
                Button button = (Button)sender;
                string buttonName = button.Name;
                GetDigit = Convert.ToByte(button.Content);
                //MessageBox.Show("Digit clicked, Button name is: " + GetDigit);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!GameOver)
            {
                DispatcherTimer dt = new DispatcherTimer();
                dt.Interval = TimeSpan.FromSeconds(1);
                dt.Tick += dtTicker;
                dt.Start();
            }

        }

        private int seconds = 0, minutes = 0, hours = 0;
        private void dtTicker(object sender, EventArgs e)
        {
            if (!GameOver)
            {

                seconds++;
                if (seconds >= 60)
                {
                    secondsLabel.Content = (seconds - 60).ToString();
                    seconds = 0;
                    minutes++;

                }
                else
                {
                    secondsLabel.Content = seconds.ToString();
                }

                if (minutes >= 60)
                {
                    minutesLabel.Content = (minutes - 60).ToString();
                    minutes = 0;
                    hours++;
                }
                else
                {
                    minutesLabel.Content = minutes.ToString();
                }

                hoursLabel.Content = hours.ToString();
            }
        }

        private void myTB_MouseEnter(object sender, MouseEventArgs e)
        {
            myTB.Background = Brushes.Wheat;
        }

        private void myTB_MouseLeave(object sender, MouseEventArgs e)
        {

            myTB.Background = Brushes.Transparent;
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;


            if (button.IsMouseOver)
            {
                button.FontSize += 4;
                button.Width += 20;
                button.Height += 20;

            }

        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;

            if (!button.IsMouseOver)
            {
                button.FontSize -= 4;
                button.Width -= 20;
                button.Height -= 20;
            }
        }

        private void Hint_clicked(object sender, RoutedEventArgs e)
        {
            HintClicked();
        }

        //Added LATER
        private void ClearPuzzle()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    string name = $"Button{row}{col}";

                    Button myButt = nameList.Where(n => n.Name == name).FirstOrDefault();

                    //MessageBox.Show(myButt.Name);

                    myButt.Content = $"".ToString();
                    myButt.Background = Brushes.Transparent;
                }
            }
        }

        private void Digits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show("Enter into Digit selection method");
            RestartGame();
            ClearPuzzle();

            sudokuTableArr = new int[PUZZLE_SIZE, PUZZLE_SIZE];
            answerTable = new int[PUZZLE_SIZE, PUZZLE_SIZE];


            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem item = (ComboBoxItem)comboBox.SelectedItem;

            string getComboBoxName = item.Name;

            DifficultyLevel = Convert.ToByte(getComboBoxName[getComboBoxName.Length - 1] - 48);
            myTB.Text = ((ComboBoxItem)difficultyLevelCombo.SelectedItem).Content.ToString();


            //MessageBox.Show("Difficulty level: " + DifficultyLevel);
            Generator(DifficultyLevel);

            //MessageBox.Show("Calling fillButtons");
            FillButtons();
        }

    }
}
