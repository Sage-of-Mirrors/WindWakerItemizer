using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindWakerItemizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel();
        }

        #region Open Command
        private void ApplicationCommandOpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.FileName = "ActorDat";
            openFileDialog.DefaultExt = ".bin";
            openFileDialog.Filter = "Enemy Item Drops (*.bin)|*.bin";

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                if (DataContext is ViewModel ctx)
                {
                    ctx.OnOpenCommandExecuted(openFileDialog.FileName);
                }
            }
        }

        private void ApplicationCommandOpenCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion

        #region Save Command
        private void ApplicationCommandSaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "ActorDat";
            saveFileDialog.DefaultExt = ".bin";
            saveFileDialog.Filter = "Enemy Item Drops (*.bin)|*.bin";

            if (saveFileDialog.ShowDialog() == true)
            {
                if (DataContext is ViewModel ctx)
                {
                    ctx.OnSaveCommandExecuted(saveFileDialog.FileName);
                }
            }
        }

        private void ApplicationCommandSaveCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;

            if (DataContext is ViewModel ctx)
            {
                e.CanExecute = ctx.CanSaveCommandExecute();
            }
        }
        #endregion
    }
}