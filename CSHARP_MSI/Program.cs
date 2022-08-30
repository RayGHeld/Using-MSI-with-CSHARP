﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Graph;


namespace CSHARP_MSI
{
    internal class Program
    {
        static string userAssignedClientId = "{client id}"; //"e167919d-4e6a-4149-96e2-90c49d141978"; //client id for the user assigned MSI
        static string keyVaultSecretName = "{keyvault secret name}"; //name for the keyvault secret
        static Uri keyVaultUri = new UriBuilder("https://rays-keyvault.vault.azure.net/").Uri;

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();

                Get_AccessToken_With_UserAssigned_MSI();
                Get_Secret_With_UserAssigned_MSI();

                Get_AccessToken_With_SystemAssigned_MSI();
                Get_Secret_With_SystemAssigned_MSI();

                Console.WriteLine("Press Enter to try again or any other key to exit");
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.Key != ConsoleKey.Enter)
                {
                    return;
                }
            }

        }

        static void Get_Secret_With_UserAssigned_MSI()
        {
            Console.WriteLine($"\nGetting secret with user assigned msi:");

            ManagedIdentityCredential credential = new ManagedIdentityCredential(userAssignedClientId);

            SecretClient client = new SecretClient(keyVaultUri, credential);
            try
            {
                KeyVaultSecret secret = client.GetSecret(keyVaultSecretName).Value;
                Console.WriteLine($"KeyVault Secret = {secret.Value}\n");
            } catch (Exception ex)
            {
                Console.WriteLine($"Error getting secret: {ex.Message}");
            }           
        }

        static void Get_Secret_With_SystemAssigned_MSI()
        {
            Console.WriteLine($"\nGetting secret with system assigned msi:");

            ManagedIdentityCredential credentail = new ManagedIdentityCredential();

            SecretClient client = new SecretClient(keyVaultUri, credentail);
            try
            {
                KeyVaultSecret secret = client.GetSecret(keyVaultSecretName).Value;
                Console.WriteLine($"KeyVault Secret = {secret.Value}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting secret: {ex.Message}");
            }
        }

        static void Get_AccessToken_With_UserAssigned_MSI()
        {
            Console.WriteLine($"Getting access token with user assigned msi:");

            ManagedIdentityCredential credential = new ManagedIdentityCredential(userAssignedClientId);

            AccessToken at = credential.GetToken(new TokenRequestContext(new string[] { "https://database.windows.net" }));
            string accessToken = at.Token;

            Console.WriteLine($"Access Token = {accessToken}");

        }

        static void Get_AccessToken_With_SystemAssigned_MSI()
        {
            Console.WriteLine($"Getting access token with system assigned msi:");

            ManagedIdentityCredential credentail = new ManagedIdentityCredential();

            AccessToken at = credentail.GetToken(new TokenRequestContext(new string[] { "https://database.windows.net" }));
            string accessToken = at.Token;

            Console.WriteLine($"Access token = {accessToken}");

        }

    }
}
