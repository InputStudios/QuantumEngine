﻿// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Editor.Utilities.Controls
{
    [TemplatePart(Name = "PART_textBlock", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_textBox", Type = typeof(TextBox))]
    class NumberBox : Control
    {
        private double _originalValue;
        private double _mouseXStart;
        private double _multiplier;
        private bool _captured;
        private bool _valueChanged;
		
        public event RoutedEventHandler ValueChanged
        {
            add => AddHandler(ValueChangedEvent, value);
            remove => RemoveHandler(ValueChangedEvent, value);
        }
		
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(nameof(ValueChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumberBox));
		
        public double Multiplier
        {
            get => (double)GetValue(MultiplierProperty);
            set => SetValue(MultiplierProperty, value);
        }
		
        public static readonly DependencyProperty MultiplierProperty = DependencyProperty.Register(nameof(Multiplier), typeof(double), typeof(NumberBox), new FrameworkPropertyMetadata(1.0));
		
        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
		
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(NumberBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    new PropertyChangedCallback(OnValueChanged)));
		
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NumberBox).RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
        }
		
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
		
            if (GetTemplateChild("PART_textBlock") is TextBlock textBlock)
            {
                textBlock.MouseLeftButtonDown += OnTextBlock_Mouse_LBD;
                textBlock.MouseLeftButtonUp += OnTextBlock_Mouse_LBU;
                textBlock.MouseMove += OnTextBock_Mouse_Move;
            }
        }
		
        private void OnTextBlock_Mouse_LBD(object sender, MouseButtonEventArgs e)
        {
            double.TryParse(Value, out _originalValue);
			
            Mouse.Capture(sender as UIElement);
            _captured = true;
            _valueChanged = false;
            e.Handled = true;
            _mouseXStart = e.GetPosition(this).X;
            Focus();
        }
		
        private void OnTextBlock_Mouse_LBU(object sender, MouseButtonEventArgs e)
        {
            if (_captured)
            {
                Mouse.Capture(null);
                _captured = false;
                e.Handled = true;
                if (!_valueChanged && GetTemplateChild("PART_textBox") is TextBox textBox)
                {
                    textBox.Visibility = Visibility.Visible;
                    textBox.Focus();
                }
            }
        }
		
        private void OnTextBock_Mouse_Move(object sender, MouseEventArgs e)
        {
            if (_captured)
            {
                var mouseX = e.GetPosition(this).X;
                var d = mouseX - _mouseXStart;
                if (Math.Abs(d) > SystemParameters.MinimumHorizontalDragDistance)
                {
                    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) _multiplier = 0.001;
                    else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) _multiplier = 0.1;
                    else _multiplier = 0.1;
                    var newValue = _originalValue + (d + _multiplier + Multiplier);
                    Value = newValue.ToString("G5");
                    _valueChanged = true;
                }
            }
        }
		
        static NumberBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberBox), new FrameworkPropertyMetadata(typeof(NumberBox)));
        }
    }
}
