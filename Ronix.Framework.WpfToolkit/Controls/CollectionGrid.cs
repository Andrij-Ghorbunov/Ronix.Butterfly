using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Ronix.Framework.WpfToolkit.Controls
{
    public class CollectionGrid : ItemsControl
    {
        #region Dependency properties

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(ColumnCollection), typeof(CollectionGrid), new PropertyMetadata(null, ColumnsUpdated));

        public static readonly DependencyProperty IsTransposedProperty =
            DependencyProperty.Register("IsTransposed", typeof(bool), typeof(CollectionGrid), new PropertyMetadata(false, ColumnsUpdated));

        public ColumnCollection Columns
        {
            get { return (ColumnCollection)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        /// <summary>
        /// If set to true, switches columns and rows of the grid. Column definitions become row definitions, and their widths become heights.
        /// </summary>
        public bool IsTransposed
        {
            get { return (bool)GetValue(IsTransposedProperty); }
            set { SetValue(IsTransposedProperty, value); }
        }

        #endregion Dependency properties

        #region Fields

        private static readonly Binding DefaultBinding;

        private Grid _grid;

        #endregion Fields

        #region Constructor

        static CollectionGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CollectionGrid), new FrameworkPropertyMetadata(typeof(CollectionGrid)));
            DefaultBinding = new Binding();
        }

        public CollectionGrid()
        {
            Columns = new ColumnCollection();
        }

        #endregion Constructor

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _grid = (Grid)GetTemplateChild("Root");
            Update();
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            Update();
        }
        
        private static void ColumnsUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = (CollectionGrid)d;
            sender.Update();
        }

        private void Update()
        {
            if (_grid == null)
                return;
            
            ClearGrid();
            if (IsTransposed)
                SetupTransposed();
            else
                SetupNormal();
        }

        private void ClearGrid()
        {
            _grid.ColumnDefinitions.Clear();
            _grid.RowDefinitions.Clear();
            _grid.Children.Clear();
        }
        
        private void SetupNormal()
        {
            var items = ItemsSource == null ? Items.OfType<object>().ToList() : ItemsSource.OfType<object>().ToList();
            int w = Columns.Count;
            int h = items.Count();
            foreach (var column in Columns)
            {
                _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = column.Width });
            }
            for (int j = 0; j < h; j++)
            {
                _grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
            for (int i = 0; i < w; i++)
            {
                var column = Columns[i];
                for (int j = 0; j < h; j++)
                {
                    var e = new ContentControl
                        {
                            DataContext = items[j],
                            HorizontalContentAlignment = HorizontalContentAlignment,
                            VerticalContentAlignment = VerticalContentAlignment
                        };
                    column.ApplyToControl(e);
                    e.SetBinding(ContentControl.ContentProperty, DefaultBinding);
                    Grid.SetRow(e, j);
                    Grid.SetColumn(e, i);
                    _grid.Children.Add(e);
                }
            }
        }

        private void SetupTransposed()
        {
            var items = ItemsSource == null ? Items.OfType<object>().ToList() : ItemsSource.OfType<object>().ToList();
            int w = Columns.Count;
            int h = items.Count();
            foreach (var column in Columns)
            {
                _grid.RowDefinitions.Add(new RowDefinition { Height = column.Width });
            }
            for (int j = 0; j < h; j++)
            {
                _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }
            for (int i = 0; i < w; i++)
            {
                var column = Columns[i];
                for (int j = 0; j < h; j++)
                {
                    var e = new ContentControl
                    {
                        DataContext = items[j],
                        HorizontalContentAlignment = HorizontalContentAlignment,
                        VerticalContentAlignment = VerticalContentAlignment
                    };
                    column.ApplyToControl(e);
                    e.SetBinding(ContentControl.ContentProperty, DefaultBinding);
                    Grid.SetRow(e, i);
                    Grid.SetColumn(e, j);
                    _grid.Children.Add(e);
                }
            }
        }

        #endregion Methods
    }
}
