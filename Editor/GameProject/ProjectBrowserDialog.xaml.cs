// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Editor.GameProject
{
    /// <summary>
    /// Interaction logic for ProjectBrowserDialog.xaml
    /// </summary>
    public partial class ProjectBrowserDialog : Window
    {
        private readonly CubicEase _easing = new() { EasingMode = EasingMode.EaseInOut };

        public static bool GoToNewProjectTab { get; set; }

        public ProjectBrowserDialog()
        {
            InitializeComponent();
            Loaded += OnProjectBrowserDialogLoaded;
        }

        private void OnProjectBrowserDialogLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnProjectBrowserDialogLoaded;
            if (!OpenProject.Projects.Any() || GoToNewProjectTab)
            {
                if (!GoToNewProjectTab)
                {
                    openProjectButton.IsEnabled = false;
                    openProjectView.Visibility = Visibility.Hidden;
                }

                OnToggleButton_Click(newProjectButton, new RoutedEventArgs());
            }

            GoToNewProjectTab = false;
        }

        private void AnimateToCreateProject()
        {
            var hightlightAnimation = new DoubleAnimation(240, 420, new Duration(TimeSpan.FromSeconds(0.2)));
            hightlightAnimation.EasingFunction = _easing;
            hightlightAnimation.Completed += (s, e) =>
            {
                var animation = new ThicknessAnimation(new Thickness(0), new Thickness(-1600, 0, 0, 0), new Duration(TimeSpan.FromSeconds(0.5)));
                animation.EasingFunction = _easing;
                browserContent.BeginAnimation(MarginProperty, animation);
            };
            highlightRect.BeginAnimation(Canvas.LeftProperty, hightlightAnimation);
        }

        private void AnimateToOpenProject()
        {
            var hightlightAnimation = new DoubleAnimation(420, 240, new Duration(TimeSpan.FromSeconds(0.2)));
            hightlightAnimation.EasingFunction = _easing;
            hightlightAnimation.Completed += (s, e) =>
            {
                var animation = new ThicknessAnimation(new Thickness(-1600, 0, 0, 0), new Thickness(0), new Duration(TimeSpan.FromSeconds(0.5)));
                animation.EasingFunction = _easing;
                browserContent.BeginAnimation(MarginProperty, animation);
            };
            highlightRect.BeginAnimation(Canvas.LeftProperty, hightlightAnimation);
        }

        private void OnToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender == openProjectButton)
            {
                if (newProjectButton.IsChecked == true)
                {
                    newProjectButton.IsChecked = false;
                    AnimateToOpenProject();
                    openProjectView.IsEnabled = true;
                    newProjectView.IsEnabled = false;
                }
                openProjectButton.IsChecked = true;
            }
            else
            {
                if (openProjectButton.IsChecked == true)
                {
                    openProjectButton.IsChecked = false;
                    AnimateToCreateProject();
                    openProjectView.IsEnabled = false;
                    newProjectView.IsEnabled = true;
                }
                newProjectButton.IsChecked = true;
            }
        }
    }
}
