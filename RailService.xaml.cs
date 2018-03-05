using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Parsing;

namespace ScheduleVis
{
    /// <summary>
    /// Interaction logic for RailService.xaml
    /// </summary>
    public partial class RailService : UserControl
    {
        public const int RSWIDTH = 300;
        public const int RSSPACE = 10;
        

        UriNode service;

        public RailService()
        {            
            InitializeComponent();
            this.Width = RSWIDTH;
            this.Height = 280;//for now
        }


        public RailService(UriNode _service)
            : this()
        {
            service = _service;
        }

        public static RailService CreateFromNode(UriNode _service)
        {
            RailService result = new RailService(_service);

            result.txtName.Text = _service.ToDisplayString();
            return result;
        }

        private void btnGetCallingPoints_Click(object sender, RoutedEventArgs e)
        {
            //Create a Parameterized String
            SparqlParameterizedString queryString = new SparqlParameterizedString();
            ProgramState.AddCommonNamespaces(queryString);

            queryString.CommandText = Properties.Settings.Default.GetServiceNodes;
            queryString.SetUri("service", this.service.Uri);
            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlQuery query = parser.ParseFromString(queryString);
            IGraph serviceNodesGraph = ProgramState.TheDataSource.GetSparlAsGraph(query);
            service.Graph.Merge(serviceNodesGraph);
           
        }


    }
}
