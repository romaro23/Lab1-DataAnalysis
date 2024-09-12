using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Lab1.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;

namespace Lab1;

public partial class StartView : UserControl
{
    public static event EventHandler DataLoaded;
    public static List<double> data = new List<double> { };
    public StartView()
    {
        InitializeComponent();

        string baseDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\Lab1"));
        string relativePath = "Assets\\data_lab1,2";
        string fullPath = Path.Combine(baseDirectory, relativePath);
        CreateButtonsForDirectories(fullPath);
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
                while ((line = reader.ReadLine()) != null)
                {
                    if (double.TryParse(line, out var value))
                    {
                        data.Add(value);
                    }
                }
            }
            DataLoaded?.Invoke(null, EventArgs.Empty);
        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        Console.WriteLine($"Clicked on file: {filePath}");
    }

}