using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestaurantApp.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace RestaurantApp.ViewModels
{
    public class CompanyInformationsViewModel : BindableBase, IDialogAware
    {
        private string _companyName;

        private string _companyAddress;

        private DelegateCommand _saveConfigCommand;

        public event Action<IDialogResult> RequestClose;

        public string Title { get; set; } = "Company informations";

        public string CompanyName
        {
            get
            {
                return _companyName;
            }

            set
            {
                _companyName = value;
                RaisePropertyChanged();
            }
        }

        public string CompanyAddress
        {
            get
            {
                return _companyAddress;
            }
            set
            {
                _companyAddress = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand SaveConfigCommand
        {
            get
            {
                _saveConfigCommand = new DelegateCommand(SaveConfig);
                return _saveConfigCommand;
            }
        }

        private void LoadConfig()
        {
            if (!File.Exists("config.ini"))
            {
                return;
            }

            string data = string.Empty;

            using (StreamReader streamReader = new StreamReader("config.ini"))
            {
                data = streamReader.ReadToEnd();
            }

            JObject parsedData = JObject.Parse(data);

            CompanyName = parsedData["Company name"].ToString();
            CompanyAddress = parsedData["Company address"].ToString();
        }

        private void SaveConfig()
        {
            if (CompanyName is null || CompanyName == string.Empty)
            {
                MessageBox.Show("Company name field can not be empty!", "Company informations", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (CompanyAddress is null || CompanyAddress == string.Empty)
            {
                MessageBox.Show("Company address field can not be empty!", "Company informations", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Dictionary<string, string> keyValues = new Dictionary<string, string>
            {
                { "Company name", CompanyName },
                { "Company address", CompanyAddress }
            };

            using (StreamWriter streamWriter = new StreamWriter("config.ini"))
            {
                string json = JsonConvert.SerializeObject(keyValues, Newtonsoft.Json.Formatting.Indented);
                streamWriter.Write(json);
            }

            DrawningHelper.CompanyName = CompanyName;
            DrawningHelper.CompanyAddress = CompanyAddress;

            MessageBox.Show("Config file is successfully created!", "Company informations", MessageBoxButton.OK, MessageBoxImage.Information);
            CloseDialog("true");
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

        public void OnDialogOpened(IDialogParameters parameters)
        {
            LoadConfig();
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public void OnDialogClosed()
        {

        }
    }
}
