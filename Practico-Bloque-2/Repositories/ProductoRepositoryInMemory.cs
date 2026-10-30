using Practico_Bloque_2.Models;         // Importa 'Producto', que SÍ se usa (es el tipo de la lista)

namespace Practico_Bloque_2.Repositories// Namespace
{
    public class ProductoRepositoryInMemory : IProductoRepository
                                        // ':' = "implementa la interfaz" (en Python sería: class ...(IProductoRepository):)
                                        // El compilador OBLIGA a escribir los 6 métodos de la interfaz.
                                        // Si falta uno -> error de compilación.
    {
        // Propiedades
        private readonly List<Producto> _productos;
                                        // LA "BASE DE DATOS". Tres palabras clave:
                                        //  private  -> solo esta clase lo ve (encapsulación)
                                        //  readonly -> SOLO se puede asignar en el constructor. Después no cambia.
                                        //             (la lista puede cambiar de contenido, pero no se reemplaza por otra)
                                        //  _productos -> convención .NET: '_' + camelCase para campos privados

        // Constructores
        public ProductoRepositoryInMemory()
                                        // Constructor SIN parámetros = el __init__ de Python.
                                        // Se ejecuta solo al hacer 'new ProductoRepositoryInMemory()'
        {
            _productos = new List<Producto>
                                        // Crea la lista con productos SEMILLA (datos iniciales de prueba).
                                        // OJO: esto significa que cada 'dotnet run' REINICIA los datos.
            {
                new Producto{Id = 1, Nombre = "Producto1", Precio = 1000, Categoria = "Categoria 1", Activo = true},
                                        // Object Initializer: se crean los campos DESPUÉS del 'new', con '='.
                                        // Equivale en Python a: Producto(id=1, nombre="Producto1", ...)
                new Producto{Id = 2, Nombre = "Producto2", Precio = 2000, Categoria = "Categoria 2", Activo = false},
                                        // Id 2, precio 2000, categoría 2, inactivo
                new Producto{Id = 3, Nombre = "Producto3", Precio = 3000, Categoria = "Categoria 3", Activo = true},
                                        // Id 3, precio 3000, categoría 3, activo
                new Producto{Id = 4, Nombre = "Producto4", Precio = 4000, Categoria = "Categoria 1", Activo = false},
                                        // Id 4, precio 4000, categoría 1 (repite la de Id 1), inactivo
                new Producto{Id = 5, Nombre = "Producto5", Precio = 5000, Categoria = "Categoria 2", Activo = true}
                                        // Id 5, precio 5000, categoría 2 (repite la de Id 2), activo
            };                          // Cierre de la lista (¡no olvidar el ';')
        }                               // Fin del constructor



        // Metodos
        public bool Actualizar(Producto producto)
                                        // Devuelve 'true' si actualizó, 'false' si el producto no existía.
                                        // (esto cumple el contrato de la interfaz)
        {
            var existente = _productos.FirstOrDefault(p => p.Id == producto.Id);
                                        // 🔑 LINQ 'FirstOrDefault': busca el PRIMERO que cumpla la condición.
                                        // p => p.Id == producto.Id es una LAMBDA = lambda p: p.Id == producto.id
                                        // Si NO encuentra nada devuelve NULL (no lanza error).
                                        // Equivale a Python:
                                        //   next((p for p in _productos if p.id == producto.id), None)

            if (existente == null)
                                        // ¿No existe ningún producto con ese Id?
            {
                return false;           // -> devolvemos false (no se pudo actualizar)
            }
            else
                                        // Si sí existe...
                                        // (💡 estilo: el 'else' acá es innecesario, ya se hizo 'return' arriba)
            {
                existente.Nombre = producto.Nombre;
                                        // Copia el nombre. IMPORTANTE: 'existente' es una REFERENCIA al objeto
                                        // que está DENTRO de la lista, así que esto MODIFICA la lista directo.
                                        // En Python pasa exactamente igual: todo es referencia.
                existente.Precio = producto.Precio;
                                        // Copia el precio
                existente.Categoria = producto.Categoria;
                                        // Copia la categoría
                existente.Activo = producto.Activo;
                                        // Copia el flag de activo
                // NOTA: NO se copia el 'Id'. Es correcto: el Id es la identidad y no debe cambiar.

                return true;            // -> devolvemos true (actualización exitosa)
            }
        }                               // Fin de Actualizar

        public List<Producto> BuscarPorCategoria(string categoria)
                                        // Devuelve solo los productos de esa categoría
        {
            var category = _productos.Where(p => p.Categoria == categoria).ToList();
                                        // 🔑 LINQ:
                                        //   .Where(...)  = FILTRO, igual que filter() o [x for x in l if ...]
                                        //   .ToList()    = MATERIALIZA en una lista nueva.
                                        //     Sin esto, .Where queda "perezoso" y se evalúa recién al recorrerlo.
                                        // Equivale a Python: [p for p in _productos if p.categoria == categoria]
            return category;
                                        // Devuelve la lista filtrada
                                        // (💡 podría ser una sola línea: return _productos.Where(...).ToList();)
        }                               // Fin de BuscarPorCategoria

        public bool Eliminar(int id)
                                        // Devuelve 'true' si borró, 'false' si no existía
        {
            var existente = _productos.FirstOrDefault(p => p.Id == id);
                                        // Busca el objeto por Id. Necesitamos la REFERENCIA (no solo el número)
                                        // porque 'Remove' trabaja comparando objetos.

            if (existente == null)
                                        // ¿No existe?
            {
                return false;           // -> nada que borrar
            }
            else
                                        // Si existe...
            {
                _productos.Remove(existente);
                                        // 🔑 Quita el PRIMER elemento que coincida con esa referencia.
                                        // (En Python: _productos.remove(existente) pero lanza ValueError si no está;
                                        //  acá lo evitamos con el 'if' de arriba)
                return true;            // -> borrado exitoso
            }

        }                               // Fin de Eliminar

        public Producto ? ObtenerPorId(int id)
                                        // Devuelve el producto, o NULL si no existe.
                                        // El '?' en el tipo de retorno AVISA a quien llama de que puede recibir null.
        {
            var product = _productos.FirstOrDefault(p => p.Id == id);
                                        // Busca por Id; si no hay coincidencia devuelve NULL
            return product;
                                        // Devuelve el resultado
                                        // (💡 podría ser una sola línea: return _productos.FirstOrDefault(p => p.Id == id);)
        }                               // Fin de ObtenerPorId

        public List<Producto> ObtenerTodos()
                                        // Devuelve TODA la lista
        {
            return _productos.ToList();
                                        // 🏆 '.ToList()' crea una COPIA. Esto está BIEN HECHO:
                                        // si devolvieras '_productos' directo, el llamador podría hacer
                                        // .Clear() y BORRAR TODA la base de datos.
                                        // Al copiar, la lista interna queda protegida.
                                        // Python equivalente seguro: return list(self._productos)
        }                               // Fin de ObtenerTodos

        public Producto Agregar(Producto producto)
                                        // Agrega un producto nuevo y lo devuelve CON su Id ya asignado
        {
            int nuevoId;                // Declara la variable SIN asignarla todavía.
                                        // C# exige que esté asignada antes de leerla (análisis de asignación)

            if (_productos.Any())
                                        // 🔑 '.Any()' = ¿la lista tiene al menos 1 elemento? -> bool
                                        // En Python dirías 'if _productos:', pero en C# eso NO compila.
                                        // Equivale a: if len(_productos) > 0:
            {
                int idMax = _productos.Max(p => p.Id);
                                        // '.Max(...)' = el ID más grande de la lista.
                                        // Equivale a: max(p.id for p in _productos)
                                        // Si la lista estuviera vacía lanzaría excepción (igual que max() en Python),
                                        // por eso el chequeo con .Any() de arriba
                nuevoId = idMax + 1;
                                        // El siguiente Id libre = máximo + 1 (auto-incremento manual)
            }
            else
                                        // Si la lista está vacía...
            {
                nuevoId = 1;            // ...el primer Id es 1
            }

            producto.Id = nuevoId;
                                        // Le asigna el Id recién calculado al producto entrante
            _productos.Add(producto);
                                        // Lo agrega a la lista (= list.append() en Python)
            return producto;
                                        // Devuelve el MISMO objeto, ya con el Id puesto,
                                        // para que quien llamó sepa qué Id le tocó
        }                               // Fin de Agregar
    }                                   // Fin de la clase
}                                        // Fin del namespace
