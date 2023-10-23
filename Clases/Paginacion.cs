using Microsoft.EntityFrameworkCore;
using Titulacion.Clases.Get;

namespace Titulacion.Clases
{
    public class Paginacion<T>: List<T>
    {
        public int PaginaInicio { get; private set; }
        public int PaginasTotales { get; private set; }

        public Paginacion(List<T> items, int contador, int paginaInicio, int cantidad)
        {
            PaginaInicio = paginaInicio;
            PaginasTotales = (int)Math.Ceiling(contador / (double)cantidad);
            this.AddRange(items);
        }

        public bool PaginasAnteriores => PaginaInicio > 1;

        public bool PaginasPosteriores => PaginaInicio < PaginasTotales;

        public int BotonesAnteriores()
        {
            if (PaginaInicio - 2 > 0)
                return PaginaInicio - 2;
            if (PaginaInicio - 2 == 0)
                return PaginaInicio - 1;
            return PaginaInicio;
        }
        public int BotonesSiguientes()
        {
            if (PaginaInicio + 2 < PaginasTotales + 1)
                return PaginaInicio + 2;
            if (PaginaInicio + 2 == PaginasTotales + 1)
                return PaginaInicio + 1;
            return PaginaInicio;
        }

        public static Paginacion<T> CrearLista(List<T> fuente, int pagaInicio, int cantidad)
        {
            var contador = fuente.Count;
            var items = fuente.Skip((pagaInicio - 1 ) * cantidad).Take(cantidad).ToList();
            return new Paginacion<T>(items, contador, pagaInicio, cantidad);
        }
    }
}
