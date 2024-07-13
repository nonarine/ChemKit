using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ChemKit.Datasources.pubchem.Models;

namespace ChemKit.Datasources.pubchem.Services;

public class CompoundService
{
    private readonly PubchemAPI _pubchemApi;
    private readonly ILogger<CompoundService> _logger;

    public CompoundService(ILogger<CompoundService> logger)
    {
        _pubchemApi = new PubchemAPI();
        _logger = logger;
    }

    public async Task<Compound> GetCompoundByCidAsync(string cid)
    {
        var json = await _pubchemApi.Retrieve(PubchemAPI.Domain.Compound, PubchemAPI.CompoundNamespace.Cid, cid, PubchemAPI.Operation.Record, PubchemAPI.OutputFormat.JSON);
        if (json == null)
        {
            _logger.LogError("No data returned from PubChem API.");
            return null;
        }

        return ParseCompound(json);
    }

    public async Task<Compound> GetCompoundByNameAsync(string name)
    {
        var json = await _pubchemApi.Retrieve(PubchemAPI.Domain.Compound, PubchemAPI.CompoundNamespace.Name, name, PubchemAPI.Operation.Record, PubchemAPI.OutputFormat.JSON);

        if (json == null)
        {
            _logger.LogError("No data returned from PubChem API.");
            return null;
        }

        return ParseCompound(json);
    }

    public async Task<List<Compound>> SearchCompoundsAsync(string searchTerm, int limit = 3)
    {
        var json = await _pubchemApi.Search("compound", searchTerm, PubchemAPI.OutputFormat.JSON, limit);
        if (json == null)
        {
            _logger.LogError("No data returned from PubChem API.");
            return null;
        }

        var ids = ParseSearchResults(json, "compound");
        var compounds = new List<Compound>();

        foreach (var id in ids)
        {
            var compound = await GetCompoundByNameAsync(id);
            if (compound != null)
            {
                compounds.Add(compound);

                _logger.LogInformation($"Got compound: {compound.PrintDetails()}");
            }
        }

        return compounds;
    }

    private Compound ParseCompound(string json)
    {
        var jsonObj = JObject.Parse(json);
        var compoundsJson = jsonObj["PC_Compounds"].ToString();
        var compounds = JsonConvert.DeserializeObject<List<Compound>>(compoundsJson);
        return compounds?.Count > 0 ? compounds[0] : null;
    }
    private List<string> ParseSearchResults(string json, string dictionary)
    {
        dynamic data = JsonConvert.DeserializeObject(json);
        var terms = new List<string>();
        foreach (var term in data.dictionary_terms[dictionary])
        {
            terms.Add((string)term);
        }
        return terms;
    }
}