using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChemKit.Datasources.pubchem.Models;

public partial class Compound
{
    public PcCompoundId Id { get; set; }
    public Atoms Atoms { get; set; }
    public Bonds Bonds { get; set; }
    public Stereo[] Stereo { get; set; }
    public Coord[] Coords { get; set; }
    public long? Charge { get; set; }
    public Prop[] Props { get; set; }
    public Count Count { get; set; }

    public string PrintDetails()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}

public partial class Atoms
{
    public long[] Aid { get; set; }
    public long[] Element { get; set; }
}

public partial class Bonds
{
    public long[] Aid1 { get; set; }
    public long[] Aid2 { get; set; }
    public long[] Order { get; set; }
}

public partial class Coord
{
    public long[] Type { get; set; }
    public long[] Aid { get; set; }
    public Conformer[] Conformers { get; set; }
}

public partial class Conformer
{
    public double[] X { get; set; }
    public double[] Y { get; set; }
    public Style Style { get; set; }
}

public partial class Style
{
    public long[] Annotation { get; set; }
    public long[] Aid1 { get; set; }
    public long[] Aid2 { get; set; }
}

public partial class Count
{
    public long? HeavyAtom { get; set; }
    public long? AtomChiral { get; set; }
    public long? AtomChiralDef { get; set; }
    public long? AtomChiralUndef { get; set; }
    public long? BondChiral { get; set; }
    public long? BondChiralDef { get; set; }
    public long? BondChiralUndef { get; set; }
    public long? IsotopeAtom { get; set; }
    public long? CovalentUnit { get; set; }
    public long? Tautomers { get; set; }
}

public partial class PcCompoundId
{
    public long? Type { get; set; }
    public IdId Id { get; set; }
}

public partial class IdId
{
    public long? Cid { get; set; }
}

public partial class Prop
{
    public Urn Urn { get; set; }
    public Value Value { get; set; }
}

public partial class Urn
{
    public string Label { get; set; }
    public string Name { get; set; }
    public long? Datatype { get; set; }
    public string? Release { get; set; }
    public string Implementation { get; set; }
    public string Version { get; set; }
    public string Software { get; set; }
    public string Source { get; set; }
    public string Parameters { get; set; }
}

public partial class Value
{
    public long? Ival { get; set; }
    public double? Fval { get; set; }
    public string Binary { get; set; }
    public string Sval { get; set; }
}

public partial class Stereo
{
    public Tetrahedral Tetrahedral { get; set; }
}

public partial class Tetrahedral
{
    public long? Center { get; set; }
    public long? Above { get; set; }
    public long? Top { get; set; }
    public long? Bottom { get; set; }
    public long? Below { get; set; }
    public long? Parity { get; set; }
    public long? Type { get; set; }
}
