using Practico_Bloque_2.DTOs;
// Importa los DTOs, porque los métodos trabajan con CreateProductoDto y ProductoDto

namespace Practico_Bloque_2.Services
{
    public interface IProductoService
    // INTERFAZ del servicio = contrato de la CAPA DE LÓGICA DE NEGOCIO.
    // Está ENTRE el Controller y el Repository:
    //   Controller  ->  IProductoService  ->  IProductoRepository  ->  lista en memoria
    // Equivale a un Protocol de Python.
    {
        List<ProductoDto> ObtenerTodos();
        // Devuelve TODOS los productos, ya convertidos a DTO

        ProductoDto? ObtenerPorId(int id);
        // Devuelve UNO por Id, o null ('?') si no existe

        ProductoDto Agregar(CreateProductoDto producto);
        // Crea un producto a partir de lo que mandó el cliente.
        // Devuelve el ProductoDto ya con el Id asignado (para poder responder 201 + Location)

        bool Eliminar(int id);
        // Borra por Id. true = existía y se borró, false = no existía

        bool Actualizar(int id, CreateProductoDto producto);
        // Actualiza por Id. true = existía y se actualizó, false = no existía
        // Fíjate que recibe DOS cosas: el Id viene por la URL, los datos por el body.
    }
}
// 🧠 ¿Para qué una interfaz si solo hay UN 'ProductoService'?
//    Para poder hacer esto más adelante sin tocar el Controller:
//       - ProductoServicePremium  (con reglas de negocio distintas)
//       - ProductoServiceMock     (para tests, devuelve datos falsos)
//    El Controller solo pide 'IProductoService' y no se entera de cuál le dan.
