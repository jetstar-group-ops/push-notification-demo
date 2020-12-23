using System;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CorePush.Apple;
using static PushApple.AppleNotification;


namespace PushApple
{
    class Program
    {  
        static HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            var path = args.Length > 0 ? args[0] : Path.Combine(Directory.GetCurrentDirectory(), "apple.json");
            var json = JObject.Parse(File.ReadAllText(path));

            var p8privateKey = json["p8privateKey"].ToString();
            var p8privateKeyId = json["p8privateKeyId"].ToString();
            var teamId = json["teamId"].ToString();
            var appBundleIdentifier = json["appBundleIdentifier"].ToString();
            var server = json["server"].ToString().Equals("Development") ? CorePush.Apple.ApnServerType.Development : CorePush.Apple.ApnServerType.Production;
            
            var deviceToken = "8b3bc9c04453e186695be4a56c55368a8e6a4c407c41ec191353bb5da409e99f";
            var notification = json["payload"].ToObject<AppleNotification>();
            
            var apnSettings =  new ApnSettings();
            apnSettings.P8PrivateKey = p8privateKey;
            apnSettings.P8PrivateKeyId = p8privateKeyId;
            apnSettings.TeamId = teamId;
            apnSettings.ServerType = server;
            apnSettings.AppBundleIdentifier = appBundleIdentifier;
            
            var apn = new ApnSender(apnSettings, client);
            var result = await apn.SendAsync(notification, deviceToken);
            Console.WriteLine($"Result: {JsonConvert.SerializeObject(result)}");
        }
    }

    public class AppleNotification {

        [JsonProperty ("aps")]

        public ApsPayload Aps { get; set; }

        public class ApsPayload {
            [JsonProperty ("alert")]
            public string AlertBody { get; set; }
        }

    }
}
