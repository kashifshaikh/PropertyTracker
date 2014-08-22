﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cirrious.MvvmCross.ViewModels;
using PropertyTracker.Core.PresentationHints;

namespace PropertyTracker.Core.ViewModels
{
    public class UserListViewModel : BaseViewModel
    {
        public override void Start()
        {
            
        }
        public IMvxCommand LogoutCommand
        {
            get { return new MvxCommand(() => ChangePresentation(new LogoutPresentationHint())); }
        }
    }
}
