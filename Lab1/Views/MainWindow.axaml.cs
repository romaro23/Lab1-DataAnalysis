using Avalonia.Controls;
using System;

namespace Lab1.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        StartView.DataLoaded += StartView_DataLoaded;
    }

    private void StartView_DataLoaded(object? sender, EventArgs e)
    {
        this.DataContext = new MainView();    
    }

}
