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

    public static ObservableCollection<double> PrimaryData { get; set; } = new ObservableCollection<double>();
    public static Data Data { get; set; } = new Data();
    public ObservableCollection<DataItem> InstanceData => Data.Items;
    public ObservableCollection<int> InstanceIndex => Data.Classes.Index;
    public ObservableCollection<double> InstanceClass => Data.Classes.LeftBound;
    public ObservableCollection<int> InstanceFrequency => Data.Classes.Frequency;
    public ObservableCollection<double> InstanceRelativeFrequency => Data.Classes.RelativeFrequency;
    public static ObservableCollection<ObservablePoint> ObservablePoints { get; set; } = new ObservableCollection<ObservablePoint>();
    public static ObservableCollection<ObservablePoint> ClassesPoints { get; set; } = new ObservableCollection<ObservablePoint>();
    public ISeries[] Series { get; set; } =
    {
        new StepLineSeries<ObservablePoint>
        {
           Values = ObservablePoints,

           Fill = null
        }
    };
    public ISeries[] HistoSeries { get; set; } =
    {
        new ColumnSeries<ObservablePoint>
        {
            Values = ClassesPoints,
            Padding = 0,
            MaxBarWidth = double.MaxValue,
            
        }
    };
    public Axis[] HistoXAxes { get; set; } =
    {
        new Axis
        {
            Labeler = value => $"{value}",
            CustomSeparators = Data.ClassBoundaries.Select(x => Math.Round(x,2)),

        }
    };
    public Axis[] XAxes { get; set; } =
    {
        new Axis
        {
            Labeler = value => $"{value}",
            CustomSeparators = Data.X.Select(x => Math.Round(x,2)),

        }
    };
    public Axis[] YAxes { get; set; } =
    {
        new Axis
        {
            //Labeler = value => $"{value} F"
        }
    };

}
