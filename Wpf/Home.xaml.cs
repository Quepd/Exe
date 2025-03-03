using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
	/// Interaction logic for Home.xaml
	/// </summary>
	public partial class Home : Window, INotifyPropertyChanged
	{
        private int userId;
        private Account LoggedInUser;
		public ObservableCollection<Category> Categories { get; set; }

		public ObservableCollection<Item> Items { get; set; }
		private ObservableCollection<Item> AllItems { get; set; } 
		public ObservableCollection<OrderItem> SelectedItems { get; set; }
		public int TotalOrderPrice => SelectedItems.Sum(i => i.TotalPrice) + 10000 + 50000;

		private Category _selectedCategory;
		public Category SelectedCategory
		{
			get { return _selectedCategory; }
			set
			{
				if (_selectedCategory != value)
				{
					_selectedCategory = value;
					OnPropertyChanged(nameof(SelectedCategory));
					LoadItemsByCategory(_selectedCategory?.Id ?? 0);
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public Home(int userId)
		{
           
            InitializeComponent();
            this.userId = userId;
            Categories = new ObservableCollection<Category>();
			Items = new ObservableCollection<Item>();
			AllItems = new ObservableCollection<Item>(); 
			SelectedItems = new ObservableCollection<OrderItem>();
			SelectedItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalOrderPrice));
			LoadUserInfo(userId);
			LoadCategories();
			DataContext = this;
            
        }
		private void LoadUserInfo(int userId)
		{
			using (var db = new SysManageContext())
			{
				LoggedInUser = db.Accounts.FirstOrDefault(a => a.Id == userId);

				if (LoggedInUser != null)
				{
					txtUserName.Text = LoggedInUser.Name;
					txtUserRole.Text = (LoggedInUser.Id == 1) ? "Admin" : "Nhân viên";
				}
			}
		}

		private void LoadCategories()
		{
			using (var context = new SysManageContext())
			{
				var categoriesFromDb = context.Categories.ToList();
				categoriesFromDb.Insert(0, new Category { Id = 0, Name = "Tất cả sản phẩm" }); 
				Categories = new ObservableCollection<Category>(categoriesFromDb);
				SelectedCategory = Categories.FirstOrDefault(); 
				OnPropertyChanged(nameof(Categories));
			}
		}

		private void LoadItemsByCategory(int categoryId)
		{
			using (var context = new SysManageContext())
			{
				var items = (categoryId == 0)
					? context.Items.ToList() 
					: context.Items.Where(i => i.Category == categoryId).ToList();

				AllItems = new ObservableCollection<Item>(items);
				FilterItems(txtSearch.Text);
			}
		}


		private void btnSearch_Click(object sender, RoutedEventArgs e)
		{
			string keyword = txtSearch.Text.Trim().ToLower();
			FilterItems(keyword);
			txtSearch.Clear();
		}

		private void FilterItems(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				Items = new ObservableCollection<Item>(AllItems); 
			}
			else
			{
				Items = new ObservableCollection<Item>(
					AllItems.Where(i => i.Name.ToLower().Contains(keyword))
				);
			}
			OnPropertyChanged(nameof(Items));
		}
		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button button && button.Tag is Item selectedItem)
			{
				var existingOrderItem = SelectedItems.FirstOrDefault(x => x.Item.Id == selectedItem.Id);

				if (existingOrderItem != null)
				{
					existingOrderItem.Quantity++;
				}
				else
				{
					SelectedItems.Add(new OrderItem { Item = selectedItem, Quantity = 1 });
				}

				OnPropertyChanged(nameof(SelectedItems));
			}
		}
		private void btnIncrease_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button button && button.Tag is OrderItem orderItem)
			{
				orderItem.Quantity++;
				OnPropertyChanged(nameof(TotalOrderPrice)); 
			}
		}

		private void btnDecrease_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button button && button.Tag is OrderItem orderItem)
			{
				if (orderItem.Quantity > 1)
				{
					orderItem.Quantity--;
				}
				else
				{
					SelectedItems.Remove(orderItem);
				}
				OnPropertyChanged(nameof(TotalOrderPrice)); 
			}
		}

		private void btnCheckout_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedItems.Count == 0)
			{
				MessageBox.Show("Chưa có món nào trong đơn hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			using (var context = new SysManageContext())
			{
				using (var transaction = context.Database.BeginTransaction()) 
				{
					try
					{
						
						int maxOrderId = context.Orders.Any() ? context.Orders.Max(o => o.Id) : 0;

						var order = new Order
						{
							Id = maxOrderId + 1,
							Total = TotalOrderPrice
						};

						context.Orders.Add(order);
						context.SaveChanges(); 

						foreach (var selectedItem in SelectedItems)
						{
							
							var existingOrderItem = context.OrderItems
								.FirstOrDefault(oi => oi.OrderId == order.Id && oi.ItemId == selectedItem.Item.Id);

							if (existingOrderItem == null)
							{
								var orderItem = new OrderItem
								{
									OrderId = order.Id,
									ItemId = selectedItem.Item.Id,
									Quantity = selectedItem.Quantity
								};

								context.OrderItems.Add(orderItem);
							}
						}

						context.SaveChanges(); 
						transaction.Commit();  
					}
					catch (Exception ex)
					{
						transaction.Rollback(); 
						MessageBox.Show($"Lỗi khi thanh toán: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}
				}
			}

			MessageBox.Show("Thanh toán thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

			
			SelectedItems.Clear();
			OnPropertyChanged(nameof(SelectedItems));
			OnPropertyChanged(nameof(TotalOrderPrice));
		}

		private void BtnLogout_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận",
													  MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (result == MessageBoxResult.Yes)
			{
				

				// Hiển thị lại màn hình đăng nhập
				Login loginWindow = new Login();
				loginWindow.Show();

				// Đóng cửa sổ hiện tại (Home)
				this.Close();
			}
		}



		private void btnAccount_Click(object sender, RoutedEventArgs e)
		{
			AccountWindow accountWindow = new AccountWindow(LoggedInUser);
			accountWindow.ShowDialog();
		}


        private void btnManagement_Click(object sender, RoutedEventArgs e)
        {
            Management managementWindow = new Management(userId); // Truyền userId
            managementWindow.Show();
            this.Close(); // Đóng màn hình Home
        }



    }
}
