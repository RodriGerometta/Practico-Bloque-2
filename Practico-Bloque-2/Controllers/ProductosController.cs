using Microsoft.AspNetCore.Http;
// Trae tipos del pipeline HTTP (Results, IFormFile, StatusCodeCodes...)

using Microsoft.AspNetCore.Mvc;
// 🔑 Trae TODO lo del MVC/Controllers: [Route], [HttpGet], [HttpPost],
//    ControllerBase, IActionResult, Ok(), NotFound()...

using Practico_Bloque_2.DTOs;      // Para recibir CreateProductoDto y devolver ProductoDto
using Practico_Bloque_2.Models;    // Para la entidad Producto
using Practico_Bloque_2.Services;  // Para pedir IProductoService (la lógica de negocio)

namespace Practico_Bloque_2.Controllers
// Namespace de los Controllers. ASP.NET busca AUTOMÁTICAMENTE en los namespaces
// que empiezan con el del proyecto para encontrar controllers.
{
    [Route("api/[controller]")]
    // 📍 Define la RUTA BASE de todos los endpoints de esta clase.
    //    [controller] = un placeholder que se reemplaza por el nombre de la clase
    //    SIN la parte "Controller": ProductosController -> "Productos"
    //    Resultado: /api/Productos   (las mayúsculas da igual, ASP.NET no distingue)
    //    Y como [HttpGet("{id}")] agrega "/{id}", la ruta final es /api/Productos/{id}
    //    Equivale a: @app.router.prefix = "/api" en FastAPI

    [ApiController]
    // 🔑 Marca esta clase como API de verdad. Activa 4 comportamientos automáticos:
    //   1. Binding de JSON -> parámetros del método (sin escribir nada)
    //   2. VALIDACIÓN automática de [Required]/[Range] del DTO -> devuelve 400 solo
    //   3. Errores de modelo -> 400 con detalle de cada campo
    //   Sin esto, tus validaciones NO se ejecutarían y tendrías que hacerlo a mano.
    //   (Equivalente a la validación automática de Pydantic en FastAPI)

    public class ProductosController : ControllerBase
    // Hereda de ControllerBase = da acceso a HttpContext, StatusCode, Ok(), NotFound()...
    // NO hereda de Controller (esa es para vistas HTML, no para APIs).
    {
        private readonly IProductoService _service;
        // La lógica de negocio de la que depende este controller.
        //  private -> solo esta clase lo usa
        //  readonly -> solo se asigna en el constructor

        public ProductosController(IProductoService service)
        // 🔑 INYECCIÓN DE DEPENDENCIAS POR CONSTRUCTOR:
        //    No hacés 'new ProductoService()' a mano.
        //    El contenedor ve que necesitás un IProductoService y te lo ENTREGA
        //    (creándolo y pasándole a su vez el repositorio).
        //    Esto es EXACTAMENTE Depends() en FastAPI.
        {
            _service = service;
            // Lo guarda para usarlo en cada método
        }



        [HttpPut("{id}")]
        // 📍 Declara que este método atiende PUT /api/Productos/{id}
        //    {id} = parámetro de ruta; su valor llega al argumento 'int id' de abajo.
        public IActionResult Actualizar(int id, CreateProductoDto dto)
        //  int id              <- SALE de la URL  (/api/productos/2  -> id = 2)
        //  CreateProductoDto dto <- SALE del BODY en JSON (se arma solo gracias a [ApiController])
        //  IActionResult = "puedo devolver cualquier código HTTP" (no un dato en concreto)
        {
            var actualizado = _service.Actualizar(id, dto);
            // Le pasa a la capa de servicio los dos datos.
            // Devuelve true si existía, false si no.

            if (!actualizado)
            // '!' = NOT. Si fue false = no existe ese Id
            {
                return NotFound();
                // Devuelve HTTP 404
            }

            return NoContent();
            // Devuelve HTTP 204 (actualizado, sin body en la respuesta)
        }

        [HttpPost]
        // 📍 Declara que atiende POST /api/Productos (sin '{id}' porque es creación)
        public IActionResult Agregar(CreateProductoDto dto)
        //  Solo recibe el BODY. El Id NO se manda, lo asigna el servidor.
        {
            var creado = _service.Agregar(dto);
            // Crea el producto. Devuelve su ProductoDto YA con el Id asignado.

            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
            // 🔑 Devuelve HTTP 201 (Created), la respuesta estándar para "creé algo".
            //
            // CreatedAtAction(...) genera 3 cosas:
            //   1. Cuerpo    -> 'creado' (el JSON del producto nuevo)
            //   2. Cabecera  -> Location: /api/Productos/{id}  (URL del recurso recién creado)
            //   3. Status    -> 201
            //
            // nameof(ObtenerPorId) = "ObtenerPorId" como STRING, pero escrito de forma segura:
            //   si mañana renombrás el método, el compilador te avisa del error.
            //   (en Python sería un string a mano: "/api/productos/{id}")
            //
            // new { id = creado.Id } = objeto anónimo que rellena el {id} de esa ruta.
            //   Como 'ObtenerPorId' tiene un parámetro 'id', hay que pasarle el valor.
        }

        [HttpDelete("{id}")]
        // 📍 Declara DELETE /api/Productos/{id}
        public IActionResult Eliminar(int id)
        //  id viene de la URL
        {
            var eliminado = _service.Eliminar(id);
            // true = existía y se borró / false = no existía

            if (!eliminado)
            {
                return NotFound();
                // HTTP 404: no hay nada que borrar
            }

            return NoContent();
            // HTTP 204: borrado OK, sin body
        }

        [HttpGet("{id}")]
        // 📍 Declara GET /api/Productos/{id}  -> buscar UNO
        public IActionResult ObtenerPorId(int id)
        //  id viene de la URL
        {
            var producto = _service.ObtenerPorId(id);
            // Puede devolver null si no existe

            if (producto == null)
            // ¿No lo encontró?
            {
                return NotFound();
                // HTTP 404
            }

            return Ok(producto);
            // HTTP 200 + el ProductoDto serializado a JSON
            // Ok(...) es literalmente return Response(200, data=...) en FastAPI
        }

        [HttpGet]
        // 📍 Declara GET /api/Productos (SIN {id}) -> listar TODOS.
        //    Puede convivir con [HttpGet("{id}")] porque las rutas son distintas:
        //      /api/productos      -> este método
        //      /api/productos/1    -> ObtenerPorId
        //    ASP.NET elige el que coincida con la URL.
        public IActionResult ObtenerTodos()
        //  Sin parámetros: no recibe ni URL ni body
        {
            var productos = _service.ObtenerTodos();
            // Devuelve la lista de ProductosDto

            return Ok(productos);
            // HTTP 200 + el arreglo completo en JSON
        }
    }
}
// 📖 ORDEN DE EJECUCIÓN de una petición a /api/productos/2 (PUT):
//   1. Llega HTTP  ->  UseHttpsRedirection  ->  UseAuthorization
//   2. Routing: coincide con [Route("api/[controller]")] + [HttpPut("{id}")]  -> id = 2
//   3. [ApiController] valida CreateProductoDto  ->  si falla: 400 y ACÁ SE CORTA
//   4. El contenedor de DI arma el controller y le inyecta IProductoService
//   5. Se ejecuta Actualizar(2, dto)
//   6. El service consulta al repository y convierte DTOs
//   7. Se devuelve 204 o 404
