using ChemKit.Datasources.pubchem.Models;

namespace ChemKit.Datasources.pubchem;

class PubchemAPI
{
    public enum Domain
    {
        Substance, Compound, Assay, Gene, Protein,
        Pathway, Taxonomy, Cell, Sources, SourceTable,
        Conformers, Annotations, Classification, Standardize, PeriodicTable
    }

    public enum CompoundNamespace
    {
        Cid, Name, Smiles, Inchi, Sdf,
        InchiKey, Formula, Substructure, Superstructure, Similarity,
        Identity, FastIdentity, FastSimilarity2D, FastSimilarity3D, FastSubstructure,
        FastSuperstructure, FastFormula, Xref, ListKey
    }

    public enum Xref
    {
        RegistryID, RN, PubMedID, MMDBID, ProteinGI,
        NucleotideGI, TaxonomyID, MIMID, GeneID, ProbeID,
        PatentID
    }

    public enum SubstanceNamespace
    {
        Sid, SourceId, SourceAll, Name, Xref,
        ListKey
    }

    public enum AssayNamespace
    {
        Aid, ListKey, Type, SourceAll, Target,
        Activity
    }

    public enum AssayType
    {
        All, Confirmatory, DoseResponse, OnHold, Panel,
        RNAi, Screening, Summary, CellBased, Biochemical,
        InVivo, InVitro, ActiveConcentrationSpecified
    }

    public enum AssayTarget
    {
        Gi, ProteinName, GeneId, GeneSymbol, Accession
    }

    public enum GeneNamespace
    {
        GeneId, GeneSymbol, Synonym
    }

    public enum ProteinNamespace
    {
        Accession, Gi, Synonym
    }

    public enum PathwayNamespace
    {
        PwAcc
    }

    public enum TaxonomyNamespace
    {
        TaxId, Synonym
    }

    public enum CellNamespace
    {
        CellAcc, Synonym
    }

    public enum Operation
    {
        Record, Property, Synonyms, Sids, Cids,
        Aids, AssaySummary, Classification, Xrefs, Description,
        Conformers, Concise, Targets, DoseResponse, Summary,
        GeneIds, Accessions, PwAccs
    }

    public enum OutputFormat
    {
        XML, ASNT, ASNB, JSON, JSONP,
        SDF, CSV, PNG, TXT
    }

    public PubchemAPI()
    {
    }

    private string BuildPubChemUrl(string domain, string namespaceStr, string identifier, string operation, string outputFormat, string callback = null)
    {
        var baseUrl = "https://pubchem.ncbi.nlm.nih.gov/rest/pug";
        var url = $"{baseUrl}/{domain.ToLower()}/{namespaceStr.ToLower()}/{identifier}/{operation.ToLower()}/{outputFormat.ToUpper()}";

        if (outputFormat.Equals("JSONP", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(callback))
        {
            url += $"?callback={callback}";
        }

        return url;
    }
    private string BuildSearchUrl(string dictionary, string searchTerm, string outputFormat, int limit = 10, string callback = null)
    {
        var baseUrl = "https://pubchem.ncbi.nlm.nih.gov/rest/autocomplete";
        var url = $"{baseUrl}/{dictionary.ToLower()}/{searchTerm.ToLower()}/{outputFormat.ToLower()}?limit={limit}";

        if (outputFormat.Equals("jsonp", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(callback))
        {
            url += $"&callback={callback}";
        }

        return url;
    }
    public async Task<string> SendHttpRequestAsync(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                Console.WriteLine("Retrieving URL: {0}", url);
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught! Message : {0} ", e.Message);
                return null;
            }
        }
    }
    public async Task<string> Retrieve<TNamespace>(Domain domain, TNamespace namespaceValue, string identifier, Operation operation, OutputFormat outputFormat, string callback = null)
       where TNamespace : Enum
    {
        var url = BuildPubChemUrl(domain.ToString(), namespaceValue.ToString(), identifier, operation.ToString(), outputFormat.ToString(), callback);
        return await SendHttpRequestAsync(url);
    }

    public async Task<string> Search(string dictionary, string searchTerm, OutputFormat outputFormat, int limit = 10, string callback = null)
    {
        var url = BuildSearchUrl(dictionary, searchTerm, outputFormat.ToString(), limit, callback);
        return await SendHttpRequestAsync(url);
    }
}