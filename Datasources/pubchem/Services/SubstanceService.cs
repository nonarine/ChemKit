using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ChemKit.Datasources.pubchem.Models;

namespace ChemKit.Datasources.pubchem.Services;

public class SubstanceService
{
    private readonly PubchemAPI _pubchemApi;
    private readonly ILogger<SubstanceService> _logger;

    public SubstanceService(ILogger<SubstanceService> logger)
    {
        _pubchemApi = new PubchemAPI();
        _logger = logger;
    }

    public async Task<Substance> GetSubstanceBySidAsync(string sid)
    {
        var json = await _pubchemApi.Retrieve(PubchemAPI.Domain.Substance, PubchemAPI.SubstanceNamespace.Sid, sid, PubchemAPI.Operation.Record, PubchemAPI.OutputFormat.JSON);
        if (json == null)
        {
            _logger.LogError("No data returned from PubChem API.");
            return null;
        }

        return ParseSubstance(json);
    }

    public async Task<Substance> GetSubstanceByNameAsync(string name)
    {
        var json = await _pubchemApi.Retrieve(PubchemAPI.Domain.Substance, PubchemAPI.SubstanceNamespace.Name, name, PubchemAPI.Operation.Record, PubchemAPI.OutputFormat.JSON);
        if (json == null)
        {
            _logger.LogError("No data returned from PubChem API.");
            return null;
        }

        return ParseSubstance(json);
    }

    public async Task<List<Substance>> SearchSubstancesAsync(string searchTerm, int limit = 3)
    {
        var json = await _pubchemApi.Search("compound", searchTerm, PubchemAPI.OutputFormat.JSON, limit);
        if (json == null)
        {
            _logger.LogError("No data returned from PubChem API.");
            return null;
        }

        var ids = ParseSearchResults(json, "compound");
        var substances = new List<Substance>();

        foreach (var id in ids)
        {
            var substance = await GetSubstanceByNameAsync(id);
            if (substance != null)
            {
                substances.Add(substance);

                _logger.LogInformation($"Got substance: {substance.PrintDetails()}");
            }
        }

        return substances;
    }

    private Substance ParseSubstance(string json)
    {
        var jsonObj = JObject.Parse(json);
        var substancesJson = jsonObj["PC_Substances"].ToString();
        var substances = JsonConvert.DeserializeObject<List<Substance>>(substancesJson);
        return substances?.Count > 0 ? substances[0] : null;
    }

    private List<string> ParseSearchResults(string json, string dictionary)
    {
        var data = JsonConvert.DeserializeObject<dynamic>(json);
        var terms = new List<string>();
        foreach (var term in data.dictionary_terms[dictionary])
        {
            terms.Add((string)term);
        }
        return terms;
    }
}