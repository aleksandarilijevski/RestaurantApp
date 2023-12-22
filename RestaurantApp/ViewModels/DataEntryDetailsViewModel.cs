using EntityFramework.Models;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;

namespace RestaurantApp.ViewModels
{
    public class DataEntryDetailsViewModel : BindableBase, IDialogAware
    {
        private ObservableCollection<ArticleDetails> _articleDetails;

        public event Action<IDialogResult> RequestClose;

        public string Title { get; set; } = "Data entry details";

        public DataEntry DataEntry { get; set; }

        public ObservableCollection<ArticleDetails> ArticleDetails
        {
            get
            {
                return _articleDetails;
            }

            set
            {
                _articleDetails = value;
                RaisePropertyChanged();
            }
        }

        protected virtual void CloseDialog(string parameter)
        {
            ButtonResult result = ButtonResult.None;

            if (parameter?.ToLower() == "true")
            {
                result = ButtonResult.OK;
            }
            else if (parameter?.ToLower() == "false")
            {
                result = ButtonResult.Cancel;
            }

            RaiseRequestClose(new DialogResult(result));
        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            DataEntry = parameters.GetValue<DataEntry>("dataEntry");
            ArticleDetails = new ObservableCollection<ArticleDetails>(DataEntry.ArticleDetails);
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
    }
}
