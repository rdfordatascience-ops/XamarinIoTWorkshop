﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace XamarinIoTWorkshop
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region Constant Fields
        protected const string _beginDataCollectionText = "Begin Data Collection";
        protected const string _endDataCollectionText = "End Data Collection";
        #endregion

        #region Fields
        string _dataCollectedLabelText = string.Empty;
        string _dataCollectionButtonText = _beginDataCollectionText;
        ICommand _dataCollectionButtonCommand;
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<Type> FeatureNotSupportedExceptionThrown;
        #endregion

        #region Properties
        public ICommand DataCollectionButtonCommand => _dataCollectionButtonCommand ??
            (_dataCollectionButtonCommand = new Command(ExecuteDataCollectionButtonCommand));

        public string DataCollectionButtonText
        {
            get => _dataCollectionButtonText;
            set => SetProperty(ref _dataCollectionButtonText, value);
        }

        protected bool IsDataCollectionActive { get; private set; }
        #endregion

        #region Methods
        protected void OnFeatureNotSupportedExceptionThrown(Type xamarinEssentialsType) =>
            FeatureNotSupportedExceptionThrown?.Invoke(this, xamarinEssentialsType);

        protected virtual void StartDataCollection()
        {
            IsDataCollectionActive = true;
        }

        protected virtual void StopDataCollection()
        {
            IsDataCollectionActive = false;
        }

        protected void SetProperty<T>(ref T backingStore, T value, Action onChanged = null, [CallerMemberName] string propertyname = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return;

            backingStore = value;

            onChanged?.Invoke();

            OnPropertyChanged(propertyname);
        }

        void ExecuteDataCollectionButtonCommand()
        {
            if (DataCollectionButtonText.Equals(_beginDataCollectionText))
            {
                StartDataCollection();
                DataCollectionButtonText = _endDataCollectionText;
            }
            else
            {
                StopDataCollection();
                DataCollectionButtonText = _beginDataCollectionText;
            }
        }

        void OnPropertyChanged([CallerMemberName]string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion
    }
}
