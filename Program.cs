namespace segundo_parcial
{
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;

 //El código implementa un sistema experto que utiliza
 //encadenamiento hacia adelante para recomendar cultivos según condiciones ambientales.
 //Carga hechos y reglas desde una base de datos SQLite, donde las reglas tienen condiciones (como tipo de suelo y clima) y conclusiones (cultivos recomendados).
 //El algoritmo verifica si los hechos conocidos cumplen las condiciones de alguna regla, y si es así, añade la conclusión como un nuevo hecho.
 //Este proceso se repite hasta que no se puedan derivar más hechos, y luego se muestran todos los hechos, incluidos los nuevos cultivos sugeridos.


    class SistemaExpertoCultivo
    {
        private List<string> hechos = new List<string>();
        private List<Regla> reglas = new List<Regla>();

        public SistemaExpertoCultivo()
        {
            CargarBaseDeDatos();
        }

        // Cargar los hechos y reglas desde la base de datos
        private void CargarBaseDeDatos()
        {
            using (SQLiteConnection conexion = new SQLiteConnection("Data Source=sistema_cultivo.db"))
            {
                conexion.Open();

                // Cargar hechos
                string sqlHechos = "SELECT descripcion FROM Hechos";
                using (SQLiteCommand cmd = new SQLiteCommand(sqlHechos, conexion))
                {
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        hechos.Add(reader.GetString(0));
                    }
                }

                // Cargar reglas
                string sqlReglas = "SELECT condicion, conclusion FROM Reglas";
                using (SQLiteCommand cmd = new SQLiteCommand(sqlReglas, conexion))
                {
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        reglas.Add(new Regla(reader.GetString(0), reader.GetString(1)));
                    }
                }
            }
        }

        // Ejecutar el encadenamiento hacia adelante
        public void EjecutarEncadenamientoAdelante()
        {
            bool nuevoHechoDerivado;
            do
            {
                nuevoHechoDerivado = false;
                foreach (var regla in reglas)
                {
                    if (hechos.Contains(regla.Condicion) && !hechos.Contains(regla.Conclusion))
                    {
                        Console.WriteLine($"Derivando nuevo hecho: {regla.Conclusion}");
                        hechos.Add(regla.Conclusion);
                        nuevoHechoDerivado = true;
                    }
                }
            } while (nuevoHechoDerivado);
        }

        // Mostrar los hechos actuales
        public void MostrarHechos()
        {
            Console.WriteLine("Hechos conocidos:");
            foreach (var hecho in hechos)
            {
                Console.WriteLine($"- {hecho}");
            }
        }
    }

    class Regla
    {
        public string Condicion { get; }
        public string Conclusion { get; }

        public Regla(string condicion, string conclusion)
        {
            Condicion = condicion;
            Conclusion = conclusion;
        }
    }

    // Ejemplo de uso
    class Program
    {
        static void Main()
        {
            SistemaExpertoCultivo sistema = new SistemaExpertoCultivo();
            sistema.MostrarHechos();

            Console.WriteLine("\nEjecutando encadenamiento hacia adelante...");
            sistema.EjecutarEncadenamientoAdelante();

            Console.WriteLine("\nHechos después del encadenamiento:");
            sistema.MostrarHechos();
        }
    }


}
