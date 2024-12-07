using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using LiveChartsCore.Motion;
using ScottPlot.Plottables;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Lab1.Class.StatisticalCharacteristics;

namespace Lab1.Class
{
    public class Criteria
    {
        private ObservableCollection<double> _firstSample;
        public ObservableCollection<double> FirstSample
        {
            get => _firstSample;
            set
            {
                _firstSample = value;
            }
        }
        private ObservableCollection<double> _secondSample;
        public ObservableCollection<double> SecondSample
        {
            get => _secondSample;
            set
            {
                _secondSample = value;
            }
        }
        public ObservableCollection<Row> FirstSampleRows { get; set; } = new ObservableCollection<Row>();
        public ObservableCollection<Row> SecondSampleRows { get; set; } = new ObservableCollection<Row>();
        public ObservableCollection<Row> DiffSampleRows { get; set; } = new ObservableCollection<Row>();
        public void SetRows()
        {
            var firstSample = FirstSample.ToList();
            firstSample.RemoveAll(double.IsNaN);
            var secondSample = SecondSample.ToList();
            secondSample.RemoveAll(double.IsNaN);
            CalculateSample(FirstSampleRows, firstSample);
            CalculateSample(SecondSampleRows, secondSample);
            if (SecondSampleRows.Count == FirstSampleRows.Count)
            {
                var diffSample = firstSample.Zip(secondSample, (x, y) => x - y).ToList();
                CalculateSample(DiffSampleRows, diffSample);
            }
        }
        private double Mean(IEnumerable<double> sample)
        {
            var sample_ = sample.ToList();
            return sample.Sum() / sample.Count();
        }
        private double StandardDeviation(IEnumerable<double> sample, double mean, int n)
        {
            var sample_ = sample.ToList();
            List<double> sum2 = new List<double>();
            List<double> sum3 = new List<double>();
            List<double> sum4 = new List<double>();
            for (int i = 0; i < n; i++)
            {
                var x = sample_[i] - mean;
                sum2.Add(Math.Pow(x, 2));
                sum3.Add(Math.Pow(x, 3));
                sum4.Add(Math.Pow(x, 4));
            }
            double s = Math.Sqrt(sum2.Sum() / (n - 1));
            return s;
        }

        public (bool, double, double) IndepMeanEquality()
        {
            if (FirstSample.Count != SecondSample.Count)
            {
                return (false, 0 , 0);
            }

            var firstSample = FirstSample.ToList();
            firstSample.RemoveAll(double.IsNaN);
            var secondSample = SecondSample.ToList();
            secondSample.RemoveAll(double.IsNaN);
            double mean1 = Mean(firstSample);
            int n1 = firstSample.Count;
            double s1 = StandardDeviation(firstSample, mean1, n1);
            double mean2 = Mean(secondSample);
            int n2 = secondSample.Count;
            double s2 = StandardDeviation(secondSample, mean2, n2);
            double variance1 = Math.Pow(s1, 2);
            double variance2 = Math.Pow(s2, 2);
            double f = variance1 / variance2;
            double v1 = n1 - 1;
            double v2 = n2 - 1;
            double fisher = GetFisherQuantile(v1, v2, 0.05);
            double t;
            double s = ((n1 - 1) * variance1 + (n2 - 1) * variance2) / (n1 + n2 - 2);
            double v = 0;
            if (f <= fisher)
            {
                t = (mean1 - mean2) / Math.Sqrt(s / n1 + s / n2);
                v = n1 + n2 - 2;
            }
            else
            {
                t = (mean1 - mean2) / Math.Sqrt(variance1 / n1 + variance2 / n2);
                v = Math.Pow(variance1 / n1 + variance2 / n2, 2) * Math.Pow((1.0 / (n1 - 1) * Math.Pow(variance1 / n1, 2)) + (1.0 / (n2 - 1)) * Math.Pow(variance2 / n2, 2),-1);
            }
            t = Math.Abs(t);
            double student = GetStudentQuantile(v, 0.05);
            if (t <= student) return (true, t, student);
            return (false, t, student);
        }
        public (bool, double, double) IndepVanDerVardenEquality()
        {
            var firstSample = FirstSample.ToList();
            firstSample.RemoveAll(double.IsNaN);
            var secondSample = SecondSample.ToList();
            secondSample.RemoveAll(double.IsNaN);
            var firstSampleWithIndex = firstSample.Select(value => Tuple.Create(1, value));
            var secondSampleWithIndex = secondSample.Select(value => Tuple.Create(2, value));
            var combSample = firstSampleWithIndex.Concat(secondSampleWithIndex).ToList();
            var ranks = CalculateRanks(combSample);
            List<Tuple<double, double>> firstValuesAndRanks = new List<Tuple<double, double>>();
            List<Tuple<double, double>> secondValuesAndRanks = new List<Tuple<double, double>>();
            for (int i = 0; i < firstSample.Count; i++)
            {
                var rank = ranks.FirstOrDefault(x => x.Item2 == firstSample[i]);
                firstValuesAndRanks.Add(new Tuple<double, double>(firstSample[i], rank.Item3));
            }
            for (int i = 0; i < secondSample.Count; i++)
            {
                var rank = ranks.FirstOrDefault(x => x.Item2 == secondSample[i]);
                secondValuesAndRanks.Add(new Tuple<double, double>(secondSample[i], rank.Item3));
            }

            double x = 0;
            double y = 0;
            double n = combSample.Count;
            
            if (combSample.GroupBy(x => x.Item2).Any(y => y.Count() > 1))
            {
                var groups = combSample
                    .GroupBy(x => x.Item2)
                    .Where(g => g.Select(x => x.Item1).Distinct().Count() > 1)
                    .Select(g => g.ToList())
                    .ToList();
                var groupsValue = groups.SelectMany(g => g.Select(x => x.Item2));
                var firstWithoutGroup = firstValuesAndRanks.Where(item => !groupsValue.Contains(item.Item1)).ToList();
                var secondWithoutGroup = secondValuesAndRanks.Where(item => !groupsValue.Contains(item.Item1)).ToList();
                foreach (var item in firstWithoutGroup)
                {
                    double normalizedRank = item.Item2 / (n + 1);
                    double quantile = MathNet.Numerics.Distributions.Normal.InvCDF(0, 1, normalizedRank);
                    x += quantile;
                }
                foreach (var item in secondWithoutGroup)
                {
                    double normalizedRank = item.Item2 / (n + 1);
                    double quantile = MathNet.Numerics.Distributions.Normal.InvCDF(0, 1, normalizedRank);
                    y += quantile;
                }
                double q = 0;
                foreach (var group in groups)
                {
                    double d_ = group.Count;
                    var first = group.Where(g => g.Item1 == 1);
                    double a = first.Count();
                    var second = group.Where(g => g.Item1 == 2);
                    double b = second.Count();
                    var orderedSample = combSample.OrderBy(item => item.Item2).ToList();
                    int index = orderedSample.FindIndex(item => item.Item2 == group[0].Item2) + 1;
                    for (int i = index; i <= index + d_ - 1; i++)
                    {
                        double normalized = i / (n + 1);
                        double quantile = MathNet.Numerics.Distributions.Normal.InvCDF(0, 1, normalized);
                        q += quantile;
                    }

                    x += a / d_ * q;
                    y += b / d_ * q;
                }
                
            }
            else
            {
                foreach (var item in firstValuesAndRanks)
                {
                    double normalizedRank = item.Item2 / (n + 1);
                    double quantile = MathNet.Numerics.Distributions.Normal.InvCDF(0, 1, normalizedRank);
                    x += quantile;
                }
                foreach (var item in secondValuesAndRanks)
                {
                    double normalizedRank = item.Item2 / (n + 1);
                    double quantile = MathNet.Numerics.Distributions.Normal.InvCDF(0, 1, normalizedRank);
                    y += quantile;
                }
            }
            double sum = 0;
            for (int i = 0; i < n; i++)
            {
                double normalized = (i + 1.0) / (n + 1);
                double quantile = MathNet.Numerics.Distributions.Normal.InvCDF(0, 1, normalized);
                sum += Math.Pow(quantile, 2);
            }
            double n1 = firstSample.Count;
            double n2 = secondSample.Count;
            double d = ((n1 * n2) / (n * (n - 1))) * sum;
            double u = x / Math.Sqrt(d);
            u = Math.Abs(u);
            double u_ = MathNet.Numerics.Distributions.Normal.InvCDF(0, 1, 1 - 0.05 / 2);
            if (u <= u_) return (true, u, u_);
            return (false, u, u_);

        }
        public (bool, double, double) DepVarianceEquality()
        {
            var firstSample = FirstSample.ToList();
            firstSample.RemoveAll(double.IsNaN);
            var secondSample = SecondSample.ToList();
            secondSample.RemoveAll(double.IsNaN);
            double mean1 = Mean(firstSample);
            int n1 = firstSample.Count;
            double s1 = StandardDeviation(firstSample, mean1, n1);
            double mean2 = Mean(secondSample);
            int n2 = secondSample.Count;
            double s2 = StandardDeviation(secondSample, mean2, n2);
            double variance1 = Math.Pow(s1, 2);
            double variance2 = Math.Pow(s2, 2);
            double f, v1, v2;
            if (variance1 >= variance2)
            {
                f = variance1 / variance2;
                v1 = n1 - 1;
                v2 = n2 - 1;
            }
            else
            {
                f = variance2 / variance1;
                v1 = n2 - 1;
                v2 = n1 - 1;
            }

            double fisherQuantile = GetFisherQuantile(v1, v2, 0.05);
            if (f <= fisherQuantile) return (true, f, fisherQuantile);
            return (false, f, fisherQuantile);
        }
        public (bool, double, double) DepMeanEquality()
        {
            if (FirstSample.Count != SecondSample.Count)
            {
                return (false, 0, 0);
            }

            var diffSample = FirstSample.Zip(SecondSample, (x, y) => x - y).ToList();
            double mean = Mean(diffSample);
            int n = diffSample.Count;
            double s = StandardDeviation(diffSample, mean, n);
            double t = mean * Math.Sqrt(n) / s;
            double studentQuantile = GetStudentQuantile(n, 0.05);
            if (Math.Abs(t) <= studentQuantile) return (true, t, studentQuantile);
            return (false, t, studentQuantile);
        }

        public (bool, double, double) DepWilcoxonRanks()
        {
            if (FirstSample.Count != SecondSample.Count)
            {   
                return (false, 0, 0);
            }

            var diffSample = FirstSample.Zip(SecondSample, (x, y) => x - y).ToList();
            diffSample.RemoveAll(x => x == 0);
            var absDiffSample = diffSample.Select(x => Math.Abs(x)).ToList();
            List<Tuple<double, double>> ranks = CalculateRanks(absDiffSample);
            List<Tuple<double, double, double>> valuesAndRanks = new List<Tuple<double, double, double>>();
            for (int i = 0; i < absDiffSample.Count; i++)
            {
                double s = diffSample[i] < 0 ? 0 : 1;
                var rank = ranks.FirstOrDefault(x => x.Item1 == absDiffSample[i]);
                valuesAndRanks.Add(new Tuple<double, double, double>(absDiffSample[i], s, rank.Item2));
            }

            double t = 0;
            foreach (var element in valuesAndRanks)
            {
                t += element.Item2 * element.Item3;
            }

            double n = absDiffSample.Count;
            double e = (1.0 / 4 * n) * (n + 1);
            double d = (1.0 / 24 * n) * (n + 1) * (2 * n + 1);
            double u = (t - e) / Math.Sqrt(d);
            u = Math.Abs(u);
            double u_ = MathNet.Numerics.Distributions.Normal.InvCDF(0, 1, 1 - 0.05 / 2);
            if (u <= u_) return (true, u, u_);
            return (false, u, u_);

        }

        private List<Tuple<double, double>> CalculateRanks(IEnumerable<double> sample)
        {
            var orderedSample = sample.OrderBy(x => x).ToList();
            List<Tuple<double, double>> serialNumbers = new List<Tuple<double, double>>();
            List<Tuple<double, double>> ranks = new List<Tuple<double, double>>();
            for (int i = 0; i < orderedSample.Count; i++)
            {
                serialNumbers.Add(new Tuple<double, double>(orderedSample[i], i + 1));
            }

            foreach (var x in serialNumbers.GroupBy(t => t.Item1))
            {
                var key = x.Key;
                var values = x.Select(t => t.Item2).ToList();
                if (values.Count == 1)
                {
                    ranks.Add(new Tuple<double, double>(key, values[0]));
                }
                else
                {
                    ranks.Add(new Tuple<double, double>(key, values.Average()));
                }
            }
            return ranks;
        }
        private List<Tuple<int, double, double>> CalculateRanks(List<Tuple<int, double> >sample)
        {
            var orderedSample = sample.OrderBy(x => x.Item2).ToList();
            List<Tuple<int, double, double>> serialNumbers = new List<Tuple<int, double, double>>();
            List<Tuple<int, double, double>> ranks = new List<Tuple<int, double, double>>();
            for (int i = 0; i < orderedSample.Count; i++)
            {
                serialNumbers.Add(new Tuple<int, double, double>(orderedSample[i].Item1, orderedSample[i].Item2, i + 1));
            }

            foreach (var x in serialNumbers.GroupBy(t => t.Item2))
            {
                var key = x.Key;
                var values = x.ToList();
                if (values.Count == 1)
                {
                    ranks.Add(new Tuple<int, double, double>(values[0].Item1, values[0].Item2, values[0].Item3));
                }
                else
                {
                    ranks.Add(new Tuple<int, double, double>(values[0].Item1, values[0].Item2, values.Average(v => v.Item3)));
                }
            }
            return ranks;
        }
        private double GetStudentQuantile(double n, double alpha)
        {
            return MathNet.Numerics.Distributions.StudentT.InvCDF(0.0, 1.0, n - 1, 1 - alpha / 2);
        }

        private double GetFisherQuantile(double v1, double v2, double alpha)
        {
            return MathNet.Numerics.Distributions.FisherSnedecor.InvCDF(v1, v2, 1 - alpha);
        }

        private double Med(IEnumerable<double> sample, int n)
        {
            double med;
            var sorted = sample.OrderBy(x => x).ToList();
            if (n % 2 == 0)
            {
                med = (sorted[n / 2 - 1] + sorted[n / 2]) / 2.0;
            }
            else
            {
                med = sorted[n / 2];
            }

            return med;
        }
        public void CalculateSample(ObservableCollection<Row> rows, IEnumerable<double> sample_)
        {
            rows.Clear();
            var sample = sample_.ToList();
            int n = sample.Count;
            double mean = Mean(sample);
            double std = StandardDeviation(sample, mean, n);
            double med = Med(sample, n);
            double min = sample.Min();
            double max = sample.Max();
            double meanDeviation = std / Math.Sqrt(n);
            double stdDeviation = std / Math.Sqrt(2 * n);
            double t = GetStudentQuantile(n, 0.05);
            double meanLow = mean - t * meanDeviation;
            double meanHigh = mean + t * meanDeviation;
            double stdLow = std - t * stdDeviation;
            double stdHigh = std + t * stdDeviation;
            int j = (int)(Math.Round((double)n / 2 - 1.96 * (Math.Sqrt(n) / 2)));
            int k = (int)(Math.Round((double)n / 2 + 1 + 1.96 * (Math.Sqrt(n) / 2)));
            var sorted = sample.OrderBy(x => x).ToList();
            double medLow = sorted[j - 1];
            double medHigh = sorted[k - 1];
            rows.Add(new Row("Середнє арифметичне", mean, meanDeviation, meanLow, meanHigh));
            rows.Add(new Row("Медіана", med, 0, medLow, medHigh));
            rows.Add(new Row("Середньоквадратичне відхилення", std, stdDeviation, stdLow, stdHigh));
            rows.Add(new Row("Мінімум", min, 0, 0, 0));
            rows.Add(new Row("Максимум", max, 0, 0, 0));
        }
    }
}
