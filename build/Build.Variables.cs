using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Utilities.Collections;

partial class Build
{
    Target Variables => _ => _
        .Executes(() =>
        {
            EnvironmentInfo.Variables.OrderBy(x => x.Key).ForEach(x => Console.WriteLine($"{x.Key} = {x.Value}"));
        });
}