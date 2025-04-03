// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using Editor.Content;
using Editor.GameProject;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string QuantumPath { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWindowLoaded;
            Closing += OnMainWindowClosing;
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnMainWindowLoaded;
            GetEnginePath();
            OpenProjectBrowserDialog();
        }

        private void GetEnginePath()
        {
            var enginePath = Environment.GetEnvironmentVariable("QUANTUM_ENGINE", EnvironmentVariableTarget.User);
            if (enginePath == null || !Directory.Exists(Path.Combine(enginePath, @"Engine\EngineAPI\")))
            {
                var dlg = new EnginePathDialog();
                if (dlg.ShowDialog() == true)
                {
                    QuantumPath = dlg.QuantumPath;
                    Environment.SetEnvironmentVariable("QUANTUM_ENGINE", QuantumPath.ToUpper(), EnvironmentVariableTarget.User);
                }
                else Application.Current.Shutdown();
            }
            else
            {
                QuantumPath = enginePath;
            }
        }

        private void OnMainWindowClosing(object? sender, CancelEventArgs e)
        {
            if (DataContext == null)
            {
                e.Cancel = true;
                Application.Current.MainWindow.Hide();
                OpenProjectBrowserDialog();
                if (DataContext != null)
                {
                    Application.Current.MainWindow.Show();
                }
            }
            else
            {
                Closing -= OnMainWindowClosing;
                Project.Current?.Unload();
                DataContext = null;
            }
        }

        private void OpenProjectBrowserDialog()
        {
            var projectBrowser = new ProjectBrowserDialog();
            if (projectBrowser.ShowDialog() == false || projectBrowser.DataContext == null)
            {
                Application.Current.Shutdown();
            }
            else
            {
                Project.Current?.Unload();
                var project = projectBrowser.DataContext as Project;
                Debug.Assert(project != null);
                ContentWatcher.Reset(project.ContentPath, project.Path);
                DataContext = project;
            }
        }
    }
}
