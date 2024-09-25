using Avalonia;
using Avalonia.Controls;
using Lab1.Class;
using Lab1.ViewModels;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Colormaps;
using ScottPlot.Plottables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lab1.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        string baseDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\Lab1"));
        string relativePath = "Assets\\data_lab1,2";
        string fullPath = Path.Combine(baseDirectory, relativePath);
        CreateButtonsForDirectories(fullPath);
        LoadQuantilies(fullPath);
        SetButton.Click += SetButton_Click;

        
    }

    private void SetButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Data.M = double.Parse(NumOfClasees.Text);
    }

    private void LoadQuantilies(string path)
    {
        if (Directory.Exists(path)) 
        {
            var files = Directory.GetFiles(path);
            string[] lines = File.ReadAllLines(files[0]);
            foreach (string line in lines)
            {
                string[] parts = line.Split(' ');
                int key = int.Parse(parts[0]);
                double value = double.Parse(parts[1]);
                Class.StatisticalCharacteristics.Quantilies.Add(key, value);
            }
        }
    }
    private void CreateButtonsForDirectories(string directoryPath)
    {
        ButtonPanel.Children.Clear();

        if (Directory.Exists(directoryPath))
        {
            string[] directories = Directory.GetDirectories(directoryPath);

            foreach (var dir in directories)
            {
                Button folderButton = new Button
                {
                    Content = Path.GetFileName(dir),
                    Margin = new Thickness(5),
                    Width = 100,
                    Height = 50
                };

                folderButton.Click += (sender, e) => OnFolderButtonClick(dir);
                ButtonPanel.Children.Add(folderButton);
            }
        }
        else
        {
            Console.WriteLine("Directory not found: " + directoryPath);
        }
    }

    private void CreateButtonsForFiles(string folderPath)
    {
        ButtonPanel.Children.Clear();

        if (Directory.Exists(folderPath))
        {
            string[] files = Directory.GetFiles(folderPath);

            Button backButton = new Button
            {
                Content = "Return",
                Margin = new Thickness(5),
                Width = 100,
                Height = 50
            };
            backButton.Click += (sender, e) => CreateButtonsForDirectories(Path.GetDirectoryName(folderPath));
            ButtonPanel.Children.Add(backButton);

            foreach (var file in files)
            {
                Button fileButton = new Button
                {
                    Content = Path.GetFileName(file),
                    Margin = new Thickness(5),
                    Width = 150,
                    Height = 50
                };

                fileButton.Click += (sender, e) => OnFileButtonClick(file);
                ButtonPanel.Children.Add(fileButton);
            }
        }
        else
        {
            Console.WriteLine("Folder not found: " + folderPath);
        }
    }

    private void OnFolderButtonClick(string folderPath)
    {
        Console.WriteLine($"Clicked on folder: {folderPath}");
        CreateButtonsForFiles(folderPath);
    }
    private void CreateHistogram()
    {
        double[] heights = new double[MainViewModel.Data.Classes.Count];
        double[] leftEdges = new double[MainViewModel.Data.ClassBoundaries.Count - 1];
        double[] rightEdges = new double[MainViewModel.Data.ClassBoundaries.Count - 1];
        for (int i = 0; i < MainViewModel.Data.Classes.Count; i++)
        {
            heights[i] = MainViewModel.Data.Classes[i].RelativeFrequency;
        }
        for (int i = 0; i < MainViewModel.Data.ClassBoundaries.Count - 1; i++)
        {
            leftEdges[i] = MainViewModel.Data.ClassBoundaries[i];
            rightEdges[i] = MainViewModel.Data.ClassBoundaries[i + 1];
        }
        AvaPlot histogram = this.Find<AvaPlot>("Histogram");
        histogram.Plot.Clear();
        for (int i = 0; i < leftEdges.Length; i++)
        {
            double centerPosition = (leftEdges[i] + rightEdges[i]) / 2;
            double barWidth = rightEdges[i] - leftEdges[i];
            Bar bar = new Bar();
            bar.Position = centerPosition;
            bar.Value = heights[i];
            bar.Size = barWidth;
            bar.FillColor = Color.FromColor(System.Drawing.Color.Orange);
            histogram.Plot.Add.Bar(bar);
        }
        histogram.Plot.Axes.Margins(0, 0);
        histogram.Refresh();
    }
    private void CreateEmpiricalCDF()
    {
        AvaPlot empiricalCDF = this.Find<AvaPlot>("EmpiricalCDF");
        empiricalCDF.Plot.Clear();
        var sp1 = empiricalCDF.Plot.Add.Scatter(MainViewModel.Data.X.ToArray(), MainViewModel.Data.F.ToArray());
        sp1.ConnectStyle = ConnectStyle.StepHorizontal;
        empiricalCDF.Plot.Axes.Margins(0, 0);
        empiricalCDF.Refresh();
    }
    private void OnFileButtonClick(string filePath)
    {
        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                ObservableCollection<double> PrimaryData = new ObservableCollection<double>();
                //MainViewModel.PrimaryData.Clear();
                while ((line = reader.ReadLine()) != null)
                {
                    if (double.TryParse(line, out var value))
                    {
                        PrimaryData.Add(value);
                    }
                }
                MainViewModel.Data.ProccedData(PrimaryData);
                CreateHistogram();
                CreateEmpiricalCDF();
                
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        Console.WriteLine($"Clicked on file: {filePath}");
    }
}
