// Copyright 2016 Sam Briggs
//
// This file is part of SimTemplate.
//
// SimTemplate is free software: you can redistribute it and/or modify it under the
// terms of the GNU General Public License as published by the Free Software 
// Foundation, either version 3 of the License, or (at your option) any later
// version.
//
// SimTemplate is distributed in the hope that it will be useful, but WITHOUT ANY 
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
// A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// SimTemplate. If not, see http://www.gnu.org/licenses/.
//
using SimTemplate.ViewModels;
using SimTemplate.ViewModels.Interfaces;
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
