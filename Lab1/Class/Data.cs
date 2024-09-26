using Lab1.ViewModels;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static Lab1.Class.StatisticalCharacteristics;
namespace Lab1.Class
{
    public class Data
    {
        public ObservableCollection<DataItem> Items { get; } = new ObservableCollection<DataItem>();
        public ObservableCollection<double> F { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> X { get; } = new ObservableCollection<double>();
        public ObservableCollection<Class> Classes { get; } = new ObservableCollection<Class>();
        public ObservableCollection<double> ClassBoundaries { get; set; } = new ObservableCollection<double>();
        public ObservableCollection<Row> Rows { get; set; } = new ObservableCollection<Row>();
        public static double M { get; set; } = 0.0;
        public StatisticalCharacteristics stats { get; set; }
        public Data()
        {

        }
        public void ProccedData(ObservableCollection<double> data)
        {
            Items.Clear();
            F.Clear();
            X.Clear();
            Classes.Clear();
            ClassBoundaries.Clear();
            var variants = data.Distinct().OrderBy(x => x).ToList();
            double empiricalCDF = 0.0;
            foreach (double v in variants)
            {
                int frequency = data.Count(x => x == v);
                double relativeFrequency = (double)frequency / (double)data.Count;
                empiricalCDF += relativeFrequency;

                Items.Add(new DataItem
                {
                    Index = Items.Count() + 1,
                    Variant = v,
                    Frequency = frequency,
                    RelativeFrequency = relativeFrequency,
                    EmpiricalCDF = empiricalCDF
                });
                F.Add(empiricalCDF);
                X.Add(v);
            }

            stats = new StatisticalCharacteristics(data, Items, Classes, ClassBoundaries, M);
            stats.SetRow(Rows);

        }
    }
    public class DataItem
    {
        public int Index { get; set; }
        public double Variant { get; set; }
        public int Frequency { get; set; }
        public double RelativeFrequency { get; set; }
        public double EmpiricalCDF { get; set; }
    }
    public class Class
    {
        public int Index { get; set; }
        public double LeftBound { get; set; }
        public int Frequency { get; set; }
        public double RelativeFrequency { get; set; }
        public double EmpiricalCDF { get; set; }
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
        public double Med { get; set; }
        public double S { get; set; }
        public double S_ { get; set; }
        public double W { get; set; }
        public double A { get; set; }
        public double A_ { get; set; }
        public double E { get; set; }
        public double E_ { get; set; }
        public double X { get; set; }
        public double MeanDeviation { get; set; }
        public double S_Deviation { get; set; }
        public double WDeviation { get; set; }
        public double ADeviation { get; set; }
        public double A_Deviation { get; set; }
        public double EDeviation { get; set; }
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
        
        public StatisticalCharacteristics(ObservableCollection<double> data, ObservableCollection<DataItem> items, ObservableCollection<Class> classes, ObservableCollection<double> classBoundaries, double M_)
        {
            M = M_;
            double max = items.Max(x => x.Variant);
            double min = items.Min(x => x.Variant);
            N = data.Count();
            if (M == 0.0)
            {
                if (N >= 100)
                {
                    var sqr = (int)Math.Pow((double)N, (double)1 / 3);
                    M = (sqr % 2 == 0) ? sqr - 1 : sqr;
                }
                else if (N < 100)
                {
                    var sqr = (int)Math.Sqrt(N);
                    M = (sqr % 2 == 0) ? sqr - 1 : sqr;
                }
            }

            H = (max - min) / M;
            for (int i = 0; i < M + 1; i++)
            {
                classBoundaries.Add(min + H * i);
            }
            double empiricalCDF = 0.0;
            for (int i = 0; i < M; i++)
            {

                int frequency = data.Count(x => x >= classBoundaries[i] && x < classBoundaries[i + 1]);
                if (i == M - 1)
                {
                    frequency = data.Count(x => x >= classBoundaries[i] && x <= classBoundaries[i + 1]);
                }
                empiricalCDF += frequency / (double)N;
                classes.Add(new Class
                {
                    Index = i + 1,
                    LeftBound = classBoundaries[i],
                    Frequency = frequency,
                    RelativeFrequency = frequency / (double)N,
                    EmpiricalCDF = empiricalCDF
                });
            }
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
            WDeviation = W * Math.Sqrt((1 + 2 * Math.Pow(W, 2)) / (2 * N));
            ADeviation = Math.Sqrt((double)(6 * (N - 2)) / ((N + 1) * (N + 3)));
            A_Deviation = 0.4479;
            EDeviation = Math.Sqrt((24 * N * (N - 2) * (N - 3)) / (Math.Pow((N + 1), 2) * (N + 3) * (N + 5)));
            E_Deviation = 0.8721;
            if (N > 0 && N <= 120)
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
        }
        public class Row
        {
            public string InstanceCharacterisation { get; set; }
            public double Estimate { get; set; }
            public double Deviation { get; set; }
            public double LeftInterval { get; set; }
            public double RightInterval { get; set; }
            public Row(string name, double estimate, double deviation, double leftInterval, double rightInterval)
            {
                InstanceCharacterisation = name;
                Estimate = estimate;
                Deviation = deviation;
                LeftInterval = leftInterval;
                RightInterval = rightInterval;
            }
        }
        public void SetRow(ObservableCollection<Row> Rows)
        {
            Rows.Clear();
            Rows.Add(new Row("Середнє арифметичне", Mean, MeanDeviation, MeanLow, MeanHigh));
            Rows.Add(new Row("Медіана", Med, 0, MedLow, MedHigh));
            Rows.Add(new Row("Середньоквадратичне відхилення", S_, S_Deviation, S_Low, S_High));
            Rows.Add(new Row("Коефіцієнт асиметрії", A_, A_Deviation, A_Low, A_High));
            Rows.Add(new Row("Коефіцієнт ексцесу", E_, E_Deviation, E_Low, E_High));
        }
    }
}