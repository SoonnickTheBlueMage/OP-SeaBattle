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

namespace Sea_Battle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            DrawTable();
        }

        private void DrawTable()
        {
            for (var rowIterator = 0; rowIterator < 11; rowIterator++)
            for (var columnIterator = 0; columnIterator < 21; columnIterator++)
            {
                if (columnIterator == 10 && rowIterator != 10)
                {
                    var numberField = new TextBlock
                    {
                        Background = Brushes.White,
                        Text = $"{rowIterator + 1}",
                        FontSize = 17,
                        TextAlignment = TextAlignment.Center
                    };
                    
                    Grid.SetColumn(numberField, columnIterator);
                    Grid.SetRow(numberField, rowIterator);

                    GridDuoTable.Children.Add(numberField);
                    continue;
                }

                if (rowIterator == 10 && columnIterator != 10)
                {
                    var letterField = new TextBlock
                    {
                        Background = Brushes.White,
                        Text = $"{(char) (columnIterator % 11 + 'a')}",
                        FontSize = 17,
                        TextAlignment = TextAlignment.Center
                    };
                    
                    Grid.SetColumn(letterField, columnIterator);
                    Grid.SetRow(letterField, rowIterator);

                    GridDuoTable.Children.Add(letterField);
                    continue;
                }

                if (rowIterator == 10 && columnIterator == 10) continue;

                var cell = new Border
                {
                    Background = Brushes.White,
                    BorderBrush = Brushes.CornflowerBlue,
                    BorderThickness = new Thickness(1)
                };

                Grid.SetColumn(cell, columnIterator);
                Grid.SetRow(cell, rowIterator);

                GridDuoTable.Children.Add(cell);
            }
        }
    }
}