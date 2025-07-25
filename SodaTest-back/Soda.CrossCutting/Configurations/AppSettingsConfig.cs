namespace Soda.CrossCutting.Configurations;

public class AppSettingsConfig
{
    public PostgresConfig PostgresConfig { get; set; }
    public Auth0Config Auth0Config { get; set; }
}

public class PostgresConfig
{
    public string Connection { get; set; }
}

public class Auth0Config
{
    public string Domain { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Audience { get; set; }
    public string Connection { get; set; }
}