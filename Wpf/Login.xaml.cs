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
using Wpf.Models;

namespace Wpf
{
	/// <summary>
	/// Interaction logic for Login.xaml
	/// </summary>
	public partial class Login : Window
	{
		public Login()
		{
			InitializeComponent();
		}
		private void btnLogin_Click(object sender, RoutedEventArgs e)
		{
			string username = txtUsername.Text;
			string password = txtPass.Password;

			using (var db = new SysManageContext())
			{
				var accountMember = db.Accounts.FirstOrDefault(a => a.Name == username);

				if (accountMember == null)
				{
					MessageBox.Show("Tài khoản không tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				if (accountMember.Password == password)
				{
					MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

					// Mở màn hình Home và truyền ID người dùng
					Home homeWindow = new Home(accountMember.Id);
					homeWindow.Show();

					// Đóng màn hình Login
					this.Close();
				}
				else
				{
					MessageBox.Show("Mật khẩu không chính xác!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}


		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
