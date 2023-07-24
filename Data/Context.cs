namespace DBConn_ASPNETCORE.Data
{
    public class Context
    {
        public string Conexion { get; }

        public Context(string valor)
        {
            Conexion = valor;
        }
    }
}
