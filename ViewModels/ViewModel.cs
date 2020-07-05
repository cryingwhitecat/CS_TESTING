using InternTest.Commands;
using InternTest.CustomObservables;
using InternTest.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Expression = System.Linq.Expressions.Expression;

namespace InternTest.ViewModels
{
    class ViewModel: DependencyObject,INotifyPropertyChanged
    {
        public ResetCommand ResetCommand { get; }

        public FilterCommand FilterCommand { get; }

        public ObservableCollection<ObservableProp<bool>> VisibleColumns 
        {
            get { return (ObservableCollection<ObservableProp<bool>>)GetValue(VisibleColumnsProperty); }
            set { SetValue(VisibleColumnsProperty, value); }
        }

        public bool Grouped
        {
            get { return (bool)GetValue(GroupedProperty); }
            set { SetValue(GroupedProperty, value); }
        }
        public ObservableCollection<Sales> Sales
        {
            get { return (ObservableCollection<Sales>)GetValue(SalesProperty); }
            set { SetValue(SalesProperty, value); }
        }
        public static readonly DependencyProperty SalesProperty =
            DependencyProperty.Register("Sales", typeof(ObservableCollection<Sales>),
                typeof(MainWindow), new UIPropertyMetadata(null));

        public static readonly DependencyProperty GroupedProperty =
            DependencyProperty.Register("GroupedProperty", typeof(bool),
                typeof(MainWindow), new UIPropertyMetadata(null));

        public static readonly DependencyProperty VisibleColumnsProperty =
            DependencyProperty.Register("VisibleColumns", typeof(ObservableCollection<ObservableProp<bool>>),
                typeof(MainWindow), new UIPropertyMetadata(null));
        public event PropertyChangedEventHandler PropertyChanged;
        internal void RaisePropertyChanged([CallerMemberName] string propName=null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        public List<string> FilterColumns { get; set; }
        public ViewModel()
        {
            FilterCommand = new FilterCommand();
            ResetCommand = new ResetCommand();
            VisibleColumns = new ObservableCollection<ObservableProp<bool>>()
            {
                true, true, true, true, true
            };
            FilterColumns = new List<string>();
            try 
            { 
                using (var db =new SalesDbModel())
                {
                    db.Sales.Load();
                    Sales = db.Sales.Local;
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        public List<Sales> Filter()
        {
            var itemsSource = new List<Sales>();
            using (var db = new SalesDbModel())
            {
                if (FilterColumns.Count == 0 || Grouped )
                    return db.Sales.Local.ToList();

                    var items = new List<List<Sales>>();
                items = RecursiveGroupBy(db.Sales.ToList(), FilterColumns);

                foreach (var item in items)
                {
                    var representativeRow = item[0];
                    for (int i = 1; i < item.Count; i++)
                    {
                        representativeRow.SalesSum += item[i].SalesSum;
                        representativeRow.Amount += item[i].Amount;
                    }
                    itemsSource.Add(representativeRow);
                }
            }

            Grouped = true;
            RaisePropertyChanged("Grouped");
            Sales = new ObservableCollection<Sales>(itemsSource);
            RaisePropertyChanged("Sales");
            return itemsSource;
        }

        private List<List<Sales>> RecursiveGroupBy(List<Sales> source, List<string> fields)
        {
            if (fields.Count == 0)
            {
                return new List<List<Sales>>() { source };
            }
            var lambda = GroupByExpression<Sales>(fields[0]);
            var grouped = source.GroupBy(lambda.Compile());
            var result = new List<List<Sales>>();
            fields.RemoveAt(0);
            foreach (var g in grouped)
            {
                var filtered = RecursiveGroupBy(g.ToList(), fields);
                result.AddRange(filtered);

            }
            return result;
        }
        public Expression<Func<TItem, object>> GroupByExpression<TItem>(string propertyName)
        {
            var property = typeof(TItem).GetProperty(propertyName);
            var propertyType = property.PropertyType;
            var param = Expression.Parameter(typeof(TItem), "item");
            var body = Expression.Property(param, property);
            if (propertyType.Name == "System.DateTime" || propertyType.Name == "DateTime")
            {
                var expression = Expression.Lambda<Func<TItem, DateTime>>(body, param);
                Expression converted = Expression.Convert(expression.Body, typeof(object));
                return Expression.Lambda<Func<TItem, object>>(converted, expression.Parameters);
            }
            var expr = Expression.Lambda<Func<TItem, object>>(body, param);
            return expr;
        }
        public List<int> GetFilterColumns ()
        {
            return VisibleColumns.Where(x => !x.Value).
                Select(x => VisibleColumns.IndexOf(x)).ToList();
        }
        public void Reset()
        {

            for(int i = 0; i < VisibleColumns.Count; i++)
            {
                VisibleColumns[i] = true;
            }
            Grouped = false;
            RaisePropertyChanged("Grouped");
            FilterColumns = new List<string>();
            RaisePropertyChanged("VisibleColumns");
            using (var db = new SalesDbModel())
            {
                db.Sales.Load();
                Sales = new ObservableCollection<Sales>(db.Sales.Local.ToList());
            }
            RaisePropertyChanged("Sales");

        }
    }
}
