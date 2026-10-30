using System;                          // Tipos básicos (DateTime, Convert, Math...). REDUNDANTE: ya lo da ImplicitUsings en el .csproj
using System.Collections.Generic;      // Colecciones (List<>, Dictionary<>...). REDUNDANTE igual
using System.Text;                     // StringBuilder. REDUNDANTE igual

namespace Practico_Bloque_2.Models      // Namespace = "carpeta lógica" donde vive la clase. Evita choques de nombres.
{                                       // En C# el bloque se delimita con llaves, NO con indentación
    public class Producto               // Declara la clase. 'public' = visible desde cualquier otro proyecto
    {                                   // Inicio del cuerpo de la clase
        // Propiedades
        public int Id { get; set; }     // Identificador único. Entero de 32 bits (~±2.100 millones)
                                        // { get; set; } = auto-propiedad: get para leer, set para escribir (como @property de Python)

        public string Nombre { get; set; } = string.Empty;
                                        // Nombre del producto.
                                        // '= string.Empty' le pone un valor por defecto ("") para que NUNCA sea null.
                                        // Hace falta porque el .csproj tiene <Nullable>enable</Nullable>

        public decimal Precio { get; set; }
                                        // 💰 PRECIO: se usa 'decimal' y NO 'double' porque es exacto en base 10.
                                        // En Python usás float por costumbre, pero 0.1 + 0.2 != 0.3 en punto flotante.
                                        // 'decimal' = 128 bits, pensado para dinero.

        public string Categoria { get; set; } = string.Empty;
                                        // Categoría. Mismo truco de string.Empty para evitar null

        public bool Activo { get; set; } // Flag de activo/inactivo (true/false)

    }                                   // Fin de la clase Producto
}                                       // Fin del namespace
