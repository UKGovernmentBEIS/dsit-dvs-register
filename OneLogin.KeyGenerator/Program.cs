using System.Security.Cryptography;

const string privateKeyFileName = "onelogin-private-key.pem";
const string publicKeyFileName = "onelogin-public-key.pem";

var force = args.Contains("--force", StringComparer.OrdinalIgnoreCase);
var outputArguments = args.Where(argument => !argument.Equals("--force", StringComparison.OrdinalIgnoreCase)).ToArray();

if (outputArguments.Length > 1)
{
    Console.Error.WriteLine("Usage: dotnet run --project OneLogin.KeyGenerator -- [output-directory] [--force]");
    return 1;
}

var outputDirectory = outputArguments.SingleOrDefault()
    ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".dvsregister-secrets");
var privateKeyPath = Path.Combine(outputDirectory, privateKeyFileName);
var publicKeyPath = Path.Combine(outputDirectory, publicKeyFileName);

if (!force && (File.Exists(privateKeyPath) || File.Exists(publicKeyPath)))
{
    Console.Error.WriteLine($"A key file already exists in '{outputDirectory}'. Use --force to replace the key pair.");
    return 1;
}

Directory.CreateDirectory(outputDirectory);

using var rsa = RSA.Create(2048);
File.WriteAllText(privateKeyPath, rsa.ExportPkcs8PrivateKeyPem());
File.WriteAllText(publicKeyPath, rsa.ExportSubjectPublicKeyInfoPem());

if (!OperatingSystem.IsWindows())
{
    File.SetUnixFileMode(privateKeyPath, UnixFileMode.UserRead | UnixFileMode.UserWrite);
}

Console.WriteLine("GOV.UK One Login RSA key pair generated.");
Console.WriteLine($"Private key: {privateKeyPath}");
Console.WriteLine($"Public key:  {publicKeyPath}");
Console.WriteLine("Register only the public key with GOV.UK One Login. Never share or commit the private key.");

return 0;
