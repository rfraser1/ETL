using System;
using System.Collections.Generic;

namespace ETL.DBModels;

public partial class AnnualSummary
{
    public string session { get; set; } = null!;

    public int? RecordYear { get; set; }

    public int? DataCount { get; set; }
}
