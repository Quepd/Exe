using Microsoft.EntityFrameworkCore;
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
	/// Interaction logic for TongQuan.xaml
	/// </summary>
	public partial class TongQuan : UserControl
	{
		private SysManageContext _context = new SysManageContext();
		public TongQuan()
		{
			InitializeComponent();
			LoadRevenueOverview();  // Load dữ liệu khi mở cửa sổ
			LoadRankingTable();     // Load bảng xếp hạng món ăn
		}
		private void LoadRevenueOverview()
		{
			var orders = _context.Orders.ToList(); // Lấy toàn bộ đơn hàng

			int totalOrders = orders.Count;
			double avgItems = _context.OrderItems.Any() ? _context.OrderItems.Average(oi => oi.Quantity) : 0;
			double avgRevenue = orders.Any() ? orders.Average(o => o.Total) : 0;
			double totalRevenue = orders.Sum(o => o.Total);

			txtTotalOrders.Text = totalOrders.ToString();
			txtAvgItems.Text = avgItems.ToString("0.0");
			txtAvgRevenue.Text = avgRevenue.ToString("N0") + " VND";
			txtTotalRevenue.Text = totalRevenue.ToString("N0") + " VND";
		}


		private void LoadRankingTable()
		{
			var ranking = _context.OrderItems
				.GroupBy(oi => oi.Item.Name)
				.Select(g => new { ItemName = g.Key, Sold = g.Sum(oi => oi.Quantity) })
				.OrderByDescending(r => r.Sold)
				.ToList();

			rankingTable.ItemsSource = ranking;
		}

	}
}
