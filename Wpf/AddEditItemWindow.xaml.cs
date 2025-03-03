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
    /// Interaction logic for AddEditItemWindow.xaml
    /// </summary>
    public partial class AddEditItemWindow : Window
    {
        private Item _currentItem;
        public event Action ItemUpdated;

        public AddEditItemWindow(Item item)
        {
            InitializeComponent();
            LoadCategories();
            _currentItem = item;

            if (item != null)
            {
                txtItemName.Text = item.Name;
                txtItemPrice.Text = item.Price.ToString();
                cbCategory.SelectedValue = item.Category;
                Title = "Chỉnh sửa Sản phẩm";
            }
            else
            {
                Title = "Thêm Sản phẩm";
            }
        }

        private void LoadCategories()
        {
            using (var db = new SysManageContext())
            {
                cbCategory.ItemsSource = db.Categories.OrderBy(c => c.Id).ToList();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new SysManageContext())
            {
                if (string.IsNullOrWhiteSpace(txtItemName.Text) || string.IsNullOrWhiteSpace(txtItemPrice.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(txtItemPrice.Text, out int price))
                {
                    MessageBox.Show("Giá sản phẩm phải là số nguyên!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cbCategory.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn danh mục!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int categoryId = (int)cbCategory.SelectedValue;

                if (_currentItem == null) // Thêm mới sản phẩm
                {
                    int maxId = db.Items.Any() ? db.Items.Max(i => i.Id) : 0; // Tìm ID lớn nhất
                    var newItem = new Item
                    {
                        Id = maxId + 1, // Gán ID mới = maxId + 1
                        Name = txtItemName.Text,
                        Price = price,
                        Category = categoryId
                    };

                    db.Items.Add(newItem);
                    db.SaveChanges();
                    MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else // Chỉnh sửa sản phẩm
                {
                    var item = db.Items.FirstOrDefault(i => i.Id == _currentItem.Id);
                    if (item != null)
                    {
                        item.Name = txtItemName.Text;
                        item.Price = price;
                        item.Category = categoryId;

                        db.SaveChanges();
                        MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                this.Close(); // Đóng cửa sổ sau khi lưu
            }
        }


        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
