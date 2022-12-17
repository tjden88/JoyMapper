using System;
using JoyMapper.Interfaces;
using JoyMapper.Models.JoyBindings.Base;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.UserControls
{
    public class JoyBindingViewModel : ViewModel, IEditModel<JoyBindingBase>
    {
        public JoyBindingBase GetModel()
        {
            throw new NotImplementedException();
        }

        public void SetModel(JoyBindingBase model)
        {
            throw new NotImplementedException();
        }
    }
}
