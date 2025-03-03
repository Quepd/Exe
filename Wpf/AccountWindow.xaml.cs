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
	/// Interaction logic for AccountWindow.xaml
	/// </summary>
	public partial class AccountWindow : Window
	{
		private Account LoggedInUser;

		public AccountWindow(Account user)
		{
			InitializeComponent();
			LoggedInUser = user;
			txtAccountName.Text = LoggedInUser.Name;
			
		}

		private void btnChangePassword_Click(object sender, RoutedEventArgs e)
		{
			string newPassword = txtNewPassword.Password;
			string confirmPassword = txtConfirmPassword.Password;

			if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
			{
				MessageBox.Show("Mật khẩu mới phải có ít nhất 6 ký tự!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			if (newPassword != confirmPassword)
			{
				MessageBox.Show("Xác nhận mật khẩu không khớp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			using (var db = new SysManageContext())
			{
				var user = db.Accounts.FirstOrDefault(a => a.Id == LoggedInUser.Id);
				if (user != null)
				{
					user.Password = newPassword;
					db.SaveChanges();
					MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
					this.Close();
				}
			}
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}


	}
}
