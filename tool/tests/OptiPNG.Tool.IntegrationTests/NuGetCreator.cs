﻿namespace OptiPNG.Tool.IntegrationTests;

internal class NuGetCreator
{
    public string Create(DirectoryInfo packagePath)
    {
        return
@$"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <packageSources>
    <clear />
    <add key=""Local"" value=""{packagePath.FullName}"" />
  </packageSources>
</configuration>
";
    }
}
