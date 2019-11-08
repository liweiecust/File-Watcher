using System;

public class VisualFile
{
    public string Path { get; set; }
    public string Version { get; set; }
    public VisualFile(string path,string version="")
	{
        Path = path;
        Version = version;

    }
}
