using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.Utilities.Collections;

partial class Build
{
    Target Variables => _ => _
        .Executes(() =>
        {
            EnvironmentInfo.Variables.OrderBy(x => x.Key).ForEach(x => Console.WriteLine($"{x.Key} = {x.Value}"));

            if (Host is GitHubActions actions)
            {
                var actionsGitHubEventPath = (AbsolutePath)actions.GitHubEventPath;
                var readOnlyCollection = actionsGitHubEventPath.Parent.GlobFiles("**/*.*");
                foreach (var absolutePath in readOnlyCollection)
                {
                    Console.WriteLine("==========");
                    Console.WriteLine(absolutePath);
                    Console.WriteLine(File.ReadAllText(absolutePath));
                }

                var content = File.ReadAllText(actions.GitHubEventPath);
                var event2 = JsonConvert.DeserializeObject<JObject>(content);
                Console.WriteLine(event2["number"]?.Value<string>());

                var contextContent = EnvironmentInfo.GetVariable<string>("GITHUB_CONTEXT");
                var context = JsonConvert.DeserializeObject<JObject>(contextContent);
                var token = context["token"]?.Value<string>();
                Console.WriteLine(token);
                Console.WriteLine(context["run_attempt"]?.Value<string>());


                var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("nuke-build");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes($":{token}")));
                client.BaseAddress = new Uri("https://api.github.com/");
                var content2 = new StringContent(JsonConvert.SerializeObject(new
                {
                    body = "foo"
                }));
                var requestUri = $"repos/{GitHubActions.Instance.GitHubRepository}/issues/2/comments";
                Console.WriteLine(requestUri);
                var response = client.PostAsync(requestUri, content2).GetAwaiter().GetResult();
                Console.WriteLine(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            }
        });
}