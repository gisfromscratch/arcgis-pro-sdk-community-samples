using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LayersPane
{
    /// <summary>
    /// Represents the view model for displaying datasets statistics like number of rows.
    /// </summary>
    internal class DatasetsPaneViewModel : ViewStatePane, INotifyPropertyChanged
    {
        private const string ViewPaneID = "LayersPane_DatasetsPane";
        private const string ViewDefaultPath = "DatasetsPaneViewModel_Pane_View_Path";

        private readonly string _path;
        private readonly FeatureLayer _layer;
        private long _rowCount;

        private static ConcurrentDictionary<FeatureLayer, DatasetsPaneViewModel> _viewModels;        

        static DatasetsPaneViewModel()
        {
            _viewModels = new ConcurrentDictionary<FeatureLayer, DatasetsPaneViewModel>();
        }

        /// <summary>
        /// Creates a new view model and wires up the specified CIM view.
        /// </summary>
        /// <param name="view">The CIM view.</param>
        /// <param param name="layer">The feature layer of the pane.</param>
        public DatasetsPaneViewModel(CIMView view, FeatureLayer layer) : base(view)
        {
            _path = view.ViewXML;
            _layer = layer;
            _viewModels.TryAdd(layer, this);
            Caption = string.Format(@"Pane {0}", layer.Name);
        }

        public long RowCount
        {
            get
            {
                return _rowCount;
            }

            set
            {
                SetProperty(ref _rowCount, value, () => RowCount);
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// The current view state of this pane.
        /// </summary>
        public override CIMView ViewState
        {
            get
            {
                var view = new CIMGenericView();
                view.ViewType = ViewPaneID;
                view.ViewXML = _path;
                view.InstanceID = (int)InstanceID;
                return view;
            }
        }

        protected async override Task InitializeAsync()
        {
            await QueuedTask.Run(() =>
            {
                var featureClass = _layer.GetFeatureClass();
                var cursor = featureClass.Search();
                long rowCount = 0;
                while (cursor.MoveNext())
                {
                    rowCount++;
                }

                RowCount = rowCount;
                //Interlocked.Exchange(ref _rowCount, rowCount);
            });
        }

        protected async override Task UninitializeAsync()
        {
            await base.UninitializeAsync();
        }

        /// <summary>
        /// Create a new instance of the pane.
        /// </summary>
        /// <param param name="layer">The feature layer of the pane.</param>
        /// <returns>A new instance or <c>null</c>.</returns>
        internal static DatasetsPaneViewModel Create(FeatureLayer layer)
        {
            var view = new CIMGenericView();
            view.ViewType = ViewPaneID;
            view.ViewXML = ViewDefaultPath;
            var pane = FrameworkApplication.Panes.Create(ViewPaneID, new object[] { view, layer });
            return pane as DatasetsPaneViewModel;
        }

        /// <summary>
        /// Opens the datasets pane.
        /// </summary>
        /// <param param name="layer">The feature layer of the pane.</param>
        /// <returns>The view model of the datasets pane.</returns>
        internal static DatasetsPaneViewModel Open(FeatureLayer layer)
        {
            DatasetsPaneViewModel viewModel;
            if (_viewModels.TryGetValue(layer, out viewModel))
            {
                viewModel = FrameworkApplication.Panes.FindPane(viewModel.InstanceID) as DatasetsPaneViewModel;
                if (null == viewModel)
                {
                    // View model was recycled by the framework?
                    _viewModels.TryRemove(layer, out viewModel);
                    viewModel = null;
                }
                else
                {
                    // Just activate
                    viewModel.Activate();
                }
            }
            if (null == viewModel)
            {
                viewModel = Create(layer);
            }
            if (null == viewModel)
            {
                // The view mode could not be obtained!
                return null;
            }

            Project.Current.SetDirty();
            return viewModel;
        }
    }
}
