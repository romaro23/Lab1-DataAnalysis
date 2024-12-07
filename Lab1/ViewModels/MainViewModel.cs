namespace Lab1.ViewModels;

using Lab1.Class;
using System.Collections.ObjectModel;
public class MainViewModel : ViewModelBase
{
    public static Data Data { get; set; } = new Data();
    public ObservableCollection<DataItem> InstanceData => Data.Items;
    public ObservableCollection<Class> InstanceClass => Data.Classes;
    public ObservableCollection<StatisticalCharacteristics.Row> Rows => Data.Rows;
    public ObservableCollection<DataRow> CombInstanceData { get; set; }
    public Criteria Criteria { get; set; }
    public ObservableCollection<StatisticalCharacteristics.Row> FirstSample => Criteria.FirstSampleRows;
    public ObservableCollection<StatisticalCharacteristics.Row> SecondSample => Criteria.SecondSampleRows;
    public ObservableCollection<StatisticalCharacteristics.Row> DiffSample => Criteria.DiffSampleRows;
    public MainViewModel()
    {
        CombInstanceData = new ObservableCollection<DataRow>();
        Criteria = new Criteria();
    }

    public class DataRow
    {
        public int Index { get; set; }
        public double FirstValue { get; set; }
        public double SecondValue { get; set; }
    }
}
