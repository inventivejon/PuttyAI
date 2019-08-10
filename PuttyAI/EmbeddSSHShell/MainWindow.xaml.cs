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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EmbeddSSHShell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
        }

        EmbeddingPanel thisPanel = null;

        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            if (thisPanel == null)
            {
                thisPanel = new EmbeddingPanel(winFormsHost);
            }
            else
            {
                
                
            }
        }

        private void TC_SetFocus(Object sender, SelectionChangedEventArgs e)
        {
            var tc = sender as TabControl; //The sender is a type of TabControl...

            if (tc != null)
            {
                if (tc.SelectedIndex == 0)
                {
                    thisPanel?.SetFocus();
                }
            }
        }

        private void BtnSet_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
