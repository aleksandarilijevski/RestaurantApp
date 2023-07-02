using Prism.Services.Dialogs;
using System;

namespace RestaurantApp.ViewModels.Interfaces
{
    public interface IDialogAware
    {
        bool CanCloseDialog();

        void OnDialogClosed();

        void OnDialogOpened(IDialogParameters parameters);

        string Title { get; set; }

        event Action<IDialogResult> RequestClose;
    }
}
