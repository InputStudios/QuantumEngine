﻿// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.
using Editor.GameProject;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace Editor.Content
{
	/// <summary>
	/// Interaction logic for SelectFolderDialog.xaml
	/// </summary>
	public partial class SelectFolderDialog : Window
	{
		public string SelectedFolder { get; private set; }
		public SelectFolderDialog(string startFolder)
		{
			InitializeComponent();
			
			contentBrowserView.Loaded += (_, _) =>
			{
				// TODO: make sure that all paths always end with a directory separator character, application-wide!
				var startPath = startFolder += Path.DirectorySeparatorChar;
				
				if (startPath.Contains(Project.Current.ContentPath) == true)
				{
					(contentBrowserView.DataContext as ContentBrowser).SelectedFolder = startFolder;
				}
			};
			
			Closing += OnDialogClosing;
		}
		
		private void OnSelectFolder_Button_Click(object sender, RoutedEventArgs e)
		{
			var contentBrowser = contentBrowserView.DataContext as ContentBrowser;
			SelectedFolder = contentBrowser.SelectedFolder;
			DialogResult = true;
			Close();
		}
		
		private void OnDialogClosing(object? sender, CancelEventArgs e)
		{
			contentBrowserView.Dispose();
		}
	}
}
