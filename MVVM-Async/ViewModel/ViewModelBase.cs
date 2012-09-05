using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Dynamic;
using MVVM_Async.Async;
using MVVM_Async.Commands;
using System.Windows;
using System.Windows.Input;
using MVVM_Async.Invokers;
using System.Collections;

namespace MVVM_Async.ViewModel
{
    public abstract class ViewModelBase : DynamicObject, INotifyPropertyChanged
    {
        #region Fields

        CentralDispatch dispatcher;
        DynamicCommandInvoker commandInvoker;

        #endregion

        #region Constructors

        public ViewModelBase()
        {
            InitalSetup();
        }

        private void InitalSetup()
        {
            this.dispatcher = new CentralDispatch();
            this.commandInvoker = new DynamicCommandInvoker(this);
        }

        #endregion

        #region Dynamic Members

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return Enumerable.Empty<string>();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (commandInvoker.SupportedCommands.Contains(binder.Name))
            {
                result = commandInvoker.GetCommand(binder.Name);
                return true;
            }
            return base.TryGetMember(binder, out result);
        }

        #endregion

        #region Dispatch Methods

        protected void Dispatch(Action action)
        {
            dispatcher.Dispatch(action);
        }

        protected void Dispatch<T>(T viewModel, Func<T, Action> function)
        {
            dispatcher.Dispatch(viewModel, function);
        }

        protected void Dispatch<T, F>(T viewModel, Func<T, Action<F>> function, F obj)
        {
            dispatcher.Dispatch(viewModel, function, obj);
        }

        protected void Dispatch<T, F1, F2>(T viewModel,
            Func<T, Action<F1, F2>> function,
            F1 obj, F2 obj2)
        {
            dispatcher.Dispatch(viewModel, function, obj, obj2);
        }

        protected void Dispatch<T, F1, F2, F3>(T viewModel,
            Func<T, Action<F1, F2, F3>> function,
            F1 obj, F2 obj2, F3 obj3)
        {
            dispatcher.Dispatch(viewModel, function, obj, obj2, obj3);
        }

        protected void Dispatch<T>(Action<T> action, T obj)
        {
            dispatcher.Dispatch(action, obj);
        }

        protected void DispatchMain(Action action)
        {
            dispatcher.DispatchMain(action);
        }

        protected void DispatchAsyncWaitAll(List<Action> actions, Action callback)
        {
            dispatcher.DispatchAsyncWaitAll(actions, callback);
        }

        #endregion

        #region Events

        public event ViewModelEventDelegate ViewModelClosingEvent;

        protected void ClosingEvent()
        {
            if (ViewModelClosingEvent != null)
            {
                ViewModelClosingEvent(this, new ViewModelEventArgs());
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnChanged(string property)
        {
            var changeEvent = PropertyChanged;
            if (changeEvent != null)
            {
                changeEvent(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}
