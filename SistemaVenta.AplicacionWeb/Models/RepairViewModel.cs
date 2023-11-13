namespace SistemaVenta.AplicacionWeb.Models
{
    public class RepairViewModel
    {
        public Repair Repair { get; set; }
        public List<Usuario> Clientes { get; set; } = new List<Usuario>();
        public List<Brand> Brands { get; set; }
        public List<ModelPhone> ModelPhones { get; set; }
    }
}
