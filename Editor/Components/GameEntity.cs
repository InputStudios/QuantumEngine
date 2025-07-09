﻿// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using Editor.DLLWrappers;
using Editor.GameProject;
using Editor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace Editor.Components
{
    [DataContract]
    [KnownType(typeof(Transform))]
    [KnownType(typeof(Script))]
    class GameEntity : ViewModelBase
    {
        private int _entityId = ID.INVALID_ID;
        public int EntityId
        {
            get => _entityId;
            set
            {
                if (_entityId != value)
                {
                    _entityId = value;
                    OnPropertyChanged(nameof(EntityId));
                }
            }
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    if (_isActive)
                    {
                        EntityId = EngineAPI.EntityAPI.CreateGameEntity(this);
                        Debug.Assert(ID.IsValid(_entityId));
                    }
                    else if (ID.IsValid(EntityId))
                    {
                        EngineAPI.EntityAPI.RemoveGameEntity(this);
                        EntityId = ID.INVALID_ID;
                    }

                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        private bool _isEnabled = true;
        [DataMember]
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        private string _name;
        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        [DataMember]
        public Scene ParentScene { get; private set; }

        [DataMember(Name = nameof(Components))]
        private readonly ObservableCollection<Component> _components = new();
        public ReadOnlyObservableCollection<Component> Components { get; private set; }
        public Component GetComponent(Type type) => Components.FirstOrDefault(c => c.GetType() == type);
        public T GetComponent<T>() where T : Component => GetComponent(typeof(T)) as T;

        public bool AddComponent(Component component)
        {
            Debug.Assert(component != null);
            if (!Components.Any(x => x.GetType() == component.GetType()))
            {
                IsActive = false;
                _components.Add(component);
                IsActive = true;
                return true;
            }
            Logger.Log(MessageType.Warning, $"Entity {Name} already has a {component.GetType().Name} component");
            return false;
        }

        public void RemoveComponent(Component component)
        {
            Debug.Assert(component != null);
            if (component is Transform) return; // Transform component can't be removed

            if (_components.Contains(component))
            {
                IsActive = false;
                _components.Remove(component);
                IsActive = true;
            }
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            if (_components != null)
            {
                Components = new ReadOnlyObservableCollection<Component>(_components);
                OnPropertyChanged(nameof(Components));
            }
        }

        public GameEntity(Scene parent)
        {
            Debug.Assert(parent != null);
            ParentScene = parent;
            _components.Add(new Transform(this));
            OnDeserialized(new StreamingContext());
        }
    }

    abstract class MSEntity : ViewModelBase
    {
        private bool _enableUpdates = true;

        private bool? _isEnabled;
        public bool? IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private readonly ObservableCollection<IMSComponent> _components = new();
        public ReadOnlyCollection<IMSComponent> Components { get; }

        public T GetMSComponent<T>() where T : IMSComponent
        {
            return (T)Components.FirstOrDefault(x => x.GetType() == typeof(T));
        }

        public List<GameEntity> SelectedEntities { get; }

        private void MakeComponentList()
        {
            _components.Clear();
            var firstEntity = SelectedEntities.FirstOrDefault();
            if (firstEntity == null) return;

            foreach (var component in firstEntity.Components)
            {
                var type = component.GetType();
                if (!SelectedEntities.Skip(1).Any(entity => entity.GetComponent(type) == null))
                {
                    Debug.Assert(Components.FirstOrDefault(x => x.GetType() == type) == null);
                    _components.Add(component.GetMSComponent(this));
                }
            }
        }

        public static float? GetMixedValues<T>(List<T> objects, Func<T, float> getProperty)
        {
            var value = getProperty(objects.First());
            return objects.Skip(1).Any(x => getProperty(x) != value) ? null : value;
        }

        public static bool? GetMixedValues<T>(List<T> objects, Func<T, bool> getProperty)
        {
            var value = getProperty(objects.First());
            return objects.Skip(1).Any(x => getProperty(x) != value) ? null : value;
        }

        public static string GetMixedValues<T>(List<T> objects, Func<T, string> getProperty)
        {
            var value = getProperty(objects.First());
            return objects.Skip(1).Any(x => getProperty(x) != value) ? null : value;
        }

        protected virtual bool UpdateGameEntities(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(IsEnabled): SelectedEntities.ForEach(x => x.IsEnabled = IsEnabled.Value); return true;
                case nameof(Name): SelectedEntities.ForEach(x => x.Name = Name); return true;
            }
            return false;
        }

        protected virtual void UpdateMSGameEntity()
        {
            IsEnabled = GetMixedValues(SelectedEntities, new Func<GameEntity, bool>(x => x.IsEnabled));
            Name = GetMixedValues(SelectedEntities, new Func<GameEntity, string>(x => x.Name));
        }

        public void Refresh()
        {
            _enableUpdates = false;
            UpdateMSGameEntity();
            MakeComponentList();
            _enableUpdates = true;
        }

        public MSEntity(List<GameEntity> entities)
        {
            Debug.Assert(entities?.Any() == true);
            Components = new ReadOnlyObservableCollection<IMSComponent>(_components);
            SelectedEntities = entities;
            PropertyChanged += (s, e) => { if (_enableUpdates) UpdateGameEntities(e.PropertyName); };
        }
    }

    class MSGameEntity : MSEntity
    {
        public MSGameEntity(List<GameEntity> entities) : base(entities)
        {
            Refresh();
        }
    }
}
