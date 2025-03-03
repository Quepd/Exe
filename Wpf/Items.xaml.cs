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
    /// Interaction logic for Items.xaml
    /// </summary>
    public partial class Items : UserControl
    {
        public Items()
        {
            InitializeComponent();
            LoadItems();
        }

        private void LoadItems()
        {
            using (var db = new SysManageContext())
            {
                dgItems.ItemsSource = db.Items.OrderBy(i => i.Id).ToList();
            }
        }

        private void BtnAddItem_Click(object sender, RoutedEventArgs e)
        {
            var addEditWindow = new AddEditItemWindow(null);
            addEditWindow.ShowDialog(); // Mở cửa sổ thêm sản phẩm

            LoadItems(); // Tải lại danh sách sản phẩm sau khi đóng cửa sổ
        }

        private void BtnEditItem_Click(object sender, RoutedEventArgs e)
        {
            if (dgItems.SelectedItem is Item selectedItem)
            {
                var editWindow = new AddEditItemWindow(selectedItem);
                editWindow.ShowDialog(); // Mở cửa sổ chỉnh sửa sản phẩm

                LoadItems(); // Cập nhật bảng sau khi chỉnh sửa
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần chỉnh sửa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (dgItems.SelectedItem is Item selectedItem)
            {
                if (MessageBox.Show("Bạn có chắc muốn xóa sản phẩm này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (var db = new SysManageContext())
                    {
                        var item = db.Items.FirstOrDefault(i => i.Id == selectedItem.Id);
                        if (item != null)
                        {
                            db.Items.Remove(item);
                            db.SaveChanges();
                            LoadItems();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sản phẩm để xóa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
