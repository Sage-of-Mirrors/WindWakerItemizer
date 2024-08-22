using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WindWakerItemizer
{
    internal class ViewModel : INotifyPropertyChanged
    {
        #region Fields
        private Model mModel;
        private int mSelectedIndex;
        private ItemDropConfig? mSelectedConfig;
        #endregion

        #region Properties
        public ObservableCollection<string> ActorNames
        {
            get => mModel.GetActorNames();
        }

        public int SelectedIndex
        {
            get => mSelectedIndex;
            set
            {
                mSelectedIndex = value;
                OnPropertyChanged();

                SelectedConfig = mModel.GetConfigAtIndex(mSelectedIndex);
            }
        }

        public ItemDropConfig? SelectedConfig
        {
            get => mSelectedConfig;
            set
            {
                mSelectedConfig = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public ViewModel()
        {
            mModel = new Model();

            ExitCommand = new RelayCommand(OnExitCommandExecuted, (X) => true);
            AddConfigCommand = new RelayCommand(OnAddCommandExecuted, CanAddCommandExecute);
        }

        #region Relay commands
        public RelayCommand ExitCommand { get; set; }
        public RelayCommand AddConfigCommand { get; set; }

        public void OnOpenCommandExecuted(string fileName)
        {
            bool result = mModel.Deserialize(fileName);

            if (!result)
            {
                Trace.WriteLine(string.Format("Failed to load data from \"{0}\"!", fileName));
            }

            OnPropertyChanged("ActorNames");
            SelectedIndex = 0;

            AddConfigCommand.RaiseCanExecuteChanged();
        }

        public void OnSaveCommandExecuted(string fileName)
        {
            bool result = mModel.Serialize(fileName);

            if (!result)
            {
                Trace.WriteLine(string.Format("Failed to save data to \"{0}\"!", fileName));
            }
        }

        public bool CanSaveCommandExecute()
        {
            return mModel.HasLoaded;
        }

        public void OnAddCommandExecuted(object? sender)
        {
            mModel.AddConfig();

            OnPropertyChanged("ActorNames");
            SelectedIndex = mModel.ConfigCount - 1;
        }

        public bool CanAddCommandExecute(object? sender)
        {
            return mModel.HasLoaded;
        }

        public void OnDeleteCommandExecuted(object? sender)
        {
            //mModel.DeleteConfig(idx);

            OnPropertyChanged("ActorNames");
            SelectedIndex = 0;
        }

        private void OnExitCommandExecuted(object? sender)
        {
            Environment.Exit(0);
        }
        #endregion

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
