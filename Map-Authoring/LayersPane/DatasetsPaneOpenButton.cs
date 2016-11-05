using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayersPane
{
    /// <summary>
    /// Represents the button for opening the datasets pane.
    /// </summary>
    internal class DatasetsPaneOpenButton : Button
    {
        protected override void OnClick()
        {
            var activeMapView = MapView.Active;
            if (null == activeMapView)
            {
                return;
            }

            var selectedLayers = activeMapView.GetSelectedLayers();
            foreach (var selectedLayer in selectedLayers)
            {
                var featureLayer = selectedLayer as FeatureLayer;
                if (null != featureLayer)
                {
                    DatasetsPaneViewModel.Open(featureLayer);
                }
            }
        }
    }
}
