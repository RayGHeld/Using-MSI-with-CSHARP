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
        static string userAssignedClientId = "e16791{redacted}49d141978"; //client id for the user assigned MSI
        static string keyVaultSecretName = "Rays-{redacted}Secret"; //name for the keyvault secret
        static Uri keyVaultUri = new UriBuilder("https://rays-{redacted}.vault.azure.net/").Uri;

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();

                Get_AccessToken_With_UserAssigned_MSI();
                Get_Secret_With_UserAssigned_MSI();
                Make_GraphRequest_With_UserMSI_Token();

                Get_AccessToken_With_SystemAssigned_MSI();
                Get_Secret_With_SystemAssigned_MSI();
                Make_GraphRequest_With_SystemMSI_Token();

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
            KeyVaultSecret secret = client.GetSecret(keyVaultSecretName).Value;

            Console.WriteLine($"KeyVault Secret = {secret.Value}\n");
        }

        static void Get_Secret_With_SystemAssigned_MSI()
        {
            Console.WriteLine($"\nGetting secret with system assigned msi:");

            ManagedIdentityCredential credentail = new ManagedIdentityCredential();

            SecretClient client = new SecretClient(keyVaultUri, credentail);
            KeyVaultSecret secret = client.GetSecret(keyVaultSecretName).Value;

            Console.WriteLine($"KeyVault secret = {secret.Value}\n");
        }

        static void Get_AccessToken_With_UserAssigned_MSI()
        {
            Console.WriteLine($"Getting access token with user assigned msi:");

            ManagedIdentityCredential credential = new ManagedIdentityCredential(userAssignedClientId);

            AccessToken at = credential.GetToken(new TokenRequestContext(new string[] { "https://graph.microsoft.com" }));
            string accessToken = at.Token;

            Console.WriteLine($"Access Token = {accessToken}");

        }

        static void Get_AccessToken_With_SystemAssigned_MSI()
        {
            Console.WriteLine($"Getting access token with system assigned msi:");

            ManagedIdentityCredential credentail = new ManagedIdentityCredential();

            AccessToken at = credentail.GetToken(new TokenRequestContext(new string[] { "https://graph.microsoft.com" }));
            string accessToken = at.Token;

            Console.WriteLine($"Access token = {accessToken}");

        }

        static void Make_GraphRequest_With_UserMSI_Token()
        {
            Console.WriteLine($"Making graph request with User MSI Token:");

            ManagedIdentityCredential credential = new ManagedIdentityCredential(userAssignedClientId);
            GraphServiceClient graphClient = new GraphServiceClient(credential);

            IGraphServiceUsersCollectionPage users;

            try
            {
                users = graphClient.Users.Request().GetAsync().Result;
                Console.WriteLine($"Number of users in tenant: {users.Count}\n");
            } catch(Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

        }

        static void Make_GraphRequest_With_SystemMSI_Token()
        {
            Console.WriteLine($"Making graph request with system MSI Token:");

            ManagedIdentityCredential credential = new ManagedIdentityCredential();
            GraphServiceClient graphClient = new GraphServiceClient(credential);

            IGraphServiceUsersCollectionPage users;

            try
            {
                users = graphClient.Users.Request().GetAsync().Result;
                Console.WriteLine($"Number of users in tenant: {users.Count}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}
