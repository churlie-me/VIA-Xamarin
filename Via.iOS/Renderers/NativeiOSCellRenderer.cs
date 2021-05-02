using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Via.Controls;
using Via.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NativeCell), typeof(NativeiOSCellRenderer))]
namespace Via.iOS.Renderers
{
    public class NativeiOSCellRenderer : ViewCellRenderer
    {
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);
            var view = item as NativeCell;
            cell.SelectedBackgroundView = new UIView
            {
                BackgroundColor = view.BackgroundColor.ToUIColor(),
            };

            return cell;
        }
    }
}