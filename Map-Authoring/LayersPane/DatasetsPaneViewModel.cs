using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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

        private string _path;

        private static ConcurrentDictionary<string, DatasetsPaneViewModel> _viewModels;        

        static DatasetsPaneViewModel()
        {
            _viewModels = new ConcurrentDictionary<string, DatasetsPaneViewModel>();
        }

        /// <summary>
        /// Creates a new view model and wires up the specified CIM view.
        /// </summary>
        /// <param name="view">The CIM view.</param>
        /// <param param name="internalId">The internal ID of the pane.</param>
        public DatasetsPaneViewModel(CIMView view, string internalId) : base(view)
        {
            _path = view.ViewXML;
            _viewModels.TryAdd(internalId, this);
            Caption = string.Format(@"Pane {0}", internalId);
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
            await base.InitializeAsync();
        }

        protected async override Task UninitializeAsync()
        {
            await base.UninitializeAsync();
        }

        /// <summary>
        /// Create a new instance of the pane.
        /// </summary>
        /// <param param name="internalId">The internal ID of the pane.</param>
        /// <returns>A new instance or <c>null</c>.</returns>
        internal static DatasetsPaneViewModel Create(string internalId)
        {
            var view = new CIMGenericView();
            view.ViewType = ViewPaneID;
            view.ViewXML = ViewDefaultPath;
            var pane = FrameworkApplication.Panes.Create(ViewPaneID, new object[] { view, internalId });
            return pane as DatasetsPaneViewModel;
        }

        /// <summary>
        /// Opens the datasets pane.
        /// </summary>
        /// <param param name="internalId">The internal ID of the pane.</param>
        /// <returns>The view model of the datasets pane.</returns>
        internal static DatasetsPaneViewModel Open(string internalId)
        {
            DatasetsPaneViewModel viewModel;
            if (_viewModels.TryGetValue(internalId, out viewModel))
            {
                // Reference equals?
                //viewModel = FrameworkApplication.Panes.FindPane(viewModel.InstanceID) as DatasetsPaneViewModel;
                
                // Just activate
                viewModel.Activate();
            }
            if (null == viewModel)
            {
                viewModel = Create(internalId);
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
