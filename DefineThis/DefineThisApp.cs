﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DefineThis
{
    public class DefineThisApp
    {
        private const string API_BASE_URL = "https://od-api.oxforddictionaries.com/api/v1";
        private const string LANGUAGE = "en";
        private const string REGION = "GB";

        private const string APP_ID = "370838dc";
        private const string APP_KEY = "2ccbeb8ccefc1a1be7d1a77d6d373c69";

        private static HttpClient httpClient = new HttpClient();

        static DefineThisApp()
        {
            httpClient.DefaultRequestHeaders.Add("app_id", APP_ID);
            httpClient.DefaultRequestHeaders.Add("app_key", APP_KEY);
        }

        public static void Main(string[] args)
        {
            while (true)
            {
                var input = askForInput();

                if (isInputValid(input))
                {
                    string[] definitions;

                    try
                    {
                        definitions = getDefinitionAsync(input).Result;
                    }
                    catch (HttpRequestException e)
                    {
                        logError(e);
                        continue;
                    }
                    catch (Exception e)
                    {
                        logError(e);
                        continue;
                    }

                    printArray(definitions);
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("The input was not valid, check that you entered a single word.");
                    Console.WriteLine();
                }
            }
        }

        private static bool isInputValid(string input)
        {
            return !input.Contains(" ");
        }

        private static string askForInput()
        {
            Console.WriteLine("Please enter a word:");
            return Console.ReadLine();
        }

        private static async Task<string[]> getDefinitionAsync(string word)
        {
            var wordId = word.ToLower();
            var jsonResponse = await httpClient.GetStringAsync($"{API_BASE_URL}/entries/{LANGUAGE}/{wordId}");

            var response = JObject.Parse(jsonResponse);
            var definitionsJson = (JArray)response["results"][0]["lexicalEntries"][0]["entries"][0]["senses"][0]["definitions"];
            var definitions = definitionsJson.ToObject<string[]>();
            return definitions;
        }

        private static void printArray(string[] arr)
        {
            for (var i = 0; i < arr.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {arr[i]}");
            }
        }

        private static void logError(Exception e)
        {
            Console.WriteLine("An error occurred: " + e.Message);
            Console.WriteLine(e);
        }
    }
}