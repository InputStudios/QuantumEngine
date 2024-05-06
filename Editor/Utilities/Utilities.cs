// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Editor.Utilities
{
    public class ID
    {
        public static int INVALID_ID => -1;
        public static bool IsValid(int id) => id != INVALID_ID;
    }

    public static class MathUtils
    {
        public static float Epsilon => 1e-10f;

        public static bool IsEquals(this float value, float other)
        {
            return Math.Abs(value - other) < Epsilon;
        }

        public static bool IsEquals(this float? value, float? other)
        {
            if (!value.HasValue || !other.HasValue) return false;
            return Math.Abs(value.Value - other.Value) < Epsilon;
        }

        public static bool IsEquals(this double value, double other)
        {
            return Math.Abs(value - other) < Epsilon;
        }

        public static bool IsEquals(this double? value, double? other)
        {
            if (!value.HasValue || !other.HasValue) return false;
            return Math.Abs(value.Value - other.Value) < Epsilon;
        }

        public static long AlignSizeUp(long size, long alignment)
        {
            Debug.Assert(alignment > 0, "Alignment must be non-zero");
            long mask = alignment -1;
            Debug.Assert((alignment & mask) == 0, "Alignment must be a power of 2");
            return ((size + mask) & ~mask);
        }

        public static long AlignSizeDown(long size, long alignment)
        {
            Debug.Assert(alignment > 0, "Alignment must be non-zero");
            long mask = alignment - 1;
            Debug.Assert((alignment & mask) == 0, "Alignment must be a power of 2");
            return ((size + mask) & ~mask);
        }

        public static bool IsPow2(int x)
        {
            return (x != 0) && (x & (x - 1)) == 0;
        }
    }

    class DelayEventTimerArgs : EventArgs
    {
        public bool RepeatEvent { get; set; }
        public IEnumerable<object> Data { get; set; }

        public DelayEventTimerArgs(IEnumerable<object> data)
        {
            Data = data;
        }
    }

    class DelayEventTimer
    {
        private readonly DispatcherTimer _timer;
        private readonly TimeSpan _delay;
        private DateTime _lastEventTime = DateTime.Now;
        private readonly List<object> _data = new();

        public event EventHandler<DelayEventTimerArgs> Triggered;

        public void Trigger(object data = null)
        {
            if (data != null) _data.Add(data);
            _lastEventTime = DateTime.Now;
            _timer.IsEnabled = true;
        }

        public void Disable()
        {
            _timer.IsEnabled = false;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if ((DateTime.Now - _lastEventTime) < _delay) return;
            var eventArgs = new DelayEventTimerArgs(_data);
            Triggered?.Invoke(this, eventArgs);
            if (!eventArgs.RepeatEvent) _data.Clear();
            _timer.IsEnabled = eventArgs.RepeatEvent;
        }

        public DelayEventTimer(TimeSpan delay, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            _delay = delay;
            _timer = new DispatcherTimer(priority)
            {
                Interval = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 0.5)
            };
            _timer.Tick += OnTimerTick;
        }
    }
}
