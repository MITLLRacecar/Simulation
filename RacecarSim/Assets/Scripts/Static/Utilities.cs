using System;
using System.IO;
using System.Security.Cryptography;

/// <summary>
/// Contains general helpful functions.
/// </summary>
public static class Utilities
{
    /// <summary>
    /// Encrypts a plaintext message.
    /// </summary>
    /// <param name="message">The message to encrypt.</param>
    /// <returns>The encrypted message, represented as a string of hex characters.</returns>
    public static string Encrypt(string message)
    {
        string output = string.Empty;
        byte[] messageBytes = new byte[16 * ((message.Length + 15) / 16)];
        for (int i = 0; i < message.Length; i++)
        {
            messageBytes[i] = (byte)message[i];
        }

        using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
        {
            aes.Key = Utilities.key;
            aes.IV = new byte[aes.BlockSize / 8];

            for (int i = 0; i < messageBytes.Length; i += 16)
            {
                ICryptoTransform transform = aes.CreateEncryptor();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(messageBytes, i, 16);
                        byte[] ciphertext = memoryStream.ToArray();
                        output += BitConverter.ToString(ciphertext).Replace("-", "");
                    }
                }
            }
        }

        return output;
    }
    
    // Security!
    private static readonly byte[] key = { 0x2b, 0x7e, 0x15, 0x16, 0x28, 0xae, 0xd2, 0xa6, 0xab, 0xf7, 0x15, 0x88, 0x09, 0xcf, 0x4f, 0x3c };
}
