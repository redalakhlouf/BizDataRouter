using System;

namespace BizDataRouter.Settings;

public sealed class AppSettings
{
    public PiApiSettings PiApi {get;set;}=new();
    public CsvSettings Csv {get;set;}=new();
    public MinioSettings Minio {get;set;}=new();
}
public sealed class PiApiSettings
{
    public string Url {get;set;}="";
}
public sealed class CsvSettings
{
    public long MaximumFileSizeMb { get; set; } = 50;
}
public sealed class MinioSettings
{
    public string Endpoint { get; set; } = "";
    public string AccessKey { get; set; } = "";
    public string SecretKey { get; set; } = "";
    public string BucketName { get; set; } = "pi-system-data";
    public bool UseSsl { get; set; }
}
