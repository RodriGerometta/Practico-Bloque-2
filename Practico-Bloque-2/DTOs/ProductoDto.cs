namespace Practico_Bloque_2.DTOs
{
    public class ProductoDto
    // DTO de SALIDA: lo que TU API le devuelve al cliente en JSON.
    // ¿Por qué no devolver directamente 'Producto'?
    //  1. Seguridad: si mañana 'Producto' tiene 'ProveedorId' o 'CostoInterno',
    //     no se te filtran al cliente por accidente.
    //  2. Control: podés decidir exactamente qué campos aparecen en la respuesta.
    //  3. Independencia: si cambiás la base de datos, tu API pública NO cambia.
    {
        public int Id { get; set; }
        // Identificador del producto (sí se expone, el cliente lo necesita)

        public string Nombre { get; set; } = string.Empty;
        // Nombre. string.Empty evita null.

        public decimal Precio { get; set; }
        // Precio. 'decimal' para cálculos de dinero exactos.

        public string Categoria { get; set; } = string.Empty;
        // Categoría.

        public bool Activo { get; set; }
        // Estado activo/inactivo. Este sí sale en el DTO de lectura,
        // pero NO está en CreateProductoDto, así el cliente no lo controla.
    }
}
// 📦 Flujo completo en tu app:
//    ENTRADA:  JSON del cliente  ->  CreateProductoDto  ->  Producto (guardado en la lista)
//    SALIDA:   Producto (de la lista)  ->  ProductoDto  ->  JSON al cliente
//    Los dos van y vienen en ProductoService, que hace la conversión.
