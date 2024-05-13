// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Editor.Utilities
{
    /// <summary>
    /// Interaction logic for RenderSurfaceView.xaml
    /// </summary>
    public partial class RenderSurfaceView : UserControl, IDisposable
    {
        private enum Win32Msg
        {
            WM_SIZEING = 0x0214,
            WM_ENTERSIZEMOVE = 0x0231,
            WM_EXITSIZEMOVE = 0x0232,
            WM_SIZE = 0x0005,
        }

        private RenderSurfaceHost _host = null;

        public RenderSurfaceView()
        {
            InitializeComponent();
            Loaded += OnRenderSurfaceViewLoaded;
        }

        private void OnRenderSurfaceViewLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnRenderSurfaceViewLoaded;

            _host = new RenderSurfaceHost(ActualWidth, ActualHeight);
            _host.MessageHook += new HwndSourceHook(HostMsgFiler);
            Content = _host;
        }

        private nint HostMsgFiler(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch ((Win32Msg)msg)
            {
                case Win32Msg.WM_SIZEING: throw new Exception();
                case Win32Msg.WM_ENTERSIZEMOVE: throw new Exception();
                case Win32Msg.WM_EXITSIZEMOVE: throw new Exception();
                case Win32Msg.WM_SIZE:
                    _host.Resize();
                    break;
                default:
                    break;
            }

            return IntPtr.Zero;
        }


        #region IDisposable support
        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _host.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
