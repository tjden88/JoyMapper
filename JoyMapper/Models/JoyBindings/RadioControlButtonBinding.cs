using System;
using JoyMapper.Models.JoyBindings.Base;

namespace JoyMapper.Models.JoyBindings
{
    public class RadioControlButtonBinding : ButtonJoyBinding
    {
        private readonly ButtonJoyBinding _Origin;
        public RadioControlTypes RadioControlType { get; set; }

        public enum RadioControlTypes
        {
            PlayStop,
            Next,
            Previous,
        }

        public RadioControlButtonBinding(ButtonJoyBinding Origin)
        {
            _Origin = Origin;
        }


        public override bool Equals(JoyBindingBase other) => other is RadioControlButtonBinding rc && rc.RadioControlType == RadioControlType;
    }
}
