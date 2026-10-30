using Practico_Bloque_2.Models;        // Importa 'Producto', porque los métodos de abajo lo usan como tipo de dato

namespace Practico_Bloque_2.Repositories// Namespace de esta clase
{
    public interface IProductoRepository // Una INTERFAZ = un CONTRATO. Dice QUÉ se puede hacer, no CÓMO se hace.
    {                                    // Equivale a un Protocol o a una clase ABC en Python.
                                         // La 'I' inicial es convención obligatoria por estilo en .NET.
                                         // Solo tiene firmas (sin cuerpo) -> cada línea cierra con ';' 

        Producto Agregar(Producto producto);
                                         // Promete: recibir un producto, guardarlo y DEVOLVERLO con el Id ya asignado.

        List<Producto> ObtenerTodos();
                                         // Promete: devolver TODA la lista de productos.

        Producto ? ObtenerPorId(int id); // Promete: devolver el producto con ese Id, o NULL si no existe.
                                         // La '?' marca que el valor puede ser null -> obliga a quien llame a manejarlo.
                                         

        List<Producto> BuscarPorCategoria(string categoria);
                                         // Promete: devolver solo los productos de esa categoría.

        bool Eliminar(int id);           // Promete: borrar por Id.
                                         // Devuelve 'true' si existía y se borró, 'false' si no existía.

        bool Actualizar(Producto producto);
                                         // Promete: sobreescribir un producto existente.
                                         // Devuelve 'true' si existía, 'false' si no.

    }                                    // Fin de la interfaz
}                                        // Fin del namespace

// 🎯 ¿Para qué sirve esta interfaz si hoy solo hay UN implementador?
// Para Dependency Injection: mañana cambiás 'ProductoRepositoryInMemory' por uno con
// SQL Server (ProductoRepositoryEF) y NO tocás ni una línea de los Controllers.
// Es el mismo concepto que Depends() en FastAPI.
