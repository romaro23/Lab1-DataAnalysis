using Accord.Statistics.Kernels;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Lab1.ViewModels;
using MathNet.Numerics.Statistics;
using ScottPlot;
using ScottPlot.Interactivity.UserActionResponses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using static Lab1.Class.StatisticalCharacteristics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lab1.Class
{
    public class Data
    {
        public PearsonProbability PearsonProbability { get; set; } = new PearsonProbability();
        public ObservableCollection<DataItem> Items { get; } = new ObservableCollection<DataItem>();
        public ObservableCollection<double> EmpiricalCDF { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> DistributionFunction { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> DensityFunction { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> Frequency { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> EmpiricalCDFQuantilies { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> T { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> Z { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> Variants { get; } = new ObservableCollection<double>();
        public ObservableCollection<Class> Classes { get; } = new ObservableCollection<Class>();
        public ObservableCollection<double> ClassBoundaries { get; set; } = new ObservableCollection<double>();
        public ObservableCollection<double> KDE { get; set; } = new ObservableCollection<double>();
        public ObservableCollection<Row> Rows { get; set; } = new ObservableCollection<Row>();
        public static double M { get; set; } = 0.0;
        public static double Bandwidth { get; set; } = 0.0;
        public StatisticalCharacteristics stats { get; set; }
        public Data()
        {

        }
        public void ProccedData(ObservableCollection<double> data)
        {
            Frequency.Clear();
            Items.Clear();
            EmpiricalCDF.Clear();
            DensityFunction.Clear();
            DistributionFunction.Clear();
            EmpiricalCDFQuantilies.Clear();
            Variants.Clear();
            Classes.Clear();
            ClassBoundaries.Clear();
            KDE.Clear();
            T.Clear();
            Z.Clear();
            //2
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
                //
                EmpiricalCDF.Add(empiricalCDF);
                Variants.Add(v);
            }

            stats = new StatisticalCharacteristics(data, Items, Classes, ClassBoundaries, M, KDE, Bandwidth, DistributionFunction, DensityFunction, Frequency, PearsonProbability);
            stats.SetRow(Rows, "second");
            //foreach(var f in Items)
            //{
            //    var i = MathNet.Numerics.Distributions.Normal.InvCDF(0, 1, f.EmpiricalCDF);
            //    f.EmpiricalCDFQuantile = i;
            //    EmpiricalCDFQuantilies.Add(Math.Round(i, 2));
            //}
            //EmpiricalCDFQuantilies[EmpiricalCDFQuantilies.Count - 1] = 0;
            //Items[Items.Count - 1].EmpiricalCDFQuantile = 0;
            //1
            foreach(var f in Items)
            {
                var t = f.Variant;
                T.Add(t);
                double z;
                if(f.EmpiricalCDF <= 0.5)
                {
                    z = Math.Log(2 * f.EmpiricalCDF);
                }
                else
                {
                    z = -Math.Log(2 * (1 - f.EmpiricalCDF));
                }             
                Z.Add(z);
                
            }
            //
        }
    }
    public class DataItem
    {
        public int Index { get; set; }
        public double Variant { get; set; }
        public int Frequency { get; set; }
        public double RelativeFrequency { get; set; }
        public double EmpiricalCDF { get; set; }
        public double EmpiricalCDFQuantile {  get; set; }
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
        public double tA {  get; set; }
        public double tE { get; set; }
        public double M { get; set; }
        public double H { get; set; }
        public List<double> Sum2 { get; set; }
        public List<double> Sum3 { get; set; }
        public List<double> Sum4 { get; set; }
        public double Mean { get; set; }
        public double Med { get; set; }
        public double S_ { get; set; }
        public double A_ { get; set; }
        public double E_ { get; set; }
        public double MeanDeviation { get; set; }
        public double S_Deviation { get; set; }
        public double A_Deviation { get; set; }
        public double E_Deviation { get; set; }
        public int N { get; set; }
        public double T { get; set; }
        public double MeanLow { get; set; }
        public double MeanHigh { get; set; }
        public double MedLow { get; set; }
        public double MedHigh { get; set; }
        public double S_Low { get; set; }
        public double S_High { get; set; }
        public double A_Low { get; set; }
        public double A_High { get; set; }
        public double E_Low { get; set; }
        public double E_High { get; set; }
        public int J { get; set; }
        public int K { get; set; }
        public double Kernel { get; set; }
        public double F { get; set; }
        public double Bandwidth { get; set; }
        public double Max { get; set; }
        public double Min { get; set; }

        public double U {  get; set; }
        public double Lambda { get; set; }
        public double UDeviation { get; set; }
        public double LambdaDeviation { get; set; }
        public double ULow { get; set; }
        public double LambdaLow { get; set; }
        public double UHigh { get; set; }
        public double LambdaHigh { get; set; }
        public StatisticalCharacteristics(ObservableCollection<double> data, ObservableCollection<DataItem> items, ObservableCollection<Class> classes, ObservableCollection<double> classBoundaries, double M_, ObservableCollection<double> kde, double Bandwidth_, ObservableCollection<double> distribution, ObservableCollection<double> density, ObservableCollection<double> frequency_, PearsonProbability pearsonProbability)
        {
            //3
            M = M_;
            Bandwidth = Bandwidth_;
            Max = items.Max(x => x.Variant);
            Min = items.Min(x => x.Variant);
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

            H = (Max - Min) / M;
            for (int i = 0; i < M + 1; i++)
            {
                classBoundaries.Add(Min + H * i);
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
                //
            }
            //7
            Mean = data.Sum() / data.Count();
            var sorted = data.OrderBy(x => x).ToList();
            if (N % 2 == 0)
            {                
                Med = (sorted[N / 2 - 1] + sorted[N / 2]) / 2.0;
            }
            else {
                Med = sorted[N / 2];
            }
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
            var S = Math.Sqrt(Sum2.Sum() / N);
            var A = Sum3.Sum() / (N * Math.Pow(S, 3));
            var E = Sum4.Sum() / (N * Math.Pow(S, 4)) - 3;
            S_ = Math.Sqrt(Sum2.Sum() / (N - 1));
            A_ = (Math.Sqrt(N * (N- 1)) / (N - 2)) * A;
            E_ = (Math.Pow(N, 2) - 1) / ((N - 2) * (N - 3)) * (E + (6.0 / (N + 1)));
            MeanDeviation = S_ / Math.Sqrt(N);
            S_Deviation = S_ / Math.Sqrt(2 * N);
            A_Deviation = Math.Sqrt((double)(6 * N * (N - 1)) / ((N - 2) * (N + 1) * (N + 3)));
            double firstPart = (N - 3) * (N - 2);
            double secondPart = (N + 3) * (N + 5);
            double result = firstPart * secondPart;
            E_Deviation = Math.Sqrt(24 * N * Math.Pow(N - 1, 2) / result);
            T = GetQuantile(N, 0.05);
            MeanLow = Mean - T * MeanDeviation;
            MeanHigh = Mean + T * MeanDeviation;
            S_Low = S_ - T * S_Deviation;
            S_High = S_ + T * S_Deviation;
            A_Low = A_ - T * A_Deviation;
            A_High = A_ + T * A_Deviation;
            E_Low = E_ - T * E_Deviation;
            E_High = E_ + T * E_Deviation;

            J = (int)(Math.Round((double)N / 2 - 1.96 * (Math.Sqrt(N) / 2)));
            K = (int)(Math.Round((double)N / 2 + 1 + 1.96 * (Math.Sqrt(N) / 2)));
            var sortedData = data.OrderBy(x => x).ToList();
            MedLow = sortedData[J - 1];
            MedHigh = sortedData[K - 1];
            //
            //5
            if(Bandwidth == 0.0)
            {
                Bandwidth = S_ * Math.Pow(N, -1.0 / 5.0);
            }
            double sum = 0;
            foreach (var v in sortedData)
            {
                sum = 0;
                for(int i = 0; i < N; i++)
                {
                    var x = (v - sortedData[i]) / Bandwidth;
                    Kernel = 1 / Math.Sqrt(2 * 3.14) * Math.Exp(-(Math.Pow(x, 2) / 2));
                    sum += Kernel;
                }                
                kde.Add(1 / (N * Bandwidth) * sum);
            }
            //
            //2
            U = Mean;
            double tempSum = 0;
            foreach(var x in items)
            {
                tempSum += Math.Pow(x.Variant, 2);
            }
            tempSum = tempSum / items.Count;
            Lambda = Math.Sqrt(2) / Math.Sqrt(tempSum - Math.Pow(Mean, 2));
            var d1 = (5 * Math.Pow(Lambda, 2)) / (4 * N);
            LambdaDeviation = Math.Sqrt(d1);
            var d2 = 2.0 / N * Math.Pow(Lambda, 2);
            UDeviation = Math.Sqrt(d2);
            ULow = U - T * UDeviation;
            UHigh = U + T * UDeviation;
            LambdaLow = Lambda - T * LambdaDeviation;
            LambdaHigh = Lambda + T * LambdaDeviation;
            //
            //3
           foreach(var x in items)
            {
                double f = 0.0;
                if(x.Variant <= Mean)
                {
                    f = 0.5 * Math.Exp(Lambda * (x.Variant - U));
                }
                else if(x.Variant > Mean)
                {
                    f = 1 - 0.5 * Math.Exp(-Lambda * (x.Variant - U));
                    
                }
                distribution.Add(f);
                f = (Lambda / 2) * Math.Exp(-Lambda * Math.Abs(x.Variant - U));
                density.Add(f);
            }
           //
           //4
           for(int i = 1; i < classBoundaries.Count; i++)
            {
                double p1 = 0.0;
                double p2 = 0.0;
                double x1 = 0.0;
                double x2 = 0.0;
                x1 = classBoundaries[i - 1];
                x2 = classBoundaries[i];
                if (x1 <= Mean)
                {
                    p1 = 0.5 * Math.Exp(Lambda * (x1 - U));
                }
                else if (x1 > Mean)
                {
                    p1 = 1 - 0.5 * Math.Exp(-Lambda * (x1 - U));

                }
                if (x2 <= Mean)
                {
                    p2 = 0.5 * Math.Exp(Lambda * (x2 - U));
                }
                else if (x2 > Mean)
                {
                    p2 = 1 - 0.5 * Math.Exp(-Lambda * (x2 - U));

                }
                frequency_.Add(N * (p2 - p1));
            }
            double x_ = 0.0;
            for(int i = 0; i < classes.Count; i++)
            {
               x_ += Math.Pow(classes[i].Frequency - frequency_[i], 2) / frequency_[i];
            }
            var v_ = M - 1;
            double x_cr = MathNet.Numerics.Distributions.ChiSquared.InvCDF(v_, 0.95);
            double p_value = 1 - MathNet.Numerics.Distributions.ChiSquared.CDF(v_, x_);
            pearsonProbability.X = x_;
            pearsonProbability.X_CR = x_cr;
            pearsonProbability.P = p_value;
            //
        }
        public class PearsonProbability
        {
            public double X {  get; set; }
            public double X_CR {  get; set; }
            public double P { get; set; }
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
        private double GetQuantile(int N, double alpha)
        {
            return MathNet.Numerics.Distributions.StudentT.InvCDF(0.0, 1.0, (double)N - 1, 1 - alpha / 2);
        }
        public void SetRow(ObservableCollection<Row> Rows, string option)
        {
            Rows.Clear();
            if (option == "first")
            {
                Rows.Add(new Row("Середнє арифметичне", Mean, MeanDeviation, MeanLow, MeanHigh));
                Rows.Add(new Row("Медіана", Med, 0, MedLow, MedHigh));
                Rows.Add(new Row("Середньоквадратичне відхилення", S_, S_Deviation, S_Low, S_High));
                Rows.Add(new Row("Коефіцієнт асиметрії", A_, A_Deviation, A_Low, A_High));
                Rows.Add(new Row("Коефіцієнт ексцесу", E_, E_Deviation, E_Low, E_High));
                Rows.Add(new Row("Мінімум", Min, 0, 0, 0));
                Rows.Add(new Row("Максимум", Max, 0, 0, 0));
            }
            if(option == "second")
            {
                Rows.Add(new Row("Lambda", Lambda, LambdaDeviation, LambdaLow, LambdaHigh));
                Rows.Add(new Row("u", U, UDeviation, ULow, UHigh));
            }
           
        }
        //8
        public List<double> FindAnomalies(ObservableCollection<double> data, out double leftInterval, out double rightInterval)
        {
            leftInterval = Mean - 1.96 * S_;
            rightInterval = Mean + 1.96 * S_;
            double li = leftInterval;
            double ri = rightInterval;
            List<double> anomalies = new List<double>();
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i] < li || data[i] > ri)
                {
                    anomalies.Add(i);
                }
            }
            return anomalies;
        }
        public void RemoveAnomalies(ObservableCollection<double> data)
        {
            double leftInterval = Mean - 1.96 * S_;
            double rightInterval = Mean + 1.96 * S_;
            var filteredData = data.Where(value => value >= leftInterval && value <= rightInterval).ToList();
            data.Clear();
            foreach (var value in filteredData)
            {
                data.Add(value);
            }
        }
        //9
        public bool IsAsymmetryZero()
        {
            var t = A_ / A_Deviation;
            T = GetQuantile(N, 0.05);
            if (t < 0) { t = t * -1; }
            return (t <= T) ? true : false;
        }
        public bool IsExcessZero()
        {
            var t = E_ / E_Deviation;
            T = GetQuantile(N, 0.05);
            if (t < 0) { t = t * -1; }
            return (t <= T) ? true : false;
        }
    }
}