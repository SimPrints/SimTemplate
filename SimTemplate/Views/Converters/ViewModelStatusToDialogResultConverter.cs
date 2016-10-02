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
using SimTemplate.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SimTemplate.Views.Converters
{
    public class ViewModelStatusToDialogResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? dialogResult = false;
            if (value.Equals(ViewModelStatus.Running))
            {
                dialogResult = null;
            }
            else if (value.Equals(ViewModelStatus.Complete))
            {
                dialogResult = true;
            }
            return dialogResult;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? dialogResult = value as bool?;

            ViewModelStatus status = ViewModelStatus.None;
            if (!dialogResult.HasValue)
            {
                status = ViewModelStatus.Running;
            }
            else if (!dialogResult.Value)
            {
                status = ViewModelStatus.NoChange;
            }
            return (status != ViewModelStatus.None) ? status : Binding.DoNothing;
        }
    }
}
