// Cette classe permet de donner le nombre des fichiers locaux
// Permet de donner le nom du dernier fichier uploadé
// Permet d'écrire un fichier CSV précis
// Permet de gérer les fichiers locaux

using System;
using System.Globalization;
using BizDataRouter.Models;
using CsvHelper;
using BizDataRouter.Settings;
using System.Linq;
using BizDataRouter.Status;

namespace BizDataRouter.CsvFilesInfo;

public sealed class CsvFileInfo
{
    static string? _activePrefix;
    static string? _activeFilePath;
    private readonly PiBatch _Data;
   
    private string prefixname;
    private readonly long _maximumFileSizeBytes;
    public PipelineStatus _status;

    public class PrefixCheck
    {
        public bool WriteTitle { get; set; }

        public string CurrentPathFichier { get; set; } = "";

        public string CurrentNameFichier { get; set; } = "";

        public bool UploadToMinio { get; set; }

        public string? FileUploadMinio { get; set; }
    }

    public CsvFileInfo(
        PiBatch Data,
        AppSettings appSettings,PipelineStatus status)
    {
        _maximumFileSizeBytes =
            appSettings.Csv.MaximumFileSizeMb * 1024L * 1024L;
        _status=status;
        _Data = Data;
       

        prefixname =
            $"{Data.Template}" +   
            $"_year={Data.BatchTimestamp.Year:D4}" +
            $"_month={Data.BatchTimestamp.Month:D2}" +
            $"_day={Data.BatchTimestamp.Day:D2}";
    }

    public PrefixCheck CheckPrefixFichier()
    {
        bool _UploadToMinio = false;
        string? _FileUploadMinio = null;

        string[] fichiers =
            Directory.GetFiles(
                "data",
                $"{prefixname}*.csv"
            );

        bool _Append = true;
        bool _WriteTitle = true;

        string _CurrentPathFichier;
        string _CurrentNameFichier = "";

        /*
         * MODIFICATION :
         *
         * On garde le vrai dernier fichier et son vrai numéro.
         *
         * On ne va plus utiliser fichiers.Length pour décider
         * que le dernier fichier est part=Length-1.
         */
        string? dernierFichier = null;
        int dernierNumeroPart = -1;

        if (fichiers.Length > 0)
        {
            /*
             * MaxBy retourne le fichier dont le numéro _part=n
             * est le plus grand.
             *
             * Exemple :
             *
             * part=0  -> 0
             * part=3  -> 3
             * part=10 -> 10
             *
             * Le résultat sera donc le chemin de part=10.
             */
            dernierFichier = fichiers.MaxBy(fichier =>
            {
                string nomSansExtension =
                    Path.GetFileNameWithoutExtension(fichier);

                string numeroTexte =
                    nomSansExtension
                        .Split("_part=")
                        .Last();

                return int.TryParse(
                    numeroTexte,
                    out int numeroPart
                )
                    ? numeroPart
                    : -1;
            })!;

            /*
             * Une fois le dernier fichier trouvé,
             * on extrait son vrai numéro.
             *
             * Exemple :
             *
             * Machine_part=10.csv
             * devient :
             * dernierNumeroPart = 10
             */
            string dernierNomSansExtension =
                Path.GetFileNameWithoutExtension(
                    dernierFichier
                );

            string dernierNumeroTexte =
                dernierNomSansExtension
                    .Split("_part=")
                    .Last();

            int.TryParse(
                dernierNumeroTexte,
                out dernierNumeroPart
            );

            long FileCurrentSizeBytes =
                new FileInfo(dernierFichier).Length;

            if (FileCurrentSizeBytes >= _maximumFileSizeBytes)
            {
                _Append = false;
                _UploadToMinio = true;

                // Le dernier fichier est plein :
                // c'est lui qui doit être uploadé.
                _FileUploadMinio = dernierFichier;
            }
            else
            {
                _Append = true;
            }
        }

        if (fichiers.Length == 0)
        {
            // Aucun fichier aujourd'hui :
            // on commence toujours par part=0.
            _CurrentNameFichier =
                $"{prefixname}_part=0.csv";
        }
        else if (!_Append)
        {
            /*
             * MODIFICATION :
             *
             * L'ancien fichier est plein.
             * Le nouveau numéro est :
             *
             * dernierNumeroPart + 1
             *
             * Exemple :
             * dernier fichier = part=10
             * nouveau fichier = part=11
             */
            _CurrentNameFichier =
                $"{prefixname}_part={dernierNumeroPart + 1}.csv";
        }
        else
        {
            _WriteTitle = false;

            /*
             * MODIFICATION :
             *
             * Le dernier fichier n'est pas plein.
             * On continue directement dans le vrai dernier fichier.
             *
             * On n'utilise plus :
             * fichiers.Length - 1
             */
            _CurrentNameFichier =
                Path.GetFileName(dernierFichier!);
        }

        _CurrentPathFichier =
            Path.GetFullPath(
                Path.Combine(
                    "data",
                    _CurrentNameFichier
                )
            );

        if (_activePrefix == null)
        {
            _activePrefix = prefixname;
            _activeFilePath = _CurrentPathFichier;
        }
        else if (_activePrefix != prefixname)
        {
            _UploadToMinio = true;
            _FileUploadMinio = _activeFilePath;

            _activePrefix = prefixname;
            _activeFilePath = _CurrentPathFichier;
        }
        else
        {
            _activeFilePath = _CurrentPathFichier;
        }

        return new PrefixCheck
        {
            WriteTitle = _WriteTitle,

            CurrentNameFichier = _CurrentNameFichier,

            CurrentPathFichier = _CurrentPathFichier,

            UploadToMinio = _UploadToMinio,

            FileUploadMinio = _FileUploadMinio
        };
    }

    public string? WriteCsvFile()
    {
        PrefixCheck ResultPrefixCheck =
            CheckPrefixFichier();

        using (
            var fileStream = new FileStream(
                ResultPrefixCheck.CurrentPathFichier,
                FileMode.Append,
                FileAccess.Write,
                FileShare.Read
            )
        )
        using (
            var streamWriter =
                new StreamWriter(fileStream)
        )
        using (
            var csvWriter =
                new CsvWriter(
                    streamWriter,
                    CultureInfo.InvariantCulture
                )
        )
        {
            if (ResultPrefixCheck.WriteTitle)
            {
                csvWriter.WriteField("Element");
                csvWriter.WriteField("atelier");
                csvWriter.WriteField("atelier_quality");
                csvWriter.WriteField("pressure");
                csvWriter.WriteField("pressure_quality");
                csvWriter.WriteField("processtemp");
                csvWriter.WriteField("processtemp_quality");
                csvWriter.WriteField("randomvalues");
                csvWriter.WriteField("randomvalues_quality");
                csvWriter.WriteField("timestamp");

                csvWriter.NextRecord();
            }

            foreach (PiElement element in _Data.Elements)
            {
                csvWriter.WriteField(element.Element);

                foreach (
                    PiAttribute attribut in element.Attributes
                )
                {
                    csvWriter.WriteField(attribut.Value);
                    csvWriter.WriteField(attribut.Quality);
                }

                csvWriter.WriteField(_Data.BatchTimestamp);
                csvWriter.NextRecord();
            }
        }

        FileInfo currentFile = new(ResultPrefixCheck.CurrentPathFichier);
        _status.CurrentFileName = Path.GetFileName(ResultPrefixCheck.CurrentPathFichier);
        _status.CurrentFileSizeBytes = currentFile.Length;

        if (ResultPrefixCheck.UploadToMinio)
            return ResultPrefixCheck.FileUploadMinio;

        return null;
    }
}
