using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVM_Async.ViewModel
{
    public delegate void ViewModelEventDelegate(object sender, ViewModelEventArgs args);

    public class ViewModelEventArgs : EventArgs
    {
        public ViewModelEventArgs()
        {
         
        }
    }
}
