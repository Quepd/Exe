using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Import để dùng [Column]

namespace Wpf.Models;

public partial class OrderItem : INotifyPropertyChanged
{
	public Item Item { get; set; }

	private int _quantity;

	[Column("NoI")] // Map cột NoI trong DB với property Quantity
	public int Quantity
	{
		get => _quantity;
		set
		{
			_quantity = value;
			OnPropertyChanged(nameof(Quantity));
			OnPropertyChanged(nameof(TotalPrice)); // Cập nhật tổng tiền khi số lượng thay đổi
		}
	}

	public int TotalPrice => Item.Price * Quantity; // Tính tổng tiền

	public int OrderId { get; set; }
	public int ItemId { get; set; }

	public event PropertyChangedEventHandler PropertyChanged;
	protected void OnPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
