using Practico_Bloque_2.Repositories;   // Importa el repositorio y su interfaz (IProductoRepository, ProductoRepositoryInMemory)
using Practico_Bloque_2.Services;        // Importa los servicios (IProductoService, ProductoService)

IProductoRepository repo = new ProductoRepositoryInMemory();
// ⚠️ CÓDIGO MUERTO: esta variable ya NO se usa en ninguna parte.
//    (antes servía para las pruebas en consola que borraste)
//    Se puede eliminar esta línea sin que pase nada.

var builder = WebApplication.CreateBuilder(args);
// 🔑 FASE 1: crea el BUILDER (objeto de configuración de la app web).
// Configura logging, el contenedor de DEPENDENCIAS, el entorno, etc.
// Equivale a: app = FastAPI() en su parte de configuración.

// Add services to the container.   // (comentario de la plantilla: "agregá servicios al contenedor")

builder.Services.AddControllers();
// 🔑 Registra en el contenedor de DI todo lo necesario para CONTROLADORES:
// enrutamiento, serialización a JSON, binding de modelos...
// Equivale a registrar tus routers en FastAPI.
// ⚠️ No confundir con MapControllers() de más abajo:
//    ESTE registra servicios, AQUEL expone las rutas.

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Registra el servicio que GENERA el documento OpenAPI (la spec de tu API).
// Es el "Swagger" moderno de ASP.NET Core.

builder.Services.AddScoped<IProductoRepository, ProductoRepositoryInMemory>();
// 🔑 INYECCIÓN DE DEPENDENCIAS: cada vez que un Controller pida un IProductoRepository,
//    el contenedor le entrega UNA INSTANCIA NUEVA de ProductoRepositoryInMemory.
//    'Scoped' = una instancia POR CADA PETICIÓN HTTP.
//    ⚠️ BUG CONOCIDO: como es Scoped, los datos NO persisten entre peticiones
//       (el POST crea en un repo que se tira a la basura al terminar la request).
//       Debería ser: builder.Services.AddSingleton<...>()  para que la lista viva toda la app.
//    Equivale a: app.dependency_overrides / Depends() en FastAPI.

builder.Services.AddScoped<IProductoService, ProductoService>();
// 🔑 Registra el SERVICIO. 'Scoped' acá SÍ está bien: el servicio es liviano y sin estado propio.
//    Al ser Scoped, ProductService recibe del contenedor el repo que le toca en esa petición.
//    ProductoService le pide el repositorio por la INTERFAZ (no a mano con 'new').
//    Cadena completa: Controller -> IProductoService -> IProductoRepository -> List<Producto>

var app = builder.Build();
// 🔑 FASE 2: CONGELA la configuración y crea la app.
// A partir de acá NO se pueden registrar más servicios (lanza excepción).
// Es el punto de no retorno.

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
// ¿El entorno es Development?
// Viene de ASPNETCORE_ENVIRONMENT en Properties/launchSettings.json
{
    app.MapOpenApi();
    // Si sí: EXPONE el documento en /openapi/v1.json
    // Equivale a: if settings.ENV == "development": app.docs_url = "/docs"
}

app.UseHttpsRedirection();
// MIDDLEWARE: redirige toda petición HTTP -> HTTPS (respuesta 307).
// Se inserta en la "pipeline" que recorre cada request.

app.UseAuthorization();
// MIDDLEWARE de autorización. Hoy no hay policy definida => deja pasar todo.
// Necesitará AddAuthentication() para servir de algo.

app.MapControllers();
// 🔑 Mapea las rutas de TODOS los Controllers encontrados por reflexión.
// Sin esta línea, tu ProductosController NO se expondría.
// Los atributos [Route("api/[controller]")] + [HttpGet] de cada método definen las rutas.

app.MapGet("/", () =>
// 🔑 ENDPOINT en la RAÍZ "/" (el que hiciste recién).
//    Equivale a: @app.get("/")
//    Los paréntesis vacíos '()' = la petición NO trae parámetros.
//    '=>' abre la lambda (la función que maneja la request).
{
    var info = new
    // Crea un OBJETO ANÓNIMO: una estructura de datos sin declarar una clase previa.
    // C# infiere sus propiedades. Equivale a un dict en Python.
    // (Si fuera Python: info = {"Mensaje": ..., "Endpoints": [...]})
    {
        Mensaje = "API Práctico Bloque 2 funcionando ✅",
        // Propiedad simple: un string.

        Endpoints = new[]
        // 'new[]' crea un ARREGLO (lista) a partir de lo que viene entre llaves.
        // Equivale a una lista de dicts en Python.
        {
            new { Metodo = "GET",    Ruta = "/api/productos",      Descripcion = "Listar todos" },
            // Elemento 1 del arreglo: objeto anónimo con 3 propiedades.
            // IMPORTANTE: todos los elementos deben tener los MISMAS propiedades
            // y en el MISMO orden (si no, no compila, porque 'new[]' infiere un solo tipo).

            new { Metodo = "GET",    Ruta = "/api/productos/{id}", Descripcion = "Obtener uno por id" },
            // Elemento 2. '{id}' es un PARÁMETRO DE RUTA: lo que vaya ahí llega como argumento.

            new { Metodo = "POST",   Ruta = "/api/productos",      Descripcion = "Crear producto" },
            // Elemento 3. POST = crear.

            new { Metodo = "PUT",    Ruta = "/api/productos/{id}", Descripcion = "Actualizar producto" },
            // Elemento 4. PUT = actualizar.

            new { Metodo = "DELETE", Ruta = "/api/productos/{id}", Descripcion = "Eliminar producto" }
            // Elemento 5. DELETE = borrar. (el último NO lleva coma)
        }
    };

    return Results.Ok(info);
    // Devuelve HTTP 200 con el objeto 'info' SERIALIZADO A JSON automáticamente.
    // Equivale a: return info  en un endpoint de FastAPI
    // Otras opciones: Results.Created(...), Results.NotFound(), Results.NoContent()
});
// Cierra: '}' cierra la lambda, ')' cierra la llamada a MapGet, ';' termina la instrucción.

app.Run();
// 🔑 ARRANCA el servidor y BLOQUEA el hilo principal hasta Ctrl+C.
// Todo lo anterior era PREPARACIÓN; esto es EJECUCIÓN.
// Equivale a: uvicorn.run(app, host="0.0.0.0", port=8000)
