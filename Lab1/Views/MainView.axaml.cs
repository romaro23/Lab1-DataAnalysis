﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using DynamicData;
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
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lab1.Views;

public partial class MainView : UserControl
{
    ObservableCollection<double> PrimaryData = new ObservableCollection<double>();
    public MainWindow ParentWindow { get; set; }
    public MainView()
    {
        InitializeComponent();
        string baseDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\Lab1"));
        string relativePath = "Assets\\data_lab1,2";
        string fullPath = Path.Combine(baseDirectory, relativePath);
        SetButton.Click += SetButton_Click;
        SetBandwidth.Click += SetBandwidth_Click;
        Files.Click += Files_Click;
        FindAnomalies.Click += FindAnomalies_Click;
        RemoveAnomalies.Click += RemoveAnomalies_Click;
        Distribution.Click += Distribution_Click;
        Pearson.Click += Pearson_Click;
        ChangeView.Click += ChangeView_Click;
    }

    private void ChangeView_Click(object? sender, RoutedEventArgs e)
    {
        ParentWindow.SetSecondView();
    }

    private void Pearson_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (MainViewModel.Data.Items.Count != 0)
        {
            var p = MainViewModel.Data.PearsonProbability;
            CreateFrequencyHistogram();
            string first = $"Статистика критерію: {p.X:F2} | ";
            string second = $"Критичне значення: {p.X_CR:F2} | ";
            string third = $"P-значення: {p.P:F5} | ";
            string fourth;
            if(p.X <= p.X_CR)
            {
                fourth = $"{p.X:F2} <= {p.X_CR:F2}, отже розподіл вірогідний";
            }
            else
            {
                fourth = $"{p.X:F2} > {p.X_CR:F2}, отже розподіл не вірогідний";
            }
            Probability.Text = first + second + third + fourth;
        }
    }

    //9
    private void Distribution_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(MainViewModel.Data.Items.Count != 0)
        {
            ShowDistribution.IsOpen = true;
            //if (MainViewModel.Data.stats.IsAsymmetryZero() == true && MainViewModel.Data.stats.IsExcessZero() == true)
            //{
            //    DistributionByCoefficient.Text = "Нормальний розподіл ідентифіковано";
            //}
            //else
            //{
            //    DistributionByCoefficient.Text = "Нормальний розподіл не ідентифіковано";
            //}
            CreateDistribution();
        }
        
    }
    //
    private void RemoveAnomalies_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ShowAnomalies.IsOpen = false;
        MainViewModel.Data.stats.RemoveAnomalies(PrimaryData);
        MainViewModel.Data.ProccedData(PrimaryData);
        CreateHistogram();
        CreateEmpiricalCDF();
    }
    //8
    private void FindAnomalies_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (PrimaryData.Count == 0)
        {
            return;
        }
        ShowAnomalies.IsOpen = true;
        double leftInterval;
        double rightInterval;
        List<double> anomalies = MainViewModel.Data.stats.FindAnomalies(PrimaryData, out leftInterval, out rightInterval);
        var selectedValues = anomalies.Select(index => PrimaryData[(int)index]);
        AnomaliesList.Text = "Anomalies: " + string.Join(", ", selectedValues);
        AvaPlot Anomalies = this.Find<AvaPlot>("Anomalies");
        Anomalies.Plot.Clear();
        var scatter = Anomalies.Plot.Add.Scatter(Enumerable.Range(0, PrimaryData.Count).Select(x => (double)x).ToArray(), PrimaryData.ToArray());
        scatter.LegendText = "Розкид значень";
        var line1 = Anomalies.Plot.Add.HorizontalLine(leftInterval, 2, ScottPlot.Color.FromColor(System.Drawing.Color.Red), LinePattern.Dashed);
        line1.LegendText = "Межі";
        Anomalies.Plot.Add.HorizontalLine(rightInterval, 2, ScottPlot.Color.FromColor(System.Drawing.Color.Red), LinePattern.Dashed);
        Anomalies.Plot.Axes.Margins(0, 0);
        Anomalies.Plot.XLabel("Індекс значення");
        Anomalies.Plot.YLabel("Значення з вибірки");
        Anomalies.Refresh();

    }
    //
    private void SetBandwidth_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        double value;
        if (double.TryParse(BandwidthBox.Text, out value))
        {
            Data.Bandwidth = value;
            MainViewModel.Data.ProccedData(PrimaryData);
            CreateHistogram();
            CreateFrequencyHistogram();
            CreateEmpiricalCDF();
        }
        else
        {
            BandwidthBox.Text = string.Empty;
        }
    }

    private async void Files_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {          
            Title = "Open Text File",
            AllowMultiple = false,
        });
        if (files == null && files.Count <= 0)
        {
            return;
        }
        var filePath = files[0].Path;
        await using var stream = await files[0].OpenReadAsync();
        using var reader = new StreamReader(stream);
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


    private void CreateFrequencyHistogram()
    {
        double[] heights1 = new double[MainViewModel.Data.Classes.Count];
        double[] heights2 = new double[MainViewModel.Data.Frequency.Count];
        double[] leftEdges = new double[MainViewModel.Data.ClassBoundaries.Count - 1];
        double[] rightEdges = new double[MainViewModel.Data.ClassBoundaries.Count - 1];
        for (int i = 0; i < MainViewModel.Data.Classes.Count; i++)
        {
            heights1[i] = MainViewModel.Data.Classes[i].Frequency;
            heights2[i] = MainViewModel.Data.Frequency[i];
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
            bar.Value = heights1[i];
            bar.Size = barWidth;
            bar.FillColor = Color.FromARGB(0x80FFA500);
            bar.Label = "Frequency";
            Bar bar1 = new Bar();
            bar1.Position = centerPosition;
            bar1.Value = heights2[i];
            bar1.Size = barWidth;
            bar1.FillColor = Color.FromColor(System.Drawing.Color.Blue);
            bar1.Label = "Theoretical frequency";
            histogram.Plot.Add.Bar(bar1);
            histogram.Plot.Add.Bar(bar);
        }
        histogram.Plot.Axes.Margins(0, 0);
        histogram.Refresh();
    }
    //4
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
        //5
        var scaledKDE = MainViewModel.Data.KDE.Select(value => value * MainViewModel.Data.stats.H).ToArray();
        var scaledDensity = MainViewModel.Data.DensityFunction.Select(value => value * MainViewModel.Data.stats.H).ToArray();
        var scatter1 = histogram.Plot.Add.Scatter(PrimaryData.OrderBy(x => x).ToArray(), scaledKDE, color: Color.FromColor(System.Drawing.Color.Blue));
        scatter1.LegendText = "KDE";
        var scatter2 = histogram.Plot.Add.Scatter(PrimaryData.OrderBy(x => x).ToArray(), scaledDensity, color: Color.FromColor(System.Drawing.Color.Green));
        scatter2.LegendText = "DensityFunction";
        histogram.Plot.Axes.Margins(0, 0);
        histogram.Plot.XLabel("Елементи з вибірки");
        histogram.Plot.YLabel("Відносна частота класів та значення KDE");
        histogram.Refresh();
    }
    //6
    private void CreateEmpiricalCDF()
    {
        AvaPlot empiricalCDF = this.Find<AvaPlot>("EmpiricalCDF");
        empiricalCDF.Plot.Clear();
        var sp1 = empiricalCDF.Plot.Add.Scatter(MainViewModel.Data.Variants.ToArray(), MainViewModel.Data.EmpiricalCDF.ToArray());
        var sp2 = empiricalCDF.Plot.Add.Scatter(MainViewModel.Data.Variants.ToArray(), MainViewModel.Data.DistributionFunction.ToArray());
        sp1.ConnectStyle = ConnectStyle.StepHorizontal;
        sp1.LegendText = "EmpiricalCDF";
        sp2.LegendText = "DistributionFunction";
        empiricalCDF.Plot.Axes.Margins(0, 0);
        empiricalCDF.Plot.XLabel("Елементи з вибірки");
        empiricalCDF.Plot.YLabel("Значення емпіричної функції");
        empiricalCDF.Refresh();
    }
    //10
    private void CreateDistribution()
    {
        //double[] xValues = MainViewModel.Data.Variants.ToArray();
        double[] xValues = MainViewModel.Data.T.ToArray();
        xValues = xValues.Take(xValues.Length - 1).ToArray();
        //double[] yValues = MainViewModel.Data.EmpiricalCDFQuantilies.ToArray();
        double[] yValues = MainViewModel.Data.Z.ToArray();
        yValues = yValues.Take(yValues.Length - 1).ToArray();
        AvaPlot distribution = this.Find<AvaPlot>("DistributionPlot");
        distribution.Plot.Clear();
        var scatter = distribution.Plot.Add.Scatter(xValues, yValues);
        //scatter.LegendText = "Значення квантилів";
        distribution.Plot.Axes.Margins(0, 0);
        //distribution.Plot.XLabel("Елементи з вибірки");
        //distribution.Plot.YLabel("Квантилі ЕФР");
        distribution.Refresh();
    }

}
