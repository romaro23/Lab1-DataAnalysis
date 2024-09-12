namespace Lab1.ViewModels;

using Lab1.Class;
using Lab1.Views;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Linq;

public class MainViewModel : ViewModelBase
{
    public static ObservableCollection<double> PrimaryData { get; set; } = new ObservableCollection<double>();
    public static Data Data { get; set; } = new Data();
    public ObservableCollection<DataItem> InstanceData => Data.Items;
    public ISeries[] Series { get; set; } =
    {
        new ColumnSeries<double>
        {
           Values = PrimaryData,

            Padding = 0,

            MaxBarWidth = double.PositiveInfinity
        }
    };


}
