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
using Wpf.Models;

namespace Wpf
{
	/// <summary>
	/// Interaction logic for NhanVien.xaml
	/// </summary>
	public partial class NhanVien : UserControl
	{
		private SysManageContext _context = new SysManageContext();
		public NhanVien()
		{
			InitializeComponent();
			LoadAccounts();
		}

		private void LoadAccounts()
		{
			using (var db = new SysManageContext())
			{
				// Lấy tất cả tài khoản trừ tài khoản có Id = 1
				var accounts = db.Accounts.Where(a => a.Id != 1).ToList();
				dgAccounts.ItemsSource = accounts;
			}
		}
		// Mở form thêm nhân viên
		private void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			using (var db = new SysManageContext())
			{
				// Tìm ID cao nhất hiện có
				int newId = db.Accounts.Any() ? db.Accounts.Max(a => a.Id) + 1 : 1;

				var newAccount = new Account { Id = newId }; // Gán ID mới

				var form = new NhanVienForm(newAccount);
				if (form.ShowDialog() == true)
				{
					db.Accounts.Add(form.Employee);
					db.SaveChanges();
					LoadAccounts();
				}
			}
		}


		// Mở form sửa nhân viên
		private void BtnEdit_Click(object sender, RoutedEventArgs e)
		{
			if (dgAccounts.SelectedItem is Account selectedAccount)
			{
				var form = new NhanVienForm(selectedAccount);
				if (form.ShowDialog() == true)
				{
					using (var db = new SysManageContext())
					{
						var accountToUpdate = db.Accounts.Find(selectedAccount.Id);
						if (accountToUpdate != null)
						{
							accountToUpdate.Name = form.Employee.Name;
							accountToUpdate.Password = form.Employee.Password;
							db.SaveChanges();
						}
					}
					LoadAccounts();
				}
			}
			else
			{
				MessageBox.Show("Vui lòng chọn nhân viên cần sửa!");
			}
		}

		// Xóa nhân viên
		private void BtnDelete_Click(object sender, RoutedEventArgs e)
		{
			if (dgAccounts.SelectedItem is Account selectedAccount)
			{
				if (MessageBox.Show("Bạn có chắc chắn muốn xóa nhân viên này?", "Xác nhận",
					MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
				{
					using (var db = new SysManageContext())
					{
						db.Accounts.Remove(selectedAccount);
						db.SaveChanges();
					}
					LoadAccounts();
				}
			}
			else
			{
				MessageBox.Show("Vui lòng chọn nhân viên cần xóa!");
			}
		}

	}
}
