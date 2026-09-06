using System.Globalization;
using BizDataRouter.Models;
using BizDataRouter.Settings;
using CsvHelper;
using Minio;
using Minio.DataModel.Args;

namespace BizDataRouter.Services;

public sealed class DataQueryService
{
    private readonly IMinioClient _minioClient;
    private readonly AppSettings _settings;

    public DataQueryService(IMinioClient minioClient, AppSettings settings)
    {
        _minioClient = minioClient;
        _settings = settings;
    }

    public async Task<DailyDataResponse> GetDataForDayAsync(DateOnly date, CancellationToken cancellationToken)
    {
        string dateMarker = $"_year={date.Year}_month={date.Month:D2}_day={date.Day:D2}";
        var response = new DailyDataResponse { Date = date.ToString("yyyy-MM-dd") };
        var files = new List<string>();

        await foreach (var item in _minioClient.ListObjectsEnumAsync(
            new ListObjectsArgs()
                .WithBucket(_settings.Minio.BucketName),
            cancellationToken))
        {
            if (item.Key.Contains(dateMarker, StringComparison.Ordinal))
                files.Add(item.Key);
        }

        response.TotalFiles = files.Count;

        foreach (string fileName in files)
        {
            using var memoryStream = new MemoryStream();
            await _minioClient.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(_settings.Minio.BucketName)
                    .WithObject(fileName)
                    .WithCallbackStream(stream => stream.CopyTo(memoryStream)),
                cancellationToken);

            memoryStream.Position = 0;
            using var reader = new StreamReader(memoryStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            response.Readings.AddRange(csv.GetRecords<SensorReading>().ToList());
        }

        response.TotalRows = response.Readings.Count;
        return response;
    }
}
