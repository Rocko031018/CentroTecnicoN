using System;
using System.Collections.Generic;

namespace SistemaVenta.AplicacionWeb.Models;

public partial class ModelPhone
{
    public int IdModel { get; set; }

    public string ModelName { get; set; } = null!;

    public int? IdBrand { get; set; }

    public virtual Brand? IdBrandNavigation { get; set; }

    public virtual ICollection<Repair> Repairs { get; set; } = new List<Repair>();
}
