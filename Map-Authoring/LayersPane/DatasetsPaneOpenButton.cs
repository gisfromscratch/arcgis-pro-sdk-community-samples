using ArcGIS.Desktop.Framework.Contracts;
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
            var random = new Random();
            var id = random.Next(5).ToString();
            DatasetsPaneViewModel.Open(id);
        }
    }
}
