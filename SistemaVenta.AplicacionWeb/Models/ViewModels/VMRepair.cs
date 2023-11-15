namespace SistemaVenta.AplicacionWeb.Models.ViewModels
{
    public class VMRepair
    {
        public int IdRep { get; set; }

        public string Client { get; set; }

        public string Imei { get; set; } = null!;

        public string NumPhone { get; set; }

        public string TipeEq { get; set; } = null!;

        public string Brand { get; set; }

        public string Model { get; set; }

        public string Failure { get; set; } = null!;

        public string Deposit { get; set; }

        public string Condition { get; set; } = null!;
    }
}
