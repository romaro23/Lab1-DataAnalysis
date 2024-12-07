using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Lab1.ViewModels;
using Lab1.Views;
using System.IO;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
namespace Lab1;

public partial class SecondView : UserControl
{
    ObservableCollection<double> FirstPrimaryData = new ObservableCollection<double>();
    ObservableCollection<double> SecondPrimaryData = new ObservableCollection<double>();
    public MainWindow ParentWindow { get; set; }
    public MainViewModel MainViewModel { get; set; } = new MainViewModel();
    public SecondView()
    {
        InitializeComponent();
        this.DataContext = MainViewModel;
        ChangeView.Click += ChangeView_Click;
        LoadData.Click += Files_Click;
        DepMeanButton.Click += ProceedCriteria_Click;
        DepVarianceButton.Click += ProceedCriteria_Click;
        DepWilcoxonButton.Click += ProceedCriteria_Click;
        IndepMeanButton.Click += ProceedCriteria_Click;
        IndepVarianceButton.Click += ProceedCriteria_Click;
        IndepVanDerVardenButton.Click += ProceedCriteria_Click;
    }
    private void ProceedCriteria_Click(object? sender, RoutedEventArgs e)
    {
        if (FirstPrimaryData.Count < 1 || SecondPrimaryData.Count < 1)
        {
            return;
        }
        Button button = sender as Button;
        if (button.Name == "DepMeanButton")
        {
            var firstSample = FirstPrimaryData.ToList();
            firstSample.RemoveAll(double.IsNaN);
            var secondSample = SecondPrimaryData.ToList();
            secondSample.RemoveAll(double.IsNaN);
            if (firstSample.Count != secondSample.Count)
            {
                return;
            }
            var result = MainViewModel.Criteria.DepMeanEquality();
            if (result.Item1 == true)
            {
                depMean.Text = $"Критерій: {result.Item2:F2}. Критичне значення: {result.Item3:F2}. {result.Item2:F2} <= {result.Item3:F2}. Середні можна вважати рівними.";
            }
            else
            {
                depMean.Text = $"Критерій: {result.Item2:F2}. Критичне значення: {result.Item3:F2}. {result.Item2:F2} > {result.Item3:F2}. Середні не можна вважати рівними.";
            }
        }
        if (button.Name == "DepVarianceButton")
        {
            var firstSample = FirstPrimaryData.ToList();
            firstSample.RemoveAll(double.IsNaN);
            var secondSample = SecondPrimaryData.ToList();
            secondSample.RemoveAll(double.IsNaN);
            if (firstSample.Count != secondSample.Count)
            {
                return;
            }
            var result = MainViewModel.Criteria.DepVarianceEquality();
            if (result.Item1 == true)
            {
                depVariance.Text = $"Критерій: {result.Item2:F2}. Критичне значення: {result.Item3:F2}. {result.Item2:F2} <= {result.Item3:F2}. Дисперсії можна вважати рівними.";
            }
            else
            {
                depVariance.Text = $"Критерій: {result.Item2:F2}. Критичне значення: {result.Item3:F2}. {result.Item2:F2} > {result.Item3:F2}. Дисперсії не можна вважати рівними.";
            }
        }
        if (button.Name == "DepWilcoxonButton")
        {
            var firstSample = FirstPrimaryData.ToList();
            firstSample.RemoveAll(double.IsNaN);
            var secondSample = SecondPrimaryData.ToList();
            secondSample.RemoveAll(double.IsNaN);
            if (firstSample.Count != secondSample.Count)
            {
                return;
            }
            var result = MainViewModel.Criteria.DepWilcoxonRanks();
            if (result.Item1 == true)
            {
                depWilcoxon.Text = $"Критерій: {result.Item2:F2}. Критичне значення: {result.Item3:F2}. {result.Item2:F2} <= {result.Item3:F2}. Зсуву у функціях розподілів немає.";
            }
            else
            {
                depWilcoxon.Text = $"Критерій: {result.Item2:F2}. Критичне значення: {result.Item3:F2}. {result.Item2:F2} > {result.Item3:F2}. Зсув у функціях розподілів є.";
            }
        }
        if (button.Name == "IndepMeanButton")
        {
            var result = MainViewModel.Criteria.IndepMeanEquality();
            if (result.Item1 == true)
            {
                indepMean.Text = $"Критерій: {result.Item2:F2}. Критичне значення: {result.Item3:F2}. {result.Item2:F2} <= {result.Item3:F2}. Середні можна вважати рівними.";
            }
            else
            {
                indepMean.Text = $"Критерій: {result.Item2:F2}. Критичне значення: {result.Item3:F2}. {result.Item2:F2} > {result.Item3:F2}. Середні не можна вважати рівними.";
            }
        }
        if (button.Name == "IndepVarianceButton")
        {
            var result = MainViewModel.Criteria.DepVarianceEquality();
            if (result.Item1 == true)
            {
                indepVariance.Text = $"Критерій: {result.Item2:F2}. Критичне значення: {result.Item3:F2}. {result.Item2:F2} <= {result.Item3:F2}. Дисперсії можна вважати рівними.";
            }
            else
            {
                indepVariance.Text = $"Критерій: {result.Item2:F2}. Критичне значення: {result.Item3:F2}. {result.Item2:F2} > {result.Item3:F2}. Дисперсії не можна вважати рівними.";
            }
        }
        if (button.Name == "IndepVanDerVardenButton")
        {
            var result = MainViewModel.Criteria.IndepVanDerVardenEquality();
            if (result.Item1 == true)
            {
                indepVanDerVarden.Text = $"Критерій: {result.Item2:F2}. Критичне значення: {result.Item3:F2}. {result.Item2:F2} <= {result.Item3:F2}. Вибірки можна вважати однорідними.";
            }
            else
            {
                indepVanDerVarden.Text = $"Критерій: {result.Item2:F2}. Критичне значення: {result.Item3:F2}. {result.Item2:F2} > {result.Item3:F2}. Вибірки не можна вважати однорідними.";
            }
        }
    }

    private void ChangeView_Click(object? sender, RoutedEventArgs e)
    {
        ParentWindow.SetMainView();
    }
    private async void Files_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        FirstPrimaryData.Clear();
        SecondPrimaryData.Clear();
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
        FirstPrimaryData.Clear();
        SecondPrimaryData.Clear();
        depMean.Text = null;
        depVariance.Text = null;
        depWilcoxon.Text = null;
        indepMean.Text = null;
        indepVariance.Text = null;
        indepVanDerVarden.Text = null;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            var values = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length >= 1 && double.TryParse(values[0], out var firstNumber))
            {
                FirstPrimaryData.Add(firstNumber);
            }
            else
            {
                FirstPrimaryData.Add(double.NaN);
            }
            if (values.Length >= 2 && double.TryParse(values[1], out var secondNumber))
            {
                SecondPrimaryData.Add(secondNumber);
            }
            else
            {
                SecondPrimaryData.Add(double.NaN);
            }
        }
        MainViewModel.CombInstanceData.Clear();

        var maxLength = Math.Max(FirstPrimaryData.Count, SecondPrimaryData.Count);

        for (int i = 0; i < maxLength; i++)
        {
            var firstValue = i < FirstPrimaryData.Count ? FirstPrimaryData[i] : 0.0;
            var secondValue = i < SecondPrimaryData.Count ? SecondPrimaryData[i] : 0.0;

            var dataRow = new MainViewModel.DataRow
            {
                Index = i + 1,
                FirstValue = firstValue,
                SecondValue = secondValue
            };

            MainViewModel.CombInstanceData.Add(dataRow);
            MainViewModel.Criteria.FirstSample = FirstPrimaryData;
            MainViewModel.Criteria.SecondSample = SecondPrimaryData;
            MainViewModel.Criteria.SetRows();
        }
    }
}