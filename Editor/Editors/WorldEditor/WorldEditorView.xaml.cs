// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using Editor.GameDev;
using Editor.GameProject;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Editor.Editors
{
    /// <summary>
    /// Interaction logic for WorldEditorView.xaml
    /// </summary>
    public partial class WorldEditorView : UserControl
    {
        public WorldEditorView()
        {
            InitializeComponent();
            Loaded += OnWorldEditorViewLoaded;
        }

        private void OnWorldEditorViewLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnWorldEditorViewLoaded;
            Focus();
        }

        private void OnNewScript_Button_Click(object sender, RoutedEventArgs e)
        {
            new NewScriptDialog().ShowDialog();
        }
    }
}
