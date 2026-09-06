using CsvHelper.Configuration.Attributes;

namespace BizDataRouter.Models;

public sealed class SensorReading
{
    [Name("Element")]
    public string? Element { get; set; }

    [Name("atelier")]
    public string? Atelier { get; set; }

    [Name("atelier_quality")]
    public string? AtelierQuality { get; set; }

    [Name("pressure")]
    public double Pressure { get; set; }

    [Name("pressure_quality")]
    public string? PressureQuality { get; set; }

    [Name("processtemp")]
    public double ProcessTemp { get; set; }

    [Name("processtemp_quality")]
    public string? ProcessTempQuality { get; set; }

    [Name("randomvalues")]
    public double RandomValues { get; set; }

    [Name("randomvalues_quality")]
    public string? RandomValuesQuality { get; set; }

    [Name("timestamp")]
    public DateTime Timestamp { get; set; }
}

public sealed class DailyDataResponse
{
    public string Date { get; set; } = string.Empty;
    public int TotalFiles { get; set; }
    public int TotalRows { get; set; }
    public List<SensorReading> Readings { get; set; } = [];
}
