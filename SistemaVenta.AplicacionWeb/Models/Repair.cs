using System;
using System.Collections.Generic;

namespace SistemaVenta.AplicacionWeb.Models;

public partial class Repair
{
    public int IdRep { get; set; }

    public int? Client { get; set; }

    public string Ubication { get; set; } = null!;

    public long? NumPhone { get; set; }

    public string TipeEq { get; set; } = null!;

    public int? IdBrand { get; set; }

    public int? IdModel { get; set; }

    public string Failure { get; set; } = null!;

    public decimal? Deposit { get; set; }

    public string Condition { get; set; } = null!;

    public virtual Usuario? ClientNavigation { get; set; }

    public virtual Brand? IdBrandNavigation { get; set; }

    public virtual ModelPhone? IdModelNavigation { get; set; }
}
