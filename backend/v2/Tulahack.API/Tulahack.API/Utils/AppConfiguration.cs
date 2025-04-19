namespace Tulahack.API.Utils;

public class KeycloakPolicyEnforcerConfiguration
{
    public const string PolicyEnforcer = "policy-enforcer";

    public Dictionary<string, string> Credentials { get; set; }
}

public class KeycloakCredentialsConfiguration
{
    public const string Credentials = "credentials";

    public string Secret { get; set; }
}

public class KeycloakConfiguration
{
    public const string Keycloak = "Keycloak";

    public string Realm { get; set; }
    public string AuthServerUrl { get; set; }
    public string SslRequired { get; set; }
    public string Resource { get; set; }
    public bool VerifyTokenAudience { get; set; }
    public KeycloakCredentialsConfiguration Credentials { get; set; }
    public int ConfidentialPort { get; set; }
    public KeycloakPolicyEnforcerConfiguration PolicyEnforcer { get; set; }
}

public class CdnConfiguration
{
    public const string Cdn = "Cdn";

    public string CdnUrl { get; set; }
}

public class WebConfiguration
{
    public const string Web = "Web";

    public string WebAppBaseUrl { get; set; }
}

public class AppConfiguration
{
    public KeycloakConfiguration? KeycloakConfiguration { get; set; }
    public CdnConfiguration? CdnConfiguration { get; set; }
    public WebConfiguration? WebConfiguration { get; set; }
}
