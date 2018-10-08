using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UINetworkDiscovery
{
    public class InfoBarController
    {
        private TextBlock _message;
        private Ellipse _icon;
        private bool _visibility;

        public InfoBarController(TextBlock messageContainer, Ellipse icon)
        {
            _message = messageContainer;
            _icon = icon;
            _visibility = false;
        }

        public void ShowMessage(string text, SolidColorBrush color)
        {
            _message.Text = text;
            _icon.Fill = color;
            SetVisibility(true);
        }

        public void SetVisibility(bool visible)
        {
            _visibility = visible;
            if (_visibility)
            {
                _message.Visibility = Visibility.Visible;
                _icon.Visibility = Visibility.Visible;
            }
            else
            {
                _message.Visibility = Visibility.Hidden;
                _icon.Visibility = Visibility.Hidden;
            }
        }

        public void Clear()
        {
            _message.Text = "";
            _icon.Fill = Brushes.Transparent;
            SetVisibility(false);
        }
    }
}
