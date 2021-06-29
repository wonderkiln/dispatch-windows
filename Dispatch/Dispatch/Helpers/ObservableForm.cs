using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Dispatch.Helpers
{
    public abstract class ObservableForm : Observable, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, string> errors = new Dictionary<string, string>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors
        {
            get
            {
                return errors.Keys.Count > 0;
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyName != null && errors.ContainsKey(propertyName))
                return new List<string>() { errors[propertyName] };

            return null;
        }

        private void SetError(string propertyName, string errorMessage)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                if (errors.ContainsKey(propertyName))
                {
                    errors.Remove(propertyName);
                }
            }
            else if (errors.ContainsKey(propertyName))
            {
                errors[propertyName] = errorMessage;
            }
            else
            {
                errors.Add(propertyName, errorMessage);
            }
        }

        private void NotifyError(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        internal abstract string GetError(string propertyName);

        internal void ValidateProperty(string propertyName)
        {
            SetError(propertyName, GetError(propertyName));
            NotifyError(propertyName);
        }

        public bool Validate()
        {
            foreach (var property in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                ValidateProperty(property.Name);
            }

            return !HasErrors;
        }

        internal override void Notify([CallerMemberName] string propertyName = null)
        {
            base.Notify(propertyName);
            ValidateProperty(propertyName);
        }
    }
}
