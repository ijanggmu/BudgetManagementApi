using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.BeemaEdgeApi.ITI;
public class QuestionaryResponseViewModel
{
    public MetaDto Meta { get; set; }
    public IEnumerable<QuestionaryViewModel> Data { get; set; }
}
public class QuestionaryViewModel
{
    public int Position { get; set; }
    public string Type { get; set; }
    public string Question { get; set; }
   
}
