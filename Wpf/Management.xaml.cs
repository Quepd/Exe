using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Wpf.Models;

namespace Wpf
{
	public partial class Management : Window
	{

        private int userId;
        public Management(int id)
		{
			InitializeComponent();
            userId = id;
        }

		private void BtnTongQuan_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new TongQuan();
		}

		private void BtnNhanVien_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new NhanVien();
		}
        private void BtnCategory_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Categories();
        }
        private void BtnItem_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Items();
        }
        private void BtnBackToHome_Click(object sender, RoutedEventArgs e)
        {
            // Mở lại trang Home
            Home homeWindow = new Home(userId);
            homeWindow.Show();

            // Đóng trang Management
            this.Close();
        }
    }
}
