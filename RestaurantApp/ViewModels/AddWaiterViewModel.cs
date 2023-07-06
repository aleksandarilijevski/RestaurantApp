using EntityFramework.Models;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace RestaurantApp.ViewModels
{
    public class AddWaiterViewModel : BindableBase, IDialogAware
    {
        private string _title = "Add waiter";

        public event Action<IDialogResult> RequestClose;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        protected virtual void CloseDialog(string parameter)
        {
            ButtonResult result = ButtonResult.None;

            if (parameter?.ToLower() == "true")
                result = ButtonResult.OK;
            else if (parameter?.ToLower() == "false")
                result = ButtonResult.Cancel;

            RaiseRequestClose(new DialogResult(result));
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {

        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
