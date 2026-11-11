using Microsoft.EntityFrameworkCore;
// 🔑 IMPORTANTE: este using es el que te daba error en rojo.
//    Trae DbContext, DbSet, ModelBuilder, ToTable, HasKey...
//    Recién existe porque instalamos los paquetes:
//      - Npgsql.EntityFrameworkCore.PostgreSQL
//      - Microsoft.EntityFrameworkCore.Design

using Practico_Bloque_2.Models;
// Trae la clase 'Producto', que es la entidad que vamos a mapear a una tabla

namespace Practico_Bloque_2.Data
{
    // 🧠 Equivale a tu 'Base' de SQLAlchemy:
    //      class Base(DeclarativeBase): ...
    //    y al 'engine' + 'SessionLocal' juntos.
    //    Su trabajo: describir CÓMO se mapean tus clases a las tablas de la base.

    public class AppDbContext : DbContext
    // 'AppDbContext' HEREDA de 'DbContext'.
    // DbContext = la clase base de EF Core. Nos da:
    //   - la conexión con la base
    //   - el seguimiento de entidades (tracking)
    //   - SaveChanges(), Set<T>(), Find()...
    // En Python sería: class AppDbContext(Base)  +  SessionLocal
    {
        // 🔑 CONSTRUCTOR con inyección de dependencias.
        //    Program.cs registra el DbContext con AddDbContext(...) y le pasa estas opciones.
        //    DbContextOptions<AppDbContext> = la configuración:
        //      qué provider usar, el connection string, etc.
        //    Equivale a: Session = sessionmaker(bind=engine)
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
            // 'base(options)' le pasa las opciones al constructor de la clase padre (DbContext).
            // Sin esto, DbContext no sabría a qué base conectarse.
        {
        }

        // ─── LAS TABLAS ───────────────────────────────────────────────
        // Cada DbSet<T> = una TABLA en la base.
        // En Python: class Producto(Base): __tablename__ = "productos"
        public DbSet<Producto> Productos => Set<Producto>();
        // 'Productos' es una PROPIEDAD que devuelve un DbSet<Producto>.
        //   =>  es la forma corta de: get { return Set<Producto>(); }
        // El nombre 'Productos' es el que vas a usar en las queries:
        //   _context.Productos.Where(...)   =   SELECT * FROM productos WHERE ...
        // El nombre de la TABLA real lo definimos abajo con ToTable("productos").

        // ─── CONFIGURACIÓN DEL MAPEO ──────────────────────────────────
        // Se ejecuta UNA sola vez, cuando EF arma el modelo.
        // Equivale a los __table_args__ / Column(...) de SQLAlchemy.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        // 'protected override' = estamos REEMPLAZANDO el método que trae DbContext.
        //    Se llama automáticamente por EF al iniciar.
        //    'ModelBuilder modelBuilder' = el objeto con el que declarás las reglas.
        {
            modelBuilder.Entity<Producto>(e =>
            // Le decimos CÓMO se mapea la clase 'Producto'.
            // 'e' = el Builder de ESTA entidad (para no repetir modelBuilder.Entity<Producto>)
            {
                // ── TABLA ──
                e.ToTable("productos");
                // Nombre real de la tabla en PostgreSQL: 'productos'
                // (sin esto, EF usaría 'Productos' con mayúscula)

                // ── CLAVE PRIMARIA ──
                e.HasKey(p => p.Id);
                // Define que 'Id' es la PRIMARY KEY.
                // Equivale a: id = Column(Integer, primary_key=True)

                e.Property(p => p.Id)
                  .HasColumnName("id")        // nombre de la columna: 'id'
                  .UseIdentityAlwaysColumn(); // SECUENCIA: se genera sola (id, id+1, id+2...)
                                              // Equivale a SERIAL / GENERATED ALWAYS AS IDENTITY

                // ── NOMBRE ──
                e.Property(p => p.Nombre)
                  .HasColumnName("nombre")    // columna 'nombre'
                  .HasMaxLength(100)           // varchar(100) → límite de 100 caracteres
                  .IsRequired();               // NOT NULL → no puede ser vacío

                // ── PRECIO ──
                e.Property(p => p.Precio)
                  .HasColumnName("precio")    // columna 'precio'
                  .HasPrecision(18, 2);        // numeric(18,2)
                                                // 18 dígitos en total, 2 después de la coma
                                                // Ej: 9999999999999999.99
                                                // Es el tipo CORRECTO para dinero (no float/double)

                // ── CATEGORÍA ──
                e.Property(p => p.Categoria)
                  .HasColumnName("categoria")  // columna 'categoria'
                  .HasMaxLength(100)           // varchar(100)
                  .IsRequired();               // NOT NULL

                // ── ACTIVO ──
                e.Property(p => p.Activo)
                  .HasColumnName("activo")     // columna 'activo'
                  .HasDefaultValue(true);      // DEFAULT true
                                                // Si no mandás valor, nace activado
            });
        }
    }
}
// ─── CÓMO SE RELACIONA CON EL RESTO ─────────────────────────────────
//
//  Program.cs
//     builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(cadena))
//         │
//         ▼  (el contenedor crea UNA instancia por petición)
//  Repositories/ProductoRepository(AppDbContext context)
//         │
//         ▼
//  _context.Productos.ToList()      → SELECT * FROM productos
//  _context.Productos.Add(x)        → INSERT
//  _context.SaveChanges()           → ejecuta lo anterior en la base
//
// ⚠️ SaveChanges() es OBLIGATORIO: sin él, los cambios solo viven en memoria.
//    Equivale a db.commit() en SQLAlchemy.
