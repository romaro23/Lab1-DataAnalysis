namespace Lab1.ViewModels;

using Lab1.Class;
using Lab1.Views;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using ReactiveUI;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System;
public class MainViewModel : ViewModelBase
{

    public static Data Data { get; set; } = new Data();
    public ObservableCollection<DataItem> InstanceData => Data.Items;
    public ObservableCollection<Class> InstanceClass => Data.Classes;

}
