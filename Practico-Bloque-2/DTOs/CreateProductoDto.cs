using System.ComponentModel.DataAnnotations;
// Trae las anotaciones de VALIDACIÓN: [Required], [StringLength], [Range]...
// Es el equivalente a los validators de Pydantic.

namespace Practico_Bloque_2.DTOs
{
    public class CreateProductoDto
    // DTO = Data Transfer Object. Es lo que el CLIENTE te manda por JSON en un POST/PUT.
    // Se separa del Modelo 'Producto' a propósito:
    //   - El DTO NO trae 'Id' (lo asigna el servidor)
    //   - El DTO NO trae 'Activo' (lo decide el servidor)
    // Esto evita que alguien te mande un JSON con id: -1 y te rompa la base.
    // Equivale a un BaseModel de Pydantic.
    {
        [Required]
        // El campo es OBLIGATORIO. Si falta o viene null/vacío -> HTTP 400.
        [StringLength(100, MinimumLength = 3)]
        // El texto debe tener entre 3 y 100 caracteres.
        // Si mandás "ab" (2 letras) -> HTTP 400 con ese mensaje.
        public string Nombre { get; set; } = string.Empty;
        // Nombre del producto. '= string.Empty' lo inicializa en "" para que nunca sea null.

        [Required]
        // Obligatorio también.
        [Range(0.01, double.MaxValue)]
        // El precio debe ser >= 0.01. Si mandás 0 o un negativo -> HTTP 400.
        // (por eso tu POST de prueba con "precio": 0 dio error)
        public decimal Precio { get; set; }
        // Precio. 'decimal' = tipo exacto para dinero (no 'double').

        [Required]
        // Obligatorio. Si mandás "" -> HTTP 400 "The Categoria field is required".
        public string Categoria { get; set; } = string.Empty;
        // Categoría del producto.
    }
}
// 💡 Todo esto se aplica SOLAMENTE porque tu Controller tiene [ApiController]:
//    ASP.NET ejecuta la validación automáticamente ANTES de llegar a tu método
//    y devuelve el 400 con los mensajes sin que escribas un solo 'if'.
//    En FastAPI harías: if not producto.nombre: raise HTTPException(400, ...)
