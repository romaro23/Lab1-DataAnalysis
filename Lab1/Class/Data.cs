using Avalonia.Controls.Platform;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Statistics;
namespace Lab1.Class
{
    public class Data
    {
        public ObservableCollection<DataItem> Items { get; } = new ObservableCollection<DataItem>();
        private double h {  get; set; }
        private int M { get; set; }
        public List<double> ClassBoundaries { get; set; } = new List<double>();
        public Data()
        {
            
        }
        public void ProccedData(ObservableCollection<double> data)
        {
            data.Remove(3.8);
            data.Remove(5.0);
            Items.Clear();
            var variants = data.Distinct().OrderBy(x => x).ToList();
            foreach (double v in variants)
            {
                int frequency = data.Count(x => x == v);
                Items.Add(new DataItem
                {
                    Index = Items.Count() + 1,
                    Variant = v,
                    Frequency = frequency,
                    RelativeFrequency = frequency / (double)data.Count
                });
            }
            
            StatisticalCharacteristics stats = new StatisticalCharacteristics(data, Items);
            
            //for (int i = 0; i < M + 1; i++)
            //{
            //    ClassBoundaries.Add(min + h * i);
            //}
            }
    }
    public class DataItem
    {
        public int Index { get; set; }
        public double Variant { get; set; }
        public int Frequency { get; set; }
        public double RelativeFrequency { get; set; }
    }
    public class StatisticalCharacteristics
    {
        public static Dictionary<int, double> Quantilies = new Dictionary<int, double>();
        public double M { get; set; }
        public double H { get; set; }
        public List<double> Sum2 { get; set; }
        public List<double> Sum3 { get; set; }
        public List<double> Sum4 { get; set; }
        public double Mean { get; set; }
        public double Med {  get; set; }
        public double S {  get; set; }
        public double S_ { get; set; }
        public double W { get; set; }
        public double A { get; set; }
        public double A_ { get;set; }
        public double E { get; set; }
        public double E_ { get; set; }
        public double X { get; set; }
        public double MeanDeviation { get; set; }
        public double S_Deviation { get; set; }
        public double WDeviation { get; set; }
        public double ADeviation { get; set; }
        public double A_Deviation { get; set; }
        public double EDeviation  { get; set; }
        public double E_Deviation { get; set; }
        public int N { get; set; }
        public double T { get; set; }
        public double MeanLow { get; set; }
        public double MeanHigh { get; set; }
        public double MedLow { get; set; }
        public double MedHigh { get; set; }
        public double S_Low { get; set; }
        public double S_High { get; set; }
        public double WLow { get; set; }
        public double WHigh { get; set; }
        public double ALow { get; set; }
        public double AHigh { get; set; }
        public double A_Low { get; set; }
        public double A_High { get; set; }
        public double ELow { get; set; }
        public double EHigh { get; set; }
        public double E_Low { get; set; }
        public double E_High { get; set; }
        public int J { get; set; }
        public int K { get; set; }
        public StatisticalCharacteristics(ObservableCollection<double> data, ObservableCollection<DataItem> items)
        {
            double max = items.Max(x => x.Variant);
            double min = items.Min(x => x.Variant);
            N = data.Count();
            M = (int)Math.Sqrt(N);            
            H = (max - min) / M;
            Mean = Statistics.Mean(data);
            Med = Statistics.Median(data);
            Sum2 = new List<double>();
            Sum3 = new List<double>();
            Sum4 = new List<double>();
            for (int i = 0; i < data.Count; i++)
            {
                var x = data[i] - Mean;
                Sum2.Add(Math.Pow(x, 2));
                Sum3.Add(Math.Pow(x, 3));
                Sum4.Add(Math.Pow(x, 4));
            }
            S = Math.Sqrt(Sum2.Sum() / N);
            S_ = Statistics.StandardDeviation(data);
            W = S_ / Mean;
            A = Sum3.Sum() / (N * Math.Pow(S, 3));
            A_ = Statistics.Skewness(data);
            E = Sum4.Sum() / (N * Math.Pow(S, 4)) - 3;
            E_ = Statistics.Kurtosis(data);
            X = 1 / Math.Sqrt(E + 3);
            MeanDeviation = S_ / Math.Sqrt(N);
            S_Deviation = S_ / Math.Sqrt(2 * N);
            WDeviation = W * Math.Sqrt((1 + 2 * Math.Pow(W,2)) / (2 * N));
            ADeviation = Math.Sqrt((double)(6 * (N - 2)) / ((N + 1) * (N + 3)));
            A_Deviation = 0.4479;
            EDeviation = Math.Sqrt((24 * N * (N - 2) * (N - 3)) / (Math.Pow((N + 1), 2) * (N + 3) * (N + 5)));
            E_Deviation = 0.8721;
            if(N > 0 && N <= 120)
            {
                T = Quantilies[N - 1];
            }
            else
            {
                T = 1.96;
            }

            MeanLow = Mean - T * MeanDeviation;
            MeanHigh = Mean + T * MeanDeviation;
            S_Low = S - T * S_Deviation;
            S_High = S + T * S_Deviation;
            WLow = W - T * WDeviation;
            WHigh = W + T * WDeviation;
            ALow = A - T * ADeviation;
            AHigh = A + T * ADeviation;
            A_Low = A_ - T * A_Deviation;
            A_High = A_ + T * A_Deviation;
            ELow = E - T * EDeviation;
            EHigh = E + T * EDeviation;
            E_Low = E_ - T * E_Deviation;
            E_High = E_ + T * E_Deviation;
            
            J = (int)(Math.Round((double)N / 2 - 1.96 * (Math.Sqrt(N) / 2)));
            K = (int)(Math.Round((double)N / 2 + 1 + 1.96 * (Math.Sqrt(N) / 2)));
            var sortedData = data.OrderBy(x => x).ToList();
            MedLow = sortedData[J - 1];
            MedHigh = sortedData[K - 1];
            var leftInterval = Mean - 1.96 * S_;
            var rightInterval = Mean + 1.96 * S_;
            var i1 = 1;
        }
    }
}
