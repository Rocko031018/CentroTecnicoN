using System;
using System.Collections.Generic;

namespace SistemaVenta.AplicacionWeb.Models;

public partial class Brand
{
    public int IdBrand { get; set; }

    public string BrandName { get; set; } = null!;

    public virtual ICollection<ModelPhone> ModelPhones { get; set; } = new List<ModelPhone>();

    public virtual ICollection<Repair> Repairs { get; set; } = new List<Repair>();
}
