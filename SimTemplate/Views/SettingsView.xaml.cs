using SimTemplate.DataTypes;
using SimTemplate.ViewModels;
using SimTemplate.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimTemplate.View
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsView : ContentControl
    {
        private SettingsViewModel m_ViewModel;

        public SettingsView()
        {
            InitializeComponent();
            DataContextChanged += SettingsView_DataContextChanged;
        }

        #region Event Handlers

        private void SettingsView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            m_ViewModel = e.NewValue as SettingsViewModel;
            if (m_ViewModel != null)
            {
                // ViewModel has just been set as context to a view
                m_ViewModel.NotifyAllPropertiesChanged();
            }
        }

        #endregion
    }
}
