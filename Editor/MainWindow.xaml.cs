// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using Editor.GameProject;
using System;
using System.ComponentModel;
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

        private void OnMainWindowClosing(object? sender, CancelEventArgs e)
        {
            Closing -= OnMainWindowClosing;
            GetEnginePath();
            Project.Current?.Unload();
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnMainWindowLoaded;
            GetEnginePath();
            OpenProjectBrowserDialog();
        }

        private void GetEnginePath()
        {
            var quantumPath = Environment.GetEnvironmentVariable("QUANTUM_ENGINE", EnvironmentVariableTarget.User);
            if (quantumPath == null || !Directory.Exists(Path.Combine(quantumPath, @"Engine\EngineAPI")))
            {
                var dlg = new EnginePathDialog();
                if (dlg.ShowDialog() == true)
                {
                    QuantumPath = dlg.QuantumPath;
                    Environment.SetEnvironmentVariable("QUANTUM_ENGINE", QuantumPath.ToUpper(), EnvironmentVariableTarget.User);
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
            else 
            {
                QuantumPath = quantumPath;
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
                DataContext = projectBrowser.DataContext;
            }
        }
    }
}
