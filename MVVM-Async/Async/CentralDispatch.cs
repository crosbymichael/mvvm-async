using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace MVVM_Async.Async
{
    /// <summary>
    /// Provides a clean lambda based interface for
    /// multithreading your viewmodels
    /// </summary>
    class CentralDispatch
    {
        SynchronizationContext mainThreadContext;
        public CentralDispatch()
        {
            mainThreadContext = SynchronizationContext.Current;

            if (mainThreadContext == null)
            {
                mainThreadContext = new SynchronizationContext();
            }
        }

        #region Dispatch Methods

        public void Dispatch(Action action)
        {
            Task.Factory.StartNew(action);
        }

        public void Dispatch<T>(T viewModel, Func<T, Action> function)
        {
            var action = function(viewModel);
            Task.Factory.StartNew(action);
        }

        public void Dispatch<T, F>(T viewModel, Func<T, Action<F>> function, F obj)
        {
            var action = function(viewModel);
            Task.Factory.StartNew(() => {
                action(obj); });
        }

        public void Dispatch<T, F1, F2>(T viewModel, 
            Func<T, Action<F1, F2>> function,
            F1 obj, F2 obj2)
        {
            var action = function(viewModel);
            Task.Factory.StartNew(() => {
                action(obj, obj2); });
        }

        public void Dispatch<T, F1, F2, F3>(T viewModel,
            Func<T, Action<F1, F2, F3>> function,
            F1 obj, F2 obj2, F3 obj3)
        {
            var action = function(viewModel);
            Task.Factory.StartNew(() => {
                action(obj, obj2, obj3); });
        }

        public void Dispatch<T>(Action<T> action, T obj)
        {
            Task.Factory.StartNew(() => {
                action(obj); });
        }

        public void DispatchMain(Action action)
        {
            mainThreadContext.Post((o) => {
                action(); }, null);
        }

        public void DispatchAsyncWaitAll(IEnumerable<Action> actions, Action callback)
        {
            var tasks = actions.Select(a => new Task(a)).ToArray();
            var bg = new Task(() => {
                Task.WaitAll(tasks);
                DispatchMain(callback);
            });
            bg.Start();
        }

        #endregion
    }
}
