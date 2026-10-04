namespace SistemaVentas.Data;

public static class SqlServerConnection
{
    public const string DefaultServer = "(localdb)\\MSSQLLocalDB";
    public const string DefaultDatabase = "SistemaVentasDb";

    public static string ConnectionString =>
        $"Server={DefaultServer};Database={DefaultDatabase};Trusted_Connection=True;TrustServerCertificate=True;";

    public static string GetConnectionString(
        string server = DefaultServer,
        string database = DefaultDatabase)
    {
        return $"Server={server};Database={database};Trusted_Connection=True;TrustServerCertificate=True;";
    }
}
