using Practico_Bloque_2.DTOs;      // Para trabajar con CreateProductoDto y ProductoDto
using Practico_Bloque_2.Models;     // Para trabajar con la entidad Producto
using Practico_Bloque_2.Repositories;// Para pedir datos al repositorio (IProductoRepository)

namespace Practico_Bloque_2.Services
{
    public class ProductoService : IProductoService
    // ':' = implementa la interfaz IProductoService.
    // Esta clase hace DOS cosas:
    //   1. Lógica de negocio (qué se puede y qué no)
    //   2. Conversión DTO <-> Modelo  (esa es su principal responsabilidad)
    {
        private readonly IProductoRepository _repository;
        // El repositorio del que depende.
        //  private  -> solo esta clase lo usa
        //  readonly -> solo se asigna en el constructor (no cambia después)
        //  IMPORTANTE: el tipo es la INTERFAZ, no la clase concreta.
        //    Así esta clase no sabe ni le importa si los datos vienen de
        //    memoria, SQL Server o un archivo. Eso lo decide el contenedor de DI.

        public ProductoService(IProductoRepository repository)
        // CONSTRUCTOR. El parámetro es lo que el contenedor de dependencias le INYECTA.
        // Cuando vos hiciste builder.Services.AddScoped<IProductoService, ProductoService>()
        // y un Controller pide un IProductoService, .NET:
        //   1. crea el ProductoService
        //   2. mira que su constructor necesita IProductoRepository
        //   3. se lo entrega (otra instancia creada por el contenedor)
        // Equivale a FastAPI: def svc(repo: IProductoRepository = Depends(get_repo))
        {
            _repository = repository;
            // Lo guarda en el campo privado para usarlo en todos los métodos de abajo
        }

        public bool Actualizar(int id, CreateProductoDto dto)
        // Devuelve true si actualizó, false si el Id no existía
        {
            var existente = _repository.ObtenerPorId(id);
            // Pide al repositorio el producto con ese Id.
            // Puede devolver null si no existe.

            if (existente == null)
            // ¿No hay nadie con ese Id?
            {
                return false;
                // -> avisamos al Controller que devuelva 404
            }

            var actualizado = new Producto
            // Construye un Producto NUEVO con los datos del DTO.
            // (Object Initializer: asignar los campos después del 'new')
            {
                Id = id,
                // El Id se toma de la URL, NO del body.
                // Así nadie puede mandar un body con id: 999 y cambiarle la identidad.

                Nombre = dto.Nombre,
                // Copia el nombre que mandó el cliente

                Precio = dto.Precio,
                // Copia el precio

                Categoria = dto.Categoria,
                // Copia la categoría

                Activo = existente.Activo
                // ⚠️ El 'Activo' NO viene del DTO (CreateProductoDto no lo tiene),
                //    así que se CONSERVA el valor que ya tenía el producto.
                //    Esto evita que un PUT sin querer reactive un producto dado de baja.
            };

            return _repository.Actualizar(actualizado);
            // Pide al repositorio que sobreescriba.
            // Devuelve true/false según existiera o no.
            // (Acá ya sabemos que existe, pero el repositorio vuelve a verificar)
        }

        public ProductoDto Agregar(CreateProductoDto dto)
        // Crea un producto nuevo y devuelve su DTO con el Id ya asignado
        {
            var producto = new Producto
            // Convierte el DTO -> Modelo interno
            {
                Nombre = dto.Nombre,
                // Copia el nombre

                Precio = dto.Precio,
                // Copia el precio

                Categoria = dto.Categoria,
                // Copia la categoría

                Activo = true
                // 📌 Regla de negocio: todo producto nuevo nace ACTIVADO.
                //    El cliente no lo pide, lo decide el servidor.
                //    (Eso es lógica de negocio = justamente el trabajo de esta capa)
            };
            // No se asigna 'Id' -> lo pone el repositorio (auto-incremento)

            var creado = _repository.Agregar(producto);
            // Guarda en la lista. Devuelve el MISMO objeto, ya con el Id puesto.

            var productoDto = new ProductoDto
            {
                Id = creado.Id,
                // Usa el Id que acaba de asignarle el repositorio

                Nombre = creado.Nombre,
                // Copia el nombre

                Precio = creado.Precio,
                // Copia el precio

                Categoria = creado.Categoria,
                // Copia la categoría

                Activo = creado.Activo
                // Copia el estado
            };

            return productoDto;
            // Devuelve el DTO (el Controller lo usa para armar el 201 + cabecera Location)
        }

        public bool Eliminar(int id)
        // Borra por Id. Devuelve true si existía.
        {
            return _repository.Eliminar(id);
            // No hay lógica de negocio extra: solo delega al repositorio en una línea.
        }

        public ProductoDto? ObtenerPorId(int id)
        // Devuelve el DTO, o null ('?') si no existe
        {
            var producto = _repository.ObtenerPorId(id);
            // Busca en la lista. Puede devolver null.

            if (producto == null)
            // ¿No existe?
            {
                return null;
                // -> el Controller traduce esto en un 404
            }

            var productoDto = new ProductoDto
            {
                Id = producto.Id,           // copia Id
                Nombre = producto.Nombre,   // copia Nombre
                Precio = producto.Precio,   // copia Precio
                Categoria = producto.Categoria, // copia Categoria
                Activo = producto.Activo    // copia Activo
            };

            return productoDto;
            // Convierte Modelo -> DTO antes de salir
        }

        public List<ProductoDto> ObtenerTodos()
        // Devuelve TODOS convertidos a DTO
        {
            var productos = _repository.ObtenerTodos();
            // Pide la lista completa (que ya es una COPIA, gracias a .ToList() del repo)

            var productosDto = productos.Select(p => new ProductoDto
            // 🔑 LINQ '.Select' = transforma cada elemento (como map() en Python)
            // p => ... es la lambda: toma un Producto 'p' y devuelve un ProductoDto
            // Equivale a Python:
            //   [ProductoDto(...) for p in productos]
            {
                Id = p.Id,                      // convierte campo por campo
                Nombre = p.Nombre,              // ...
                Precio = p.Precio,              // ...
                Categoria = p.Categoria,        // ...
                Activo = p.Activo               // ...
                // (este bloque es el mismo que el constructor de arriba, solo que inline)

            }).ToList();
            // '.ToList()' materializa el resultado en una lista real.
            // Sin esto, .Select queda "perezoso" y se ejecutaría recién al recorrerlo.

            return productosDto;
            // Devuelve la lista de DTOs -> el Controller la envuelve en Ok(...) = HTTP 200
        }
    }
}
// 🏗️ RESUMEN DE CAPAS EN TU APP:
//    Controllers/ProductosController  ->  decide códigos HTTP (200, 201, 204, 404)
//    Services/ProductoService         ->  lógica de negocio + conversión DTO  ← ESTE ARCHIVO
//    Repositories/ProductoRepository  ->  guarda/recupera datos
//    Models/Producto                  ->  la entidad
//    DTOs/*                           ->  lo que entra y sale por JSON
