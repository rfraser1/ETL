using System;
using System.Collections.Generic;

namespace ETL.DBModels;

public partial class Rainfall
{
    public int Id { get; set; }

    public string session { get; set; } = null!;

    public decimal xref { get; set; }

    public decimal yref { get; set; }

    public decimal value { get; set; }

    public DateTime date { get; set; }
}
