using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Models.BeemaEdgeApi.ITI;
public class ConfigurationResponseViewModel
{
    public MetaDto Meta { get; set; }
    public IEnumerable<ItiConfigurationViewModel> Data { get; set; }
}

public class MetaDto
{
    public string Copyright { get; set; }
    public string Email { get; set; }
    public ApiDto Api { get; set; }
}

public class ApiDto
{
    public string Version { get; set; }
}

public class ItiConfigurationViewModel
{
    public string? Title { get; set; }

    public string? Slug { get; set; }

    public bool Status { get; set; }
}
