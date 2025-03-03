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
	/// Interaction logic for NhanVienForm.xaml
	/// </summary>
	public partial class NhanVienForm : Window
	{
		public Account Employee { get; private set; }

		public NhanVienForm(Account account = null)
		{
			InitializeComponent();
			Employee = account ?? new Account(); // Nếu không có thì tạo mới

			// Nếu sửa, hiển thị thông tin nhân viên
			if (account != null)
			{
				txtName.Text = account.Name;
				txtPassword.Password = account.Password;
			}
		}

		private void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			Employee.Name = txtName.Text;
			Employee.Password = txtPassword.Password;

			if (string.IsNullOrWhiteSpace(Employee.Name) || string.IsNullOrWhiteSpace(Employee.Password))
			{
				MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			DialogResult = true; // Đóng cửa sổ và trả về kết quả
			Close();
		}

		private void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false; // Đóng cửa sổ mà không lưu
			Close();
		}
	}
}
