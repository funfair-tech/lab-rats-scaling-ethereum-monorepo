namespace FunFair.Labs.ScalingEthereum.Authentication.Config
{
    public interface IJwtConfig
    {
        byte[]? EcDsaPublicKey { get; }

        string Audience { get; }

        string Issuer { get; }
    }
}