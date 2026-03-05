namespace SistemaDeCursos.ViewModels
{
    public class CarritoPaginaViewModel
    {
        public List<CarritoViewModel> Items { get; set; } = new();
        public decimal Total => Items.Sum(x => x.Subtotal);
    }
}
