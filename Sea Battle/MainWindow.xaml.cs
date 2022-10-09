using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Sea_Battle.Models;

namespace Sea_Battle;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private static readonly Game CurrentGame = new();

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

    private void ButtonClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button pressedButton)
        {
            MessageBox.Show("Нажатие на объект, не являющийся кнопкой");
            return;
        }

        if (pressedButton.Name == "TurnButton")
        {
            Execute(CurrentGame.Click(Player.First, -1, -1));
            return;
        }

        var owner = ParseButtonName(pressedButton.Name).Item1;
        var row = ParseButtonName(pressedButton.Name).Item2.Item1;
        var column = ParseButtonName(pressedButton.Name).Item2.Item2;

        Execute(CurrentGame.Click(owner, row, column));
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

    private void Execute(Command command)
    {
        switch (command.Draw)
        {
            case DrawingType.DrawHit:
            {
                var line1 = new Line
                {
                    IsHitTestVisible = false,
                    X1 = 0,
                    Y1 = 0,
                    X2 = 30,
                    Y2 = 30,
                    Stroke = Brushes.Firebrick,
                    StrokeThickness = 3
                };

                var line2 = new Line
                {
                    IsHitTestVisible = false,
                    X1 = 30,
                    Y1 = 0,
                    X2 = 0,
                    Y2 = 30,
                    Stroke = Brushes.Firebrick,
                    StrokeThickness = 3
                };

                Grid.SetRow(line1, command.Row);
                Grid.SetColumn(line1, command.BoardOwner == Player.First ? command.Column : command.Column + 11);

                Grid.SetRow(line2, command.Row);
                Grid.SetColumn(line2, command.BoardOwner == Player.First ? command.Column : command.Column + 11);

                GridDuoTable.Children.Add(line1);
                GridDuoTable.Children.Add(line2);

                break;
            }

            case DrawingType.DrawMiss:
            {
                var dot = new Ellipse
                {
                    IsHitTestVisible = false,
                    Width = 5,
                    Height = 5,
                    Fill = Brushes.Black
                };

                Grid.SetRow(dot, command.Row);
                Grid.SetColumn(dot, command.BoardOwner == Player.First ? command.Column : command.Column + 11);

                GridDuoTable.Children.Add(dot);

                break;
            }

            case DrawingType.DrawShipPart:
            {
                var square = new Rectangle()
                {
                    IsHitTestVisible = false,
                    Tag = $"Ship_{command.BoardOwner}_{command.Row}_{command.Column}",
                    Width = 30,
                    Height = 30,
                    Fill = Brushes.MediumBlue
                };

                Grid.SetRow(square, command.Row);
                Grid.SetColumn(square, command.BoardOwner == Player.First ? command.Column : command.Column + 11);

                GridDuoTable.Children.Add(square);
                break;
            }

            case DrawingType.EraseShipPart:
            {
                var name = $"Ship_{command.BoardOwner}_{command.Row}_{command.Column}";

                for (var i = GridDuoTable.Children.Count - 1; i >= 0; --i)
                    if (GridDuoTable.Children[i] is Rectangle figure && (string) figure.Tag == name)
                    {
                        GridDuoTable.Children.RemoveAt(i);
                        break;
                    }

                break;
            }

            case DrawingType.Hide:
            {
                for (var i = GridDuoTable.Children.Count - 1; i >= 0; --i)
                    if (GridDuoTable.Children[i] is Rectangle figure &&
                        figure.Tag.ToString()!.Contains($"Ship_{command.BoardOwner}"))
                        figure.Visibility = Visibility.Hidden;

                break;
            }

            case DrawingType.Show:
            {
                for (var i = GridDuoTable.Children.Count - 1; i >= 0; --i)
                    if (GridDuoTable.Children[i] is Rectangle figure &&
                        figure.Tag.ToString()!.Contains($"Ship_{command.BoardOwner}"))
                        figure.Visibility = Visibility.Visible;

                break;
            }

            case DrawingType.Empty:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}