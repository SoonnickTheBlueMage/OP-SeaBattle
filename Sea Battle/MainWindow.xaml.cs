using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sea_Battle.Models;

namespace Sea_Battle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Game _currentGame = new Game();
        
        public MainWindow()
        {
            InitializeComponent();

            DrawTable();
        }

        private static Tuple<Player, Tuple<int, int>> ParseButtonName(string name)
        {
            var owner = name[0] == 'F' ? Player.First : Player.Second;
            var row = name[1] - '0';
            var column = name[2] - '0';

            return new Tuple<Player, Tuple<int, int>>(owner, new Tuple<int, int>(row, column));
        }

        private static void ButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button pressedButton)
            {
                MessageBox.Show("Нажатие на объект, не являющийся кнопкой");
                return;
            }

            MessageBox.Show($"{ParseButtonName(pressedButton.Name).Item1}" +
                            $"{ParseButtonName(pressedButton.Name).Item2.Item1}" + 
                            $"{ParseButtonName(pressedButton.Name).Item2.Item2}");
        }

        private void DrawTable()
        {
            for (var rowIterator = 0; rowIterator < 11; rowIterator++)
            for (var columnIterator = 0; columnIterator < 21; columnIterator++)
            {
                if (columnIterator == 10 && rowIterator != 10)
                {
                    DrawText($"{rowIterator + 1}", rowIterator, columnIterator);
                    continue;
                }

                if (rowIterator == 10 && columnIterator != 10)
                {
                    DrawText($"{(char) (columnIterator % 11 + 'A')}", rowIterator, columnIterator);
                    continue;
                }

                if (rowIterator == 10 && columnIterator == 10) continue;

                var name = columnIterator < 10 ? 'F' : 'S';
                var column = columnIterator < 10 ? columnIterator : columnIterator - 11;

                var cell = new Button
                {
                    Background = Brushes.White,
                    BorderBrush = Brushes.DarkOrchid,
                    BorderThickness = new Thickness(0.4),
                    Name = $"{name}{rowIterator}{column}"
                };

                cell.Click += ButtonClick;
                Grid.SetColumn(cell, columnIterator);
                Grid.SetRow(cell, rowIterator);

                GridDuoTable.Children.Add(cell);
            }
            DrawBorders();
        }

        private void DrawText(string text, int row, int column)
        {
            var numberField = new TextBlock
            {
                Background = Brushes.White,
                Foreground = Brushes.MediumBlue,
                FontFamily = new FontFamily("Comic Sans MS, Verdana"),
                Text = text,
                FontSize = 17,
                TextAlignment = TextAlignment.Center
            };

            Grid.SetRow(numberField, row);
            Grid.SetColumn(numberField, column);

            GridDuoTable.Children.Add(numberField);
        }

        private void DrawBorders()
        {
            var borderLeft = new Border
            {
                BorderBrush = Brushes.MediumBlue,
                BorderThickness = new Thickness(2)
            };

            var borderRight = new Border
            {
                BorderBrush = Brushes.MediumBlue,
                BorderThickness = new Thickness(2)
            };

            Grid.SetRow(borderLeft, 0);
            Grid.SetColumn(borderLeft, 0);
            Grid.SetRowSpan(borderLeft, 10);
            Grid.SetColumnSpan(borderLeft, 10);

            Grid.SetRow(borderRight, 0);
            Grid.SetColumn(borderRight, 11);
            Grid.SetRowSpan(borderRight, 10);
            Grid.SetColumnSpan(borderRight, 10);

            GridDuoTable.Children.Add(borderLeft);
            GridDuoTable.Children.Add(borderRight);
        }
    }
}