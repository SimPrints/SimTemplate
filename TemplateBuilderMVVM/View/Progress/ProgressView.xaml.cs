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
using System.Windows.Shapes;
using TemplateBuilder.Helpers;
using TemplateBuilder.ViewModel.Progress;

namespace TemplateBuilder.View.Progress
{
    /// <summary>
    /// Interaction logic for ProgressView.xaml
    /// </summary>
    public partial class ProgressView : Window
    {
        private ProgressViewModel m_ViewModel;

        public ProgressView(ProgressViewModel viewModel)
        {
            IntegrityCheck.IsNotNull(viewModel);
            m_ViewModel = viewModel;
            InitializeComponent();
            DataContext = m_ViewModel;
        }

        public ProgressViewModel ViewModel { get { return m_ViewModel; } }
    }
}
