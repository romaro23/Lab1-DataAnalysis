using Avalonia.Controls;
using System;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace Lab1.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Content = new MainView {ParentWindow = this};
    }

    public void SetMainView()
    {
        Content = new MainView { ParentWindow = this };
    }

    public void SetSecondView()
    {
        Content = new SecondView { ParentWindow = this };
    }
}
