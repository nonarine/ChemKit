using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChemKit.Datasources.pubchem.Models;

public partial class Substance
{
    public Sid Sid { get; set; }
    public SourceClass Source { get; set; }
    public string[] Synonyms { get; set; }
    public string[] Comment { get; set; }
    public Xref[] Xref { get; set; }
    public Compound[] Compound { get; set; }

    public string PrintDetails()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}

public partial class PropValue
{
    public Slist[] Slist { get; set; }
    public string Sval { get; set; }
    public Date Date { get; set; }
}

public partial class Date
{
    public Std Std { get; set; }
}

public partial class Std
{
    public long? Year { get; set; }
    public long? Month { get; set; }
    public long? Day { get; set; }
}

public partial class Sid
{
    public long? Id { get; set; }
    public long? Version { get; set; }
}

public partial class SourceClass
{
    public Db Db { get; set; }
}

public partial class Db
{
    public string Name { get; set; }
    public SourceId SourceId { get; set; }
    public Date Date { get; set; }
}

public partial class SourceId
{
    public string Str { get; set; }
}

public partial class Xref
{
    public string Dburl { get; set; }
    public string? Rn { get; set; }
    public Uri Sburl { get; set; }
    public string Regid { get; set; }
    public long? Pmid { get; set; }
    public long? Mmdb { get; set; }
    public long? Gene { get; set; }
    public long? Mim { get; set; }
    public long? Taxonomy { get; set; }
    public string Patent { get; set; }
}

public enum Label { AutoGenerated, Conformer, Mmdb, Publication, ReferenceStandardization };

public enum Name { Date, Id, MoleculeName, Structure };

public enum Software { PubChem };

public enum SourceEnum { NcbiNlmNihGov };

public enum Slist { Est, Estradiol, SlistEstradiol };

public enum Rn { The141290020, The17916675, The50282, The57910 };
