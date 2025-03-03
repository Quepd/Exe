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
    /// Interaction logic for Categories.xaml
    /// </summary>
    public partial class Categories : UserControl
    {
       
        private SysManageContext _db = new SysManageContext();
        private int selectedCategoryId = -1;
         public Categories()
        {
            InitializeComponent();
            LoadCategories();
        }


        private void LoadCategories()
        {
            using (var db = new SysManageContext())
            {
                dgCategories.ItemsSource = db.Categories
                                             .OrderBy(c => c.Id) // Sắp xếp ID tăng dần
                                             .ToList();
            }
        }


        private void BtnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                using (var db = new SysManageContext())
                {
                    string newCategoryName = txtCategoryName.Text.Trim();

                    // Kiểm tra danh mục đã tồn tại chưa
                    bool isExist = db.Categories.Any(c => c.Name == newCategoryName);
                    if (isExist)
                    {
                        MessageBox.Show("Danh mục đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Lấy ID cao nhất +1
                    int newId = (db.Categories.Any()) ? db.Categories.Max(c => c.Id) + 1 : 1;

                    var newCategory = new Category
                    {
                        Id = newId,
                        Name = newCategoryName
                    };

                    db.Categories.Add(newCategory);
                    db.SaveChanges();
                }

                LoadCategories();
                txtCategoryName.Clear();
            }
            else
            {
                MessageBox.Show("Vui lòng nhập tên danh mục!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void BtnEditCategory_Click(object sender, RoutedEventArgs e)
        {
            if (dgCategories.SelectedItem is Category selectedCategory)
            {
                if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên danh mục!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var db = new SysManageContext())
                {
                    var category = db.Categories.FirstOrDefault(c => c.Id == selectedCategory.Id);
                    if (category != null)
                    {
                        // Kiểm tra xem có danh mục nào khác có cùng tên không
                        if (db.Categories.Any(c => c.Name == txtCategoryName.Text.Trim() && c.Id != category.Id))
                        {
                            MessageBox.Show("Tên danh mục đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        category.Name = txtCategoryName.Text.Trim();
                        db.SaveChanges();
                        LoadCategories();

                        // Xóa nội dung ô nhập sau khi chỉnh sửa
                        txtCategoryName.Clear();

                        // Reset danh mục được chọn
                        dgCategories.SelectedItem = null;
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn danh mục cần chỉnh sửa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }



        private void BtnDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (dgCategories.SelectedItem is Category selectedCategory)
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa danh mục này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new SysManageContext())
                    {
                        var category = db.Categories.FirstOrDefault(c => c.Id == selectedCategory.Id);
                        if (category != null)
                        {
                            db.Categories.Remove(category);
                            db.SaveChanges();
                            LoadCategories();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn danh mục cần xóa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void dgCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCategories.SelectedItem is Category selectedCategory)
            {
                selectedCategoryId = selectedCategory.Id;
                txtCategoryName.Text = selectedCategory.Name;
            }
        }

    }
}
