using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace appDesafio
{
    public class GuardaLogs
    {
        private static bool benArchivo;
        private static bool btoConsole;
        private static bool bMensajEe;
        private static bool bAdvertencia;
        private static bool bError;
        private static bool btoDataBase;
        //private bool inicializar;

        //Constructor
        public GuardaLogs() : this(true, false, true, false, true, true)
        {
        }

        //Constructor
        public GuardaLogs(bool enArchivo, bool enConsola, bool mensaje, bool advertencia, bool error, bool EnBaseDeDatos)
        {
            benArchivo = enArchivo;
            btoConsole = enConsola;
            bMensajEe = mensaje;
            btoDataBase = EnBaseDeDatos;
            bAdvertencia = advertencia;
            bError = error;
        }

        // Metodo para registrar mensaje
        public static void LogMessage(string mensaje, bool message1, bool warning, bool error)
        {
            mensaje.Trim();
            if (mensaje == null || mensaje.Length == 0)
            {
                return;
            }
            if (!btoConsole && !benArchivo && !btoDataBase)
            {
                throw new Exception("Configuracion invalida");
            }
            if ((!bError && !message1 && !bAdvertencia) || (!message1 && !warning && !error))
            {
                throw new Exception("Debe especificar el nivel de error");
            }

            //Abre conexion
            System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings["CadenaDeConexion"]);            
            connection.Open();

            int t = -10;
            if (message1 && bMensajEe)
            {
                t = 1;
            }
            if (error && bError)
            {
                t = 2;
            }
            if (warning && bAdvertencia)
            {
                t = 3;
            }

            //para manejar excepciones posibles al insertar a la BD.
            try
            {
                //System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("Insert into Logger Values('" + mensaje + "', " + t.ToString() + ")");
                SqlCommand cm = new SqlCommand("sp_Insertar", connection);                
                cm.CommandType = CommandType.StoredProcedure;
                cm.Parameters.AddWithValue("@mensaje", mensaje);
                //ejecutamos storeprocedure
                cm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
            string l = "inicializar string";
            if
            (!System.IO.File.Exists(System.Configuration.ConfigurationManager.AppSettings["CarpetaLogs"] + "DocLog" + DateTime.Now.ToShortDateString() + ".txt"))
            {
                l = System.IO.File.ReadAllText(System.Configuration.ConfigurationManager.AppSettings["CarpetaLogs"] + "DocLog" + DateTime.Now.ToShortDateString() + ".txt");
            }
            if (error && bError)
            {
                l = l + DateTime.Now.ToShortDateString() + mensaje;
            }
            if (warning && bAdvertencia)
            {
                l = l + DateTime.Now.ToShortDateString() + mensaje;
            }
            if (message1 && bMensajEe)
            {
                l = l + DateTime.Now.ToShortDateString() + mensaje;
            }
            System.IO.File.WriteAllText(System.Configuration.ConfigurationManager.AppSettings["CarpetaLogs"] + "DocLog" + DateTime.Now.ToShortDateString() + ".txt", l);
            if (error && bError)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            if (warning && bAdvertencia)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            if (message1 && bMensajEe)
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine(DateTime.Now.ToShortDateString() + mensaje);
        }
        static void Main(string[] args)
        {
        }
    }
}