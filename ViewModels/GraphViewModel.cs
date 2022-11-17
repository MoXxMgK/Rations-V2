using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ZedGraph;
using Rations_V2.Models;
using System.Windows;

namespace Rations_V2.ViewModels
{
    public class GraphViewModel : BaseViewModel
    {
        protected ZedGraphControl _graph;
        protected Cow _cow;

        public Command FillGraphCommand { get; set; }

        public GraphViewModel(ZedGraphControl graph, Cow cow)
        {
            FillGraphCommand = new((o) => FillGraph());

            _graph = graph;
            _cow = cow;
        }

        protected virtual void FillGraph()
        {

        }
    }
}
