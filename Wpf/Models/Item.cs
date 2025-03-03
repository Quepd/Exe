using System;
using System.Collections.Generic;

namespace Wpf.Models;

public partial class Item
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Price { get; set; }

    public int Category { get; set; }

    public virtual Category CategoryNavigation { get; set; } = null!;
}
