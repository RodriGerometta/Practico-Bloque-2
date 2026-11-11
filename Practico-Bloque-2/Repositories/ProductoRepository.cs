using Microsoft.EntityFrameworkCore;
using Practico_Bloque_2.Data;
using Practico_Bloque_2.Models;

namespace Practico_Bloque_2.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly AppDbContext _context;
        public ProductoRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool Actualizar(Producto producto)
        {
            var existente = _context.Productos
                             .FirstOrDefault(p => p.Id == producto.Id);

            if (existente == null) return false;

            existente.Nombre = producto.Nombre;
            existente.Precio = producto.Precio;
            existente.Categoria = producto.Categoria;
            existente.Activo = producto.Activo;

            _context.SaveChanges();
            return true;
        }

        public Producto Agregar(Producto producto)
        {
            _context.Productos.Add(producto);
            _context.SaveChanges();
            return producto;
        }

        public List<Producto> BuscarPorCategoria(string categoria)
        {
            return _context.Productos
                   .AsNoTracking()
                   .Where(p => p.Categoria == categoria)
                   .ToList();
        }

        public bool Eliminar(int id)
        {
            var existente = _context.Productos
                             .FirstOrDefault(p => p.Id == id);

            if (existente == null) return false;

            _context.Productos.Remove(existente);
            _context.SaveChanges();
            return true;
        }

        public Producto? ObtenerPorId(int id)
        {
            return _context.Productos
                   .AsNoTracking()
                   .FirstOrDefault(p => p.Id == id);
        }


        public List<Producto> ObtenerTodos()
        {
            return _context.Productos
                   .AsNoTracking()
                   .ToList();
        }
    }
}
