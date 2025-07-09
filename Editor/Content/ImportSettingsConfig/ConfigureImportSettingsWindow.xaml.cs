﻿// Copyright (c) Andrey Trepalin. 
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Editor.Content
{
	/// <summary>
	/// Interaction logic for ConfigureImportSettingsWindow.xaml
	/// </summary>
	public partial class ConfigureImportSettingsWindow : Window
	{
		public ConfigureImportSettingsWindow()
		{
			InitializeComponent();
			
			Loaded += (_, _) =>
			{
				var vm = DataContext as ConfigureImportSettings;
				
				tabControl.SelectedIndex =
					vm.GeometryImportSettingsConfigurator.GeometryProxies.Any() ? 0 :
					vm.TextureImportSettingsConfigurator.TextureProxies.Any() ? 1 :
					vm.AudioImportSettingsConfigurator.AudioProxies.Any() ? 2 : 0;
			};
		}
		
		private void OnTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ImportingItemCollection.SetItemFilter((AssetType)(tabControl.SelectedItem as TabItem)?.Tag);
		}
		
		internal static void AddDroppedFiles(ConfigureImportSettings? dataContext, ListBox? listBox, DragEventArgs e)
		{
			var files = (string[])e.Data.GetData(DataFormats.FileDrop);
			if (files?.Length > 0)
			{
				var destinationFolder = listBox.HasItems ? (listBox.Items[^1] as AssetProxy).DestinationFolder: dataContext.LastDestinationFolder; 
			}
		}
	}
}
