using ForestTrails.Command;
using ForestTrails.Extentions;
using ForestTrails.Paths;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ForestTrails.Builder;
using System.Windows.Media;
using System.Runtime.Serialization;
using System.Windows.Data;
using System.Text;
using System.Diagnostics;
using ForestTrails.TwoDRange;

namespace ForestTrails
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ForestPaths GlobalForestPaths { get; set; } = new ForestPaths();
        public Invoker invoker = new Invoker();
        public Ellipse bufferEllipse = null;
        public CampCrossroad bufferCrossroad = null;
        public CrossroadContext crossroadContext = new CrossroadContext();
        public HashSet<Line> highlightedLines = new HashSet<Line>();
        public HashSet<Line> deletedLines = new HashSet<Line>();
        public TwoDRangeTree<ICrossroad, double> GlobalRangeTree;

        public MainWindow()
        {
            InitializeComponent();
            InitializeLegend();
            using (FileStream fs = new FileStream("./sample.txt", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                ForestPaths newForestPath = (ForestPaths)formatter.Deserialize(fs);
                GlobalForestPaths.Update(newForestPath);
                DrawForrestPaths();
            }
        }

        private void InitializeLegend()
        {
            DrawLegentEllipse(SimpleCrossroadExample, new SimpleCrossroad());
            DrawLegentEllipse(CampCrossroadExample, new CampCrossroad());
            DrawLegentEllipse(BusStopCrossroadExample, new BusStopCrossroad());
        }

        private void DrawLegentEllipse(Ellipse ellipse, ICrossroad crossroad)
        {
            var simpleDrawInf = crossroad.GetDrawInformation();
            ellipse.Height = simpleDrawInf.Height;
            ellipse.Width = simpleDrawInf.Width;
            ellipse.Fill = simpleDrawInf.Fill;
            ellipse.Stroke = simpleDrawInf.Stroke;
            ellipse.StrokeThickness = simpleDrawInf.Thickness;
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas canvas = (Canvas)sender;
            IInputElement inputElement = Mouse.DirectlyOver;
            if (GlobalEditModeRadioButton.IsChecked == true)
            {
                DrawPaths(sender, e, inputElement);
            }
            else if (GlobalRunModeRadioButton.IsChecked == true) 
            {
                if (inputElement is Ellipse ellipse)
                {
                    FindWay(ellipse);
                }
                else if(inputElement is Line line)
                {
                    if (deletedLines.Contains(line))
                    {
                        line.Stroke = Brushes.Gray;
                        line.MouseEnter += Line_MouseEnter;
                        line.MouseLeave += Line_MouseLeave;
                        deletedLines.Remove(line);
                        string[] keys = line.Name.Split('_');
                        GlobalForestPaths.AddRoad(keys[0], keys[1]);
                    }
                    else
                    { 
                        line.Stroke = Brushes.Red;
                        line.MouseEnter -= Line_MouseEnter;
                        line.MouseLeave -= Line_MouseLeave;
                        deletedLines.Add(line);
                        string[] keys = line.Name.Split('_');
                        GlobalForestPaths.RemoveRoad(keys[0], keys[1]);
                    }
                }
            }

        }

        private void FindWay(Ellipse ellipse)
        {
            ICrossroad crossroad = GlobalForestPaths.GetCrossroad(ellipse.Name);
            if (bufferCrossroad == null && crossroad is CampCrossroad)
            {
                bufferCrossroad = crossroad as CampCrossroad;
                bufferEllipse = ellipse;
                bufferEllipse.MouseLeave -= Ellipse_MouseLeave;
                RunModeToolBar.IsEnabled = false;
                GlobalModeToolBar.IsEnabled = false;
            }
            else if (bufferCrossroad != null && crossroad is BusStopCrossroad)
            {
                FindAndHighlight(crossroad);

                bufferEllipse.Unhighlight(GlobalForestPaths);
                bufferEllipse.MouseLeave += Ellipse_MouseLeave;
                bufferCrossroad = null;
                bufferEllipse = null;
                RunModeToolBar.IsEnabled = true;
                GlobalModeToolBar.IsEnabled = true;
            }
        }

        private void FindAndHighlight(ICrossroad crossroad)
        {
            AStar aStar = new AStar();
            List<ICrossroad> way = aStar.FindWay(GlobalForestPaths, bufferCrossroad, crossroad as BusStopCrossroad);
            if(way == null)
            {
                MessageBox.Show("The way does not exists");
                return;
            }
            for (int i = 0; i < way.Count - 1; i++)
            {
                ICrossroad firstCrossroad = way[i];
                ICrossroad secondCrossroad = way[i + 1];
                foreach (var children in Canvas.Children)
                {
                    if (children is Line line &&
                        (line.Name == $"{firstCrossroad.Key}_{secondCrossroad.Key}" ||
                        line.Name == $"{secondCrossroad.Key}_{firstCrossroad.Key}"))
                    {
                        line.Stroke = Brushes.Green;
                        line.MouseEnter -= Line_MouseEnter;
                        line.MouseLeave -= Line_MouseLeave;
                        highlightedLines.Add(line);
                    }
                }
            }
        }

        private void DrawPaths(object sender, MouseButtonEventArgs e, IInputElement inputElement)
        {
            if (DrawModeRadioButton.IsChecked == true)
            {
                DrawElement(sender, e, inputElement);
            }
            else if (DeleteModeRadioButton.IsChecked == true)
            {
                DeleteElement(sender, inputElement);
            }
            else if (EditModeRadioButton.IsChecked == true)
            {
                EditElement(sender, e, inputElement);
            }
            try
            {
                invoker.Execute();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                invoker.ClearCommand();
            }
        }

        private void EditElement(object sender, MouseEventArgs e, IInputElement inputElement)
        {
            if (DotRadioButton.IsChecked == true && inputElement is Ellipse ellipse)
            {
                SetContext(sender, e);
                invoker.SetCommand(new EditTypeCommand(sender, ellipse, GlobalForestPaths, crossroadContext));
            }
        }

        private void DeleteElement(object sender, IInputElement inputElement)
        {
            if (LineRadioButton.IsChecked == true && inputElement is Line line)
            {
                invoker.SetCommand(new DeleteConnectionCommand(sender, line, GlobalForestPaths));
            }
            else if (DotRadioButton.IsChecked == true && inputElement is Ellipse ellipse)
            {
                invoker.SetCommand(new DeleteCrossroadCommand(sender, ellipse, GlobalForestPaths));
            }
        }

        private void DrawElement(object sender, MouseButtonEventArgs e, IInputElement inputElement)
        {
            if (DotRadioButton.IsChecked == true && (inputElement is Ellipse) == false)
            {                
                SetContext(sender, e);
                invoker.SetCommand(new DrawCrossroadCommand(sender, GlobalForestPaths, crossroadContext));
            }
            else if (LineRadioButton.IsChecked == true && inputElement is Ellipse ellipse)
            {
                if (bufferEllipse == null)
                {
                    bufferEllipse = ellipse;
                    bufferEllipse.MouseLeave -= Ellipse_MouseLeave;
                    DrawToolBar.IsEnabled = false;
                    ModeToolBar.IsEnabled = false;
                    GlobalModeToolBar.IsEnabled = false;
                }
                else
                {
                    invoker.SetCommand(new DrawConnectionCommand(sender, bufferEllipse, ellipse, GlobalForestPaths));
                    bufferEllipse.Unhighlight(GlobalForestPaths);
                    bufferEllipse.MouseLeave += Ellipse_MouseLeave;
                    bufferEllipse = null;
                    DrawToolBar.IsEnabled = true;
                    ModeToolBar.IsEnabled = true;
                    GlobalModeToolBar.IsEnabled = true;
                }
            }
        }

        private void SetContext(object sender, MouseEventArgs e)
        {
            Canvas canvas = (Canvas)sender;
            Point point = e.GetPosition(canvas);
            if (CampRadioButton.IsChecked == true)
            {
                crossroadContext.Сrossroad = new CampCrossroad()
                {
                    Position = point
                };
            }
            else if (BusStopRadioButton.IsChecked == true)
            {
                crossroadContext.Сrossroad = new BusStopCrossroad()
                {
                    Position = point
                };
            }
            else
            {
                crossroadContext.Сrossroad = new SimpleCrossroad()
                {
                    Position = point
                };
            }
        }


        public void Ellipse_MouseEnter(object sender, MouseEventArgs e)
        {
            Ellipse ellipse = (Ellipse)sender;
            ellipse.Highlight(GlobalForestPaths);
        }

        public void Ellipse_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                Ellipse ellipse = (Ellipse)sender;
                ellipse.Unhighlight(GlobalForestPaths);
            }
            catch (KeyNotFoundException)
            {

            }
        }

        private void GlobalRunModeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                EditModeTray.Visibility = Visibility.Collapsed;
                RunModeTray.Visibility = Visibility.Visible;
            }
            catch (NullReferenceException)
            {

            }
        }

        private void GlobalEditModeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RunModeTray.Visibility = Visibility.Collapsed;
            EditModeTray.Visibility = Visibility.Visible;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, GlobalForestPaths);
                }
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                using(FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open))
                {
                    try
                    {
                        ForestPaths newForestPath = (ForestPaths)formatter.Deserialize(fs);
                        GlobalForestPaths.Update(newForestPath);
                        DrawForrestPaths();
                        List<ICrossroad> crossroads = GlobalForestPaths.GetCrossroads();

                    }
                    catch (SerializationException)
                    {
                        MessageBox.Show($"Unacceptable file", "Error");
                    }
                }
            }
        }

        private void DrawForrestPaths()
        {
            Canvas.Children.Clear();
            foreach (var crossroad in GlobalForestPaths.GetCrossroads())
            {
                BuildDirector ellipseDirector = new BuildDirector();
                EllipseBuilder ellipseBuilder = ellipseDirector.BuildEllipse(crossroad);
                Ellipse ellipse = ellipseBuilder.GetEllipse();

                Canvas.SetLeft(ellipse, crossroad.Position.X - ellipse.Height / 2);
                Canvas.SetTop(ellipse, crossroad.Position.Y - ellipse.Width / 2);
                Canvas.SetZIndex(ellipse, 2);
                Canvas.Children.Add(ellipse);

                foreach (var eges in GlobalForestPaths.GetNextCrossroads(crossroad))
                {
                    BuildDirector lineDirector = new BuildDirector();
                    LineBuilder lineBuilder = lineDirector.BuildLine(GlobalForestPaths, crossroad.Key, eges.Key);
                    Line line = lineBuilder.GetLine();
                    string[] lineName = line.Name.Split('_');
                    if (FindChildByName($"{lineName[1]}_{lineName[0]}") == null)
                    {
                        Canvas.SetZIndex(line, 1);
                        Canvas.Children.Add(line);
                    }
                    
                }
            }
        }

        private FrameworkElement FindChildByName(string name)
        {
            foreach (FrameworkElement element in Canvas.Children)
            {
                if(element.Name == name)
                {
                    return element;
                }
            }
            return null;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var line in highlightedLines)
            {
                if (!deletedLines.Contains(line))
                {
                    line.Stroke = Brushes.Gray;
                    line.MouseEnter += Line_MouseEnter;
                    line.MouseLeave += Line_MouseLeave;
                }
            }
            highlightedLines.Clear();
        }

        public void Line_MouseEnter(object sender, MouseEventArgs e)
        {
            Line line = (Line)sender;
            line.Stroke = Brushes.DarkRed;
            line.StrokeThickness = 5;
        }

        public void Line_MouseLeave(object sender, MouseEventArgs e)
        {
            Line line = (Line)sender;
            line.Stroke = Brushes.Gray;
            line.StrokeThickness = 3;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Canvas.Children.Clear();
            GlobalForestPaths.Clear();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Ellipse ellipse = (Ellipse)FindChildByName(FindTextBox.Text);
                ellipse.Highlight(GlobalForestPaths);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Element not dound", "Error");
            }
        }

        private void GenerateMatrixButton_Click(object sender, RoutedEventArgs e)
        {
            List<List<ICrossroad>> aStarResults = new List<List<ICrossroad>>();
            StringBuilder matrix = new StringBuilder();
            foreach (var camp in GlobalForestPaths.GetCampCrossroads())
            {
                foreach (var busStop in GlobalForestPaths.GetBusStopCrossroads())
                {
                    AStar aStar = new AStar();
                    aStarResults.Add(aStar.FindWay(GlobalForestPaths, camp, busStop));
                }
            }

            
            foreach (var aStarList in aStarResults)
            {
                try
                {
                    matrix.Append($"From {aStarList[0].Key} to {aStarList[aStarList.Count - 1].Key}:\n\t");
                    foreach (var crossroad in aStarList)
                    {
                        matrix.Append($"{crossroad.Key} ");
                    }
                    matrix.Append("\n");
                }
                catch (NullReferenceException)
                {

                }
            }
            NotepadHelper.ShowMessage(matrix.ToString());

        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("notepad.exe", "./HelpFile.txt");
        }
    }
}
