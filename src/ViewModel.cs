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
        #region Relay commands
        public RelayCommand ExitCommand { get; set; }
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

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

                SelectedConfig = mModel.GetConfig(value);
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

        public Items Items
        {
            get;
            set;
        }

        private Model mModel;
        private int mSelectedIndex;
        private ItemDropConfig? mSelectedConfig;

        public ViewModel()
        {
            ExitCommand = new RelayCommand(OnExitCommandExecuted, (X) => true);

            mModel = new Model();
        }

        public void OnOpenCommandExecuted(string fileName)
        {
            bool result = mModel.Deserialize(fileName);

            if (!result)
            {
                Trace.WriteLine(string.Format("Failed to load data from \"{0}\"!", fileName));
            }
        }

        #region Exit command
        private void OnExitCommandExecuted(object? sender)
        {
            Environment.Exit(0);
        }
        #endregion

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
