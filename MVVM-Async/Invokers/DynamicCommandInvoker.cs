using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVM_Async.Commands;
using System.Windows.Input;
using System.Reflection;
using MVVM_Async.ViewModel;

namespace MVVM_Async.Invokers
{
    class DynamicCommandInvoker
    {
        object viewModelContext;
        Dictionary<string, ICommand> dynamicCommands;
        List<string> publicMethods;
        
        public DynamicCommandInvoker(object viewModelContext)
        {
            this.viewModelContext = viewModelContext;
            dynamicCommands = new Dictionary<string, ICommand>();
            publicMethods = new List<string>();
            PopulatePublicMethods();
        }

        #region Register Command

        public IEnumerable<string> SupportedCommands
        {
            get { return publicMethods.AsEnumerable(); }
        }

        public ICommand GetCommand(string name)
        { 
            ICommand command = null;
            if (dynamicCommands.TryGetValue(name, out command))
            {
                return command;    
            }
            var method = GetMethodInfoFor(name);
            
            if (method == null)
            {
                throw new NotImplementedException(string.Format(
                    "There is no delegate for {0}", name));
            }
            
            command = RegisterDelegateCommand(CreateActionFor(name, method));
            dynamicCommands.Add(name, command);
            return command;
        }

        MethodInfo GetMethodInfoFor(string name)
        {
            return viewModelContext
                .GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(m => m.Name == name);
        }

        Action CreateActionFor(string name, MethodInfo method)
        {
            return (Action)Delegate.CreateDelegate(
                typeof(Action),
                viewModelContext,
                method.Name);
        }

        void PopulatePublicMethods()
        {
            publicMethods = viewModelContext
                .GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Select(m => m.Name)
                .ToList();
        }

        static bool DefaultEnabled()
        {
            return true;
        }

        #endregion

        #region Commands and Delegates

        ICommand RegisterDelegateCommand(Action method)
        {
            return new DelegateCommand(method, DefaultEnabled);
        }

        #endregion
    }
}
