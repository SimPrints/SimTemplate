using SimTemplate.ViewModels;
using SimTemplate.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimTemplate.Utilities
{
    public class WindowService : IWindowService
    {
        public void Show(object dataContext)
        {
            IDialogViewModel viewModel = (IDialogViewModel)dataContext;
            var win = new WindowDialogView();
            win.Title = viewModel.Title;
            win.Content = viewModel;
            win.Show();
        }

        public bool? ShowDialog(object dataContext)
        {
            IDialogViewModel viewModel = (IDialogViewModel)dataContext;
            var win = new WindowDialogView();
            win.Title = viewModel.Title;
            win.DataContext = viewModel;
            win.Owner = Application.Current.MainWindow;
            return win.ShowDialog();
        }
    }
}
