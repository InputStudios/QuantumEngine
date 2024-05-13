// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Components
{
    [DataContract]
    class Script : Component
    {
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

        public override IMSComponent GetMSComponent(MSEntity msEntity) => new MSScript(msEntity);

        public override void WriteToBinary(BinaryWriter bw)
        {
            var nameBytes = Encoding.UTF8.GetBytes(Name);
            bw.Write(nameBytes.Length);
            bw.Write(nameBytes);
        }

        public Script(GameEntity owner) : base(owner) { }
    }

    sealed class MSScript : MSComponent<Script>
    {
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

        protected override bool UpdateComponents(string propertyName)
        {
            if (propertyName == nameof(Name))
            {
                SelectedComponents.ForEach(c => c.Name = _name);
                return true;
            }
            return false;
        }

        protected override bool UpdateMSComponent()
        {
            Name = MSEntity.GetMixedValues(SelectedComponents, new Func<Script, string>(x => x.Name));
            return true;
        }

        public MSScript(MSEntity msEntity) : base(msEntity) { Refresh(); }
    }
}
