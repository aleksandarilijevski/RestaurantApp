using EntityFramework.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Services.Interface;
using System;
using System.Reflection;

namespace RestaurantApp.ViewModels
{
    public class EditArticleViewModel : BindableBase, IDialogAware
    {
        private IDatabaseService _databaseService;
        private DelegateCommand _editArticleCommand;
        private Article _deepCopyArticle;

        public event Action<IDialogResult> RequestClose;

        public EditArticleViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public Article Article { get; set; }

        public string Title { get; set; } = "Edit article";

        public DelegateCommand EditArticleCommand
        {
            get
            {
                _editArticleCommand = new DelegateCommand(EditArticle);
                return _editArticleCommand;
            }
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

        public Article DeepCopyArticle
        {
            get
            {
                return _deepCopyArticle;
            }

            set
            {
                _deepCopyArticle = value;
                RaisePropertyChanged();
            }
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
            Article = parameters.GetValue<Article>("article");
            DeepCopyArticle = DeepCopy(Article);
        }

        public T DeepCopy<T>(T source)
        {
            Type targetType = source.GetType();
            object copiedObject = Activator.CreateInstance(targetType);

            foreach (PropertyInfo property in targetType.GetProperties())
            {
                if (property.CanWrite)
                {
                    object value = property.GetValue(source);
                    property.SetValue(copiedObject, value);
                }
            }

            return (T)copiedObject;
        }
        private async void EditArticle()
        {
            //Article article = await _databaseService.GetArticleByID(DeepCopyArticle.ID);

            //article.Name = DeepCopyArticle.Name;
            //article.Barcode = DeepCopyArticle.Barcode;
            //article.Price = DeepCopyArticle.Barcode;

            //await _databaseService.EditArticle(DeepCopyArticle);
            CloseDialog("true");
        }
    }
}
