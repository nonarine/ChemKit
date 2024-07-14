using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ChemKit.Datasources.pubchem.Models;
using ChemKit;

namespace ChemKit.Datasources.pubchem.Services;

public class SubstanceService
{
    private readonly PubchemAPI _pubchemApi;
    private readonly ILogger<SubstanceService> _logger;
    private readonly LoadingBarService _loadingBarService;

    public SubstanceService(ILogger<SubstanceService> logger, LoadingBarService loadingBarService)
    {
        _loadingBarService = loadingBarService;
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
        return await Task.Run(async () =>
        {
            // Search for compounds first
            var compoundJson = await _pubchemApi.Search("compound", searchTerm, PubchemAPI.OutputFormat.JSON, limit);
            if (compoundJson == null)
            {
                _logger.LogError("No data returned from PubChem API for compounds.");
                return null;
            }

            var compoundIds = ParseSearchResults(compoundJson, "compound");
            var substanceTasks = new List<Task<Substance>>();

            foreach (var compoundId in compoundIds)
            {
                // Use each compound ID to search for corresponding substances
                substanceTasks.Add(GetSubstanceByNameAsync(compoundId));
            }

            var substances = (await _loadingBarService.WhenAll(substanceTasks)).Where(substance => substance != null).ToList();

            foreach (var substance in substances)
            {
                var serializedSubstance = JsonConvert.SerializeObject(substance, Formatting.Indented);
                _logger.LogInformation($"Got substance: {serializedSubstance}");
            }

            return substances;
        });
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