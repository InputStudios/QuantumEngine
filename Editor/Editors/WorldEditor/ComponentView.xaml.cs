// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Editor.Editors
{
    /// <summary>
    /// Interaction logic for ComponentView.xaml
    /// </summary>
    [ContentProperty("ComponentContent")]
    public partial class ComponentView : UserControl
    {
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // using a DependencyProperty as the backing store for Header 
        // enabling animation, styling, binding, and more
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(string), typeof(ComponentView));

        public FrameworkElement ComponentContent
        {
            get { return (FrameworkElement)GetValue(ComponentContentProperty); }
            set { SetValue(ComponentContentProperty, value); }
        }

        // using a DependencyProperty as the backing store for ComponentContent 
        // enabling animation, styling, binding, and more
        public static readonly DependencyProperty ComponentContentProperty =
            DependencyProperty.Register(nameof(ComponentContent), typeof(FrameworkElement), typeof(ComponentView));

        public ComponentView()
        {
            InitializeComponent();
        }
    }
}
