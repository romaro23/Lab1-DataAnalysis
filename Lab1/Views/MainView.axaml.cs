using Avalonia;
using Avalonia.Controls;
using Lab1.Class;
using Lab1.ViewModels;
using LiveChartsCore.Defaults;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

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
    }
    private void LoadQuantilies(string path)
    {
        if (Directory.Exists(path)) 
        {
            //path = path + "\\Quantilies";
            var files = Directory.GetFiles(path);
            string[] lines = File.ReadAllLines(files[0]);
            foreach (string line in lines)
            {
                string[] parts = line.Split(' ');
                int key = int.Parse(parts[0]);
                double value = double.Parse(parts[1]);
                Class.StatisticalCharacteristics.Quantilies.Add(key, value);
            }
            //try
            //{
            //    using (StreamReader reader = new StreamReader(path))
            //    {
            //        string line;
            //        MainViewModel.PrimaryData.Clear();
            //        while ((line = reader.ReadLine()) != null)
            //        {
            //            if (double.TryParse(line, out var value))
            //            {
            //                MainViewModel.PrimaryData.Add(value);
            //            }
            //        }
            //        MainViewModel.Data.ProccedData(MainViewModel.PrimaryData);

            //    }
            //}

            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
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

    private void OnFileButtonClick(string filePath)
    {
        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                MainViewModel.PrimaryData.Clear();
                while ((line = reader.ReadLine()) != null)
                {
                    if (double.TryParse(line, out var value))
                    {
                        MainViewModel.PrimaryData.Add(value);
                    }
                }
                MainViewModel.Data.ProccedData(MainViewModel.PrimaryData);
                MainViewModel.ObservablePoints.Clear();
                for (int i = 0; i < MainViewModel.Data.X.Count; i++)
                {
                    MainViewModel.ObservablePoints.Add(new ObservablePoint(MainViewModel.Data.X[i], MainViewModel.Data.F[i]));
                }
                MainViewModel.ClassesPoints.Clear();
                for (int i = 0; i < MainViewModel.Data.Classes.LeftBound.Count; i++)
                {
                    MainViewModel.ClassesPoints.Add(new ObservablePoint(MainViewModel.Data.ClassBoundaries[i], MainViewModel.Data.Classes.RelativeFrequency[i]));
                }
                
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        Console.WriteLine($"Clicked on file: {filePath}");
    }
}
