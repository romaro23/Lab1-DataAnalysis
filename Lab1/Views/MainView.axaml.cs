﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
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
using System.Reflection.PortableExecutable;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lab1.Views;

public partial class MainView : UserControl
{
    ObservableCollection<double> PrimaryData = new ObservableCollection<double>();
    public MainView()
    {
        InitializeComponent();
        string baseDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\Lab1"));
        string relativePath = "Assets\\data_lab1,2";
        string fullPath = Path.Combine(baseDirectory, relativePath);
        LoadQuantilies(fullPath);
        SetButton.Click += SetButton_Click;
        Files.Click += Files_Click;       
    }

    private async void Files_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {          
            Title = "Open Text File",
            AllowMultiple = false,
        });
        if (files != null && files.Count > 0)
        {
            var filePath = files[0].Path;
            using (var stream = await files[0].OpenReadAsync())
            using (var reader = new StreamReader(stream))
            {
                string line;                
                PrimaryData.Clear();
                while ((line = await reader.ReadLineAsync()) != null)
                {

                    var values = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var value in values)
                    {
                        if (double.TryParse(value, out var number))
                        {
                            PrimaryData.Add(number);
                        }
                    }
                }
                MainViewModel.Data.ProccedData(PrimaryData);

                CreateHistogram();
                CreateEmpiricalCDF();
            }
        }
    }
    private void SetButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        double value;
        if(double.TryParse(NumOfClasees.Text, out value))
        {
            Data.M = value;
            MainViewModel.Data.ProccedData(PrimaryData);
            CreateHistogram();
            CreateEmpiricalCDF();
        }
        else
        {
            NumOfClasees.Text = string.Empty;
        }
        
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

}
