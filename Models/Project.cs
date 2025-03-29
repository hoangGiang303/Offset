using System;
using System.Collections.Generic;

namespace Offset.Models;

public partial class Project
{
    public int Id { get; set; }

    public string? ProjectName { get; set; }

    public string? Description { get; set; }

    public string? ProjectImage { get; set; }

    public decimal? Price { get; set; }

    public int? Availibility { get; set; }
}
