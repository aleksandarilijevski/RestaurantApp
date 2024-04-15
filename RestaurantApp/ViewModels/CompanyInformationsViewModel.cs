using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RestauranApp.Utilities.Constants;
using RestaurantApp.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using DialogResult = Prism.Services.Dialogs.DialogResult;
using DialogResultForm = System.Windows.Forms.DialogResult;
using MessageBox = System.Windows.MessageBox;

namespace RestaurantApp.ViewModels
{
    public class CompanyInformationsViewModel : BindableBase, IDialogAware
    {
        private string _companyName;

        private string _companyAddress;

        private string _billOutputPath;

        private int _pdv;

        private DelegateCommand _saveConfigCommand;

        private DelegateCommand _selectBillOutputPathCommand;

        public event Action<IDialogResult> RequestClose;

        public string Title { get; set; } = ViewConstants.CompanyInformationsTitle;

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

        public string BillOutputPath
        {
            get
            {
                return _billOutputPath;
            }

            set
            {
                _billOutputPath = value;
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

        public int PDV
        {
            get
            {
                return _pdv;
            }

            set
            {
                _pdv = value;
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

        public DelegateCommand SelectBillOutputPathCommand
        {
            get
            {
                _selectBillOutputPathCommand = new DelegateCommand(SelectBillOutputPath);
                return _selectBillOutputPathCommand;
            }
        }

        private void LoadConfig()
        {
            PDV = 20;

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
            BillOutputPath = parsedData["Bill output path"].ToString();
            PDV = int.Parse(parsedData["PDV"].ToString());
        }

        private void SaveConfig()
        {
            if (CompanyName is null || CompanyName == string.Empty)
            {
                MessageBox.Show(MessageBoxConstants.CompanyNameFieldCanNotBeEmpty, MessageBoxConstants.CompanyInformationsTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (CompanyAddress is null || CompanyAddress == string.Empty)
            {
                MessageBox.Show(MessageBoxConstants.CompanyAddressFieldCanNotBeEmpty, MessageBoxConstants.CompanyInformationsTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (BillOutputPath is null || BillOutputPath == string.Empty)
            {
                MessageBox.Show(MessageBoxConstants.BillOutputPathFieldCanNotBeEmpty, MessageBoxConstants.CompanyInformationsTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (PDV == 0 || PDV < 0)
            {
                MessageBox.Show(MessageBoxConstants.PDVCannotBeZeroOrLess, MessageBoxConstants.CompanyInformationsTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!BillOutputPath.EndsWith("\\"))
            {
                BillOutputPath += "\\";
            }

            Dictionary<string, string> keyValues = new Dictionary<string, string>
            {
                { "Company name", CompanyName },
                { "Company address", CompanyAddress },
                { "Bill output path", BillOutputPath },
                { "PDV", PDV.ToString() }
            };

            using (StreamWriter streamWriter = new StreamWriter("config.ini"))
            {
                string json = JsonConvert.SerializeObject(keyValues, Newtonsoft.Json.Formatting.Indented);
                streamWriter.Write(json);
            }

            DrawningHelper.CompanyName = CompanyName;
            DrawningHelper.CompanyAddress = CompanyAddress;
            DrawningHelper.BillOutputPath = BillOutputPath;
            DrawningHelper.PDV = PDV;

            MessageBox.Show(MessageBoxConstants.ConfigFileIsSuccessfullyCreated, MessageBoxConstants.CompanyInformationsTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            CloseDialog("true");
        }

        private void SelectBillOutputPath()
        {
            using (FolderBrowserDialog browserDialog = new FolderBrowserDialog())
            {
                DialogResultForm dialogResult = browserDialog.ShowDialog();

                if (dialogResult == DialogResultForm.OK)
                {
                    BillOutputPath = browserDialog.SelectedPath;
                }
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
