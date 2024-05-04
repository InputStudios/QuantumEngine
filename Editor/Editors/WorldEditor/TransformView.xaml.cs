using Editor.Components;
using Editor.GameProject;
using Editor.Utilities;
using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace Editor.Editors
{
    public partial class TransformView : UserControl
    {
        private Action _undoAction = null;
        private bool _propertyChanged = false;

        public TransformView()
        {
            InitializeComponent();
            Loaded += OnTransformViewLoaded;
        }

        private void OnTransformViewLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnTransformViewLoaded;
            (DataContext as MSTransform).PropertyChanged += (s, e) => _propertyChanged = true;
        }

        private Action GetAction(Func<Transform, (Transform transform, Vector3)> selector, Action<(Transform transform, Vector3)> forEachAction)
        {
            if (!(DataContext is MSTransform vm))
            {
                _undoAction = null;
                _propertyChanged = false;
                return null;
            }

            var selection = vm.SelectedComponents.Select(selector).ToList();
            return new Action(() =>
            {
                selection.ForEach(forEachAction);
                (GameEntityView.Instance.DataContext as MSEntity)?.GetMSComponent<MSTransform>().Refresh();
            });
        }

        private Action GetPositionAction() => GetAction((x) => (x, x.Position), (x) => x.transform.Position = x.Item2);
        private Action GetRotationAction() => GetAction((x) => (x, x.Rotation), (x) => x.transform.Rotation = x.Item2);
        private Action GetScaleAction() => GetAction((x) => (x, x.Scale), (x) => x.transform.Scale = x.Item2);

        private void RecordAction(Action redoAction, string name)
        {
            if (_propertyChanged)
            {
                Debug.Assert(_undoAction != null);
                _propertyChanged = false;
                Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, name));
            }
        }

        private void OnPosition_VectorBox_Mouse_LBD(object sender, MouseButtonEventArgs e)
        {
            _propertyChanged = false;
            _undoAction = GetPositionAction();
        }


        private void OnPosition_VectorBox_Mouse_LBU(object sender, MouseButtonEventArgs e)
        {
            RecordAction(GetPositionAction(), "Position changed");
        }

        private void OnPosition_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChanged && _undoAction != null) OnPosition_VectorBox_Mouse_LBU(sender, null);
           
        }

        private void OnRotation_VectorBox_Mouse_LBD(object sender, MouseButtonEventArgs e)
        {
            _propertyChanged = true;
            _undoAction = GetRotationAction();
        }


        private void OnRotation_VectorBox_Mouse_LBU(object sender, MouseButtonEventArgs e)
        {
            RecordAction(GetRotationAction(), "Rotation changed");
        }

        private void OnRotation_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChanged && _undoAction != null) OnRotation_VectorBox_Mouse_LBU(sender, null);
            
        }

        private void OnScale_VectorBox_Mouse_LBD(object sender, MouseButtonEventArgs e)
        {
            _propertyChanged = true;
            _undoAction = GetScaleAction();
        }

        private void OnScale_VectorBox_Mouse_LBU(object sender, MouseButtonEventArgs e)
        {
            RecordAction(GetScaleAction(), "Scale changed");
        }

        private void OnScale_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChanged && _undoAction != null) OnScale_VectorBox_Mouse_LBU(sender, null);
            
        }
    }
}
